package com.driverguard.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.driverguard.data.api.RetrofitClient
import com.driverguard.data.model.Device
import com.driverguard.data.model.DeviceConfiguration
import com.driverguard.data.model.DriverEvent
import com.driverguard.data.repository.DeviceRepository
import com.driverguard.data.repository.EventRepository
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch

class DeviceDetailViewModel : ViewModel() {

    private val eventRepo = EventRepository(RetrofitClient.api)
    private val deviceRepo = DeviceRepository(RetrofitClient.api)

    private var pollingJob: Job? = null

    private val _device = MutableStateFlow<Device?>(null)
    val device: StateFlow<Device?> = _device

    private val _events = MutableStateFlow<List<DriverEvent>>(emptyList())
    val events: StateFlow<List<DriverEvent>> = _events

    private val _config = MutableStateFlow<DeviceConfiguration?>(null)
    val config: StateFlow<DeviceConfiguration?> = _config

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    private val _configMessage = MutableStateFlow<String?>(null)
    val configMessage: StateFlow<String?> = _configMessage

    private val _deleted = MutableStateFlow(false)
    val deleted: StateFlow<Boolean> = _deleted

    fun load(deviceId: String) {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null
            eventRepo.getEventsByDevice(deviceId)
                .onSuccess { _events.value = it }
                .onFailure { _error.value = it.message }
            deviceRepo.getDevice(deviceId)
                .onSuccess { _device.value = it }
            deviceRepo.getConfiguration(deviceId)
                .onSuccess { _config.value = it }
            _isLoading.value = false
        }
        startPolling(deviceId)
    }

    private fun startPolling(deviceId: String) {
        pollingJob?.cancel()
        pollingJob = viewModelScope.launch {
            while (isActive) {
                delay(5_000)
                eventRepo.getEventsByDevice(deviceId)
                    .onSuccess { _events.value = it }
                deviceRepo.getDevice(deviceId)
                    .onSuccess { _device.value = it }
            }
        }
    }

    fun loadEvents(deviceId: String) = load(deviceId)

    override fun onCleared() {
        super.onCleared()
        pollingJob?.cancel()
    }

    fun updateConfiguration(deviceId: String, drowsiness: Double, attention: Double) {
        viewModelScope.launch {
            deviceRepo.updateConfiguration(deviceId, drowsiness, attention)
                .onSuccess {
                    _configMessage.value = "Конфігурацію збережено"
                    _config.value = _config.value?.copy(
                        drowsinessThreshold = drowsiness,
                        attentionThreshold = attention
                    )
                }
                .onFailure { _configMessage.value = it.message }
        }
    }

    fun clearConfigMessage() { _configMessage.value = null }

    fun deleteDevice(deviceId: String) {
        viewModelScope.launch {
            deviceRepo.deleteDevice(deviceId)
                .onSuccess { _deleted.value = true }
                .onFailure { _configMessage.value = it.message }
        }
    }
}
