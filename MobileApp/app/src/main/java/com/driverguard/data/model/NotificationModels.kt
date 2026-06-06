package com.driverguard.data.model

data class AppNotification(
    val id: String,
    val deviceId: String,
    val driverEventId: String,
    val type: String,
    val message: String,
    val isRead: Boolean,
    val createdAt: String
)
