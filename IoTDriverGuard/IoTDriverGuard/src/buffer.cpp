#include "buffer.h"
#include "config.h"
#include <Arduino.h>

static DriverEvent buffer[MAX_BUFFERED_EVENTS];
static int bufferSize = 0;

void bufferEvent(const char* eventType, int severity, float confidence) {
  if (bufferSize >= MAX_BUFFERED_EVENTS) {
    for (int i = 1; i < bufferSize; i++) {
      buffer[i - 1] = buffer[i];
    }
    bufferSize--;
  }

  buffer[bufferSize++] = {
    eventType,
    severity,
    confidence,
    millis()
  };
}


bool hasBufferedEvents() {
  return bufferSize > 0;
}

DriverEvent getNextBufferedEvent() {
  return buffer[0];
}

void popBufferedEvent() {
  if (bufferSize == 0) return;

  for (int i = 1; i < bufferSize; i++) {
    buffer[i - 1] = buffer[i];
  }
  bufferSize--;
}
