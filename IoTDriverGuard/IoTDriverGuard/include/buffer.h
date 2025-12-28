#pragma once
#include "event.h"

void bufferEvent(const char* eventType, int severity, float confidence);

bool hasBufferedEvents();
DriverEvent getNextBufferedEvent();
void popBufferedEvent();
