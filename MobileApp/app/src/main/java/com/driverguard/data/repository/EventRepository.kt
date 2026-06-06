package com.driverguard.data.repository

import com.driverguard.data.api.ApiService
import com.driverguard.data.model.DriverEvent

class EventRepository(private val api: ApiService) {

    suspend fun getEventsByDevice(deviceId: String): Result<List<DriverEvent>> =
        runCatching {
            val response = api.getEventsByDevice(deviceId)
            if (response.isSuccessful) response.body()!!
            else error("Помилка завантаження подій: ${response.code()}")
        }
}
