package com.driverguard.data.repository

import com.driverguard.data.api.ApiService
import com.driverguard.data.model.AppNotification

class NotificationRepository(private val api: ApiService) {

    suspend fun getMyNotifications(): Result<List<AppNotification>> =
        runCatching {
            val response = api.getMyNotifications()
            if (response.isSuccessful) response.body()!!
            else error("Помилка завантаження сповіщень: ${response.code()}")
        }

    suspend fun markAsRead(id: String): Result<Unit> =
        runCatching {
            val response = api.markNotificationRead(id)
            if (!response.isSuccessful) error("Помилка: ${response.code()}")
        }
}
