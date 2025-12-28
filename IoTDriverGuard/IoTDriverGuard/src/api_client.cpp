#include <HTTPClient.h>
#include <time.h>
#include "api_client.h"
#include "config.h"
#include "wifi_manager.h"

static void formatIsoTimestamp(unsigned long ms_since_boot, char* out, size_t out_size) {
  time_t now = time(nullptr);
  if (now < 100000) {
    now = ms_since_boot / 1000;
  }

  struct tm tm_utc;
  gmtime_r(&now, &tm_utc);
  snprintf(out, out_size, "%04d-%02d-%02dT%02d:%02d:%02dZ",
           tm_utc.tm_year + 1900,
           tm_utc.tm_mon + 1,
           tm_utc.tm_mday,
           tm_utc.tm_hour,
           tm_utc.tm_min,
           tm_utc.tm_sec);
}

bool sendEventToServer(const DriverEvent& ev) {
  if (!isWiFiConnected()) return false;

  HTTPClient http;
  http.begin(String(SERVER_URL) + EVENTS_ENDPOINT);
  http.addHeader("Content-Type", "application/json");
  http.addHeader("X-Device-Key", DEVICE_API_KEY);

  char occurred_at[25];
  formatIsoTimestamp(ev.occurredAt, occurred_at, sizeof(occurred_at));

  String payload = "{";
  payload += "\"eventType\":\"" + String(ev.eventType) + "\",";
  payload += "\"severity\":" + String(ev.severity) + ",";
  payload += "\"confidence\":" + String(ev.confidence, 2) + ",";
  payload += "\"occurredAt\":\"" + String(occurred_at) + "\"";
  payload += "}";

  int code = http.POST(payload);
  http.end();

  return code == 200 || code == 201;
}
