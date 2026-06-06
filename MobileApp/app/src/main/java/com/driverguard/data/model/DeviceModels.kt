package com.driverguard.data.model

data class Device(
    val id: String,
    val serialNumber: String,
    val isActive: Boolean,
    val lastSeenAt: String?
)

data class CreateDeviceRequest(
    val serialNumber: String
)

data class CreateDeviceResponse(
    val id: String,
    val serialNumber: String,
    val apiKey: String
)

data class DeviceConfiguration(
    val deviceId: String,
    val drowsinessThreshold: Double,
    val attentionThreshold: Double,
    val updatedAt: String
)

data class UpdateDeviceConfigurationRequest(
    val drowsinessThreshold: Double,
    val attentionThreshold: Double
)

data class UserProfile(
    val id: String,
    val email: String,
    val createdAt: String
)
