package com.driverguard.data.repository

import com.driverguard.data.api.ApiService
import com.driverguard.data.model.CreateDeviceRequest
import com.driverguard.data.model.CreateDeviceResponse
import com.driverguard.data.model.Device
import com.driverguard.data.model.DeviceConfiguration
import com.driverguard.data.model.UpdateDeviceConfigurationRequest

class DeviceRepository(private val api: ApiService) {

    suspend fun getMyDevices(): Result<List<Device>> =
        runCatching {
            val response = api.getMyDevices()
            if (response.isSuccessful) response.body()!!
            else error("Помилка завантаження пристроїв: ${response.code()}")
        }

    suspend fun createDevice(serialNumber: String): Result<CreateDeviceResponse> =
        runCatching {
            val response = api.createDevice(CreateDeviceRequest(serialNumber))
            if (response.isSuccessful) response.body()!!
            else error("Помилка створення пристрою: ${response.code()}")
        }

    suspend fun getConfiguration(deviceId: String): Result<DeviceConfiguration> =
        runCatching {
            val response = api.getDeviceConfiguration(deviceId)
            if (response.isSuccessful) response.body()!!
            else error("Конфігурацію не знайдено")
        }

    suspend fun updateConfiguration(
        deviceId: String,
        drowsinessThreshold: Double,
        attentionThreshold: Double
    ): Result<Unit> = runCatching {
        val response = api.updateDeviceConfiguration(
            deviceId,
            UpdateDeviceConfigurationRequest(drowsinessThreshold, attentionThreshold)
        )
        if (!response.isSuccessful) error("Помилка оновлення конфігурації: ${response.code()}")
    }

    suspend fun deleteDevice(deviceId: String): Result<Unit> = runCatching {
        val response = api.deleteDevice(deviceId)
        if (!response.isSuccessful) error("Помилка видалення пристрою: ${response.code()}")
    }
}
