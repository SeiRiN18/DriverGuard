package com.driverguard.data.model

data class DriverEvent(
    val id: String,
    val deviceId: String,
    val eventType: String,
    val severity: Int,
    val confidence: Double,
    val occurredAt: String
)
