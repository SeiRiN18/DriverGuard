#pragma once

struct DriverEvent {
  const char* eventType;
  int severity;
  float confidence;
  unsigned long occurredAt;
};

