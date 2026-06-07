#include <Arduino.h>
#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <cstring>


#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "freertos/queue.h"

#include "config.h"
#include "wifi_manager.h"
#include "buffer.h"
#include "api_client.h"


constexpr int PIN_RED = 26;
constexpr int PIN_GREEN = 27;
constexpr int PIN_BLUE = 14; 
constexpr int PIN_DROWSY = 34;
constexpr int PIN_ATTENTION = 35;
constexpr int PIN_BUZZER = 25;


constexpr uint8_t MPU_ADDR = 0x68;
constexpr int OLED_WIDTH = 128;
constexpr int OLED_HEIGHT = 64;
constexpr unsigned long BUZZER_BEEP_MS = 500;
constexpr unsigned long BUZZER_COOLDOWN_MS = 2000;


unsigned long lastBuzzMs = 0;
unsigned long buzzerUntilMs = 0;
unsigned long lastUiMs = 0;
unsigned long lastSend = 0; 
int lastReportedSeverity = 0;
const char* lastReportedEventType = nullptr;

Adafruit_SSD1306 display(OLED_WIDTH, OLED_HEIGHT, &Wire, -1);
bool displayOk = false;
bool mpuOk = false;

QueueHandle_t eventQueue;



void setLedColor(bool r, bool g, bool b) {
  digitalWrite(PIN_RED, r ? HIGH : LOW);
  digitalWrite(PIN_GREEN, g ? HIGH : LOW);
  digitalWrite(PIN_BLUE, b ? HIGH : LOW);
}

bool readAccel(float& ax_g, float& ay_g, float& az_g) {
  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x3B);
  if (Wire.endTransmission(false) != 0) return false;
  if (Wire.requestFrom(MPU_ADDR, (uint8_t)6, (uint8_t)true) != 6) return false;

  int16_t rawX = (Wire.read() << 8) | Wire.read();
  int16_t rawY = (Wire.read() << 8) | Wire.read();
  int16_t rawZ = (Wire.read() << 8) | Wire.read();
  
  ax_g = rawX / 16384.0f;
  ay_g = rawY / 16384.0f;
  az_g = rawZ / 16384.0f;
  return true;
}

void updateDisplay(float drowsy, float attention, int severity, bool headDown) {
  if (!displayOk) return;
  if (millis() - lastUiMs < 33) return; 
  lastUiMs = millis();

  display.clearDisplay();
  display.setTextColor(SSD1306_WHITE);
  display.setTextSize(1);

  display.setCursor(0, 0);
  display.printf("Drowsy: %d%%", (int)(drowsy * 100));

  display.setCursor(0, 12);
  display.printf("Attention: %d%%", (int)(attention * 100));

  display.setCursor(0, 24);
  display.print("Head: ");
  display.print(headDown ? "DOWN" : "OK");

  display.setCursor(0, 40);
  display.print("Risk: ");
  for (int i = 1; i <= 5; i++) {
    if (i <= severity) display.print((char)219); 
    else display.print((char)176);                
  }
  display.display();
}


void networkTask(void * parameter) {
  Serial.println("[NetTask] started on core 0");
  DriverEvent ev;
  for(;;) {

    if (xQueueReceive(eventQueue, &ev, portMAX_DELAY)) {

      Serial.println("[Task] Received event, sending to server...");
      
     
      if (!sendEventToServer(ev)) {
         Serial.println("[Task] Send failed, buffering...");
         bufferEvent(ev.eventType, ev.severity, ev.confidence);
      } else {
         Serial.println("[Task] Send OK!");
      }

      
      if (WiFi.isConnected() && hasBufferedEvents()) {
         DriverEvent oldEv = getNextBufferedEvent();
         if (sendEventToServer(oldEv)) {
            popBufferedEvent();
         }
      }
    }
  }
}

// 
void setup() {
  Serial.begin(115200);
  
  pinMode(PIN_RED, OUTPUT);
  pinMode(PIN_GREEN, OUTPUT);
  pinMode(PIN_BLUE, OUTPUT);
  pinMode(PIN_BUZZER, OUTPUT);
  pinMode(PIN_DROWSY, INPUT);
  pinMode(PIN_ATTENTION, INPUT);
  
  setLedColor(1, 0, 0); delay(300);
  setLedColor(0, 1, 0); delay(300);
  setLedColor(0, 0, 1); delay(300);
  setLedColor(0, 0, 0); 

  Wire.begin(21, 22);

  Wire.beginTransmission(MPU_ADDR);
  Wire.write(0x6B); Wire.write(0);
  if (Wire.endTransmission(true) == 0) {
    mpuOk = true;
    Serial.println("MPU6050 OK");
  }

  displayOk = display.begin(SSD1306_SWITCHCAPVCC, 0x3C);
  if (displayOk) {
    display.clearDisplay();
    display.display();
  }

 
  eventQueue = xQueueCreate(10, sizeof(DriverEvent));

  
  connectWiFi();

 
  xTaskCreatePinnedToCore(
    networkTask,
    "NetTask",
    8192,
    NULL,
    1,
    NULL,
    1              // core 1 — same as Arduino loop, Serial works in Wokwi
  );
}


void loop() {
  float drowsy = analogRead(PIN_DROWSY) / 4095.0f;
  float attention = analogRead(PIN_ATTENTION) / 4095.0f;

  float ax_g = 0, ay_g = 0, az_g = 0;
  bool accel_ok = mpuOk && readAccel(ax_g, ay_g, az_g);

  bool headDown = false;
  if (accel_ok) {
    if (abs(ax_g) > 0.4 || abs(ay_g) > 0.4) headDown = true;
  }


  int score = 0;
  if (drowsy > 0.7f) score += 2;
  else if (drowsy > 0.4f) score += 1;

  if (attention < 0.3f) score += 2;
  else if (attention < 0.6f) score += 1;

  if (headDown) score += 2;

  int severity = 1;
  if (score <= 1) severity = 1;
  else if (score == 2) severity = 2;
  else if (score == 3) severity = 3;
  else if (score == 4) severity = 4;
  else severity = 5;

  float confidence = (drowsy * 0.4f) + ((1.0f - attention) * 0.4f) + (headDown ? 0.2f : 0.0f);
  confidence = constrain(confidence, 0.0f, 1.0f);


  if (severity <= 2) { setLedColor(0, 1, 0); } 
  else if (severity <= 4) { setLedColor(1, 1, 0); } 
  else {
    setLedColor(1, 0, 0);
    if (millis() > buzzerUntilMs && millis() - lastBuzzMs >= BUZZER_COOLDOWN_MS) {
      lastBuzzMs = millis();
      buzzerUntilMs = lastBuzzMs + BUZZER_BEEP_MS;
    }
  }
  
  if (severity < 5) digitalWrite(PIN_BUZZER, LOW);
  else digitalWrite(PIN_BUZZER, (millis() <= buzzerUntilMs) ? HIGH : LOW);


  updateDisplay(drowsy, attention, severity, headDown);


  const char* eventType = "NORMAL";
  if (severity > 2) {
    float drowsyScore = drowsy;
    float attentionScore = 1.0f - attention;
    if (drowsyScore >= attentionScore) {
      eventType = "DROWSINESS";
    } else {
      eventType = "ATTENTION_LOSS";
    }
  }

  const bool eventTypeChanged = (lastReportedEventType == nullptr) || (std::strcmp(eventType, lastReportedEventType) != 0);
  const bool severityChanged = (severity != lastReportedSeverity);

  if ((eventTypeChanged || severityChanged) && (millis() - lastSend >= SEND_INTERVAL_MS)) {
    lastSend = millis();
    

    DriverEvent ev = {
      eventType,
      severity,
      confidence,
      millis()
    };

   
    lastReportedSeverity = severity;
    lastReportedEventType = eventType;

   
    Serial.printf("[Loop] Queuing event: type=%s sev=%d conf=%.2f\n", eventType, severity, confidence);
    xQueueSend(eventQueue, &ev, 0);
  }

  delay(50);
}