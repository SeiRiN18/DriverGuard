#include <WiFi.h>
#include <time.h>
#include "wifi_manager.h"
#include "config.h"

void connectWiFi() {
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
  }

  configTime(0, 0, "pool.ntp.org", "time.nist.gov");
  time_t now = time(nullptr);
  const unsigned long start_ms = millis();
  while (now < 1700000000 && (millis() - start_ms) < 5000) {
    delay(200);
    now = time(nullptr);
  }
}

bool isWiFiConnected() {
  return WiFi.status() == WL_CONNECTED;
}
