package com.driverguard.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.driverguard.data.api.RetrofitClient
import com.driverguard.data.model.CreateDeviceResponse
import com.driverguard.data.model.Device
import com.driverguard.data.repository.DeviceRepository
import com.driverguard.data.repository.NotificationRepository
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch

class DashboardViewModel : ViewModel() {

    private val deviceRepo = DeviceRepository(RetrofitClient.api)
    private val notifRepo = NotificationRepository(RetrofitClient.api)

    private var pollingJob: Job? = null

    private val _devices = MutableStateFlow<List<Device>>(emptyList())
    val devices: StateFlow<List<Device>> = _devices

    private val _unreadCount = MutableStateFlow(0)
    val unreadCount: StateFlow<Int> = _unreadCount

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    private val _newDevice = MutableStateFlow<CreateDeviceResponse?>(null)
    val newDevice: StateFlow<CreateDeviceResponse?> = _newDevice

    fun load() {
        viewModelScope.launch {
            _isLoading.value = true
            _error.value = null

            deviceRepo.getMyDevices()
                .onSuccess { _devices.value = it }
                .onFailure { _error.value = it.message }

            notifRepo.getMyNotifications()
                .onSuccess { _unreadCount.value = it.count { n -> !n.isRead } }

            _isLoading.value = false
        }
        startPolling()
    }

    private fun startPolling() {
        pollingJob?.cancel()
        pollingJob = viewModelScope.launch {
            while (isActive) {
                delay(5_000)
                deviceRepo.getMyDevices().onSuccess { _devices.value = it }
            }
        }
    }

    override fun onCleared() {
        super.onCleared()
        pollingJob?.cancel()
    }

    fun addDevice(serialNumber: String) {
        viewModelScope.launch {
            deviceRepo.createDevice(serialNumber)
                .onSuccess {
                    _newDevice.value = it
                    load()
                }
                .onFailure { _error.value = it.message }
        }
    }

    fun clearNewDevice() { _newDevice.value = null }
    fun clearError() { _error.value = null }
}
