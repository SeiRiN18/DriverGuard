package com.driverguard.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.driverguard.data.api.RetrofitClient
import com.driverguard.data.model.AppNotification
import com.driverguard.data.repository.NotificationRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

class NotificationsViewModel : ViewModel() {

    private val repo = NotificationRepository(RetrofitClient.api)

    private val _notifications = MutableStateFlow<List<AppNotification>>(emptyList())
    val notifications: StateFlow<List<AppNotification>> = _notifications

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    fun load() {
        viewModelScope.launch {
            _isLoading.value = true
            repo.getMyNotifications()
                .onSuccess { _notifications.value = it }
            _isLoading.value = false
        }
    }

    fun markAsRead(id: String) {
        viewModelScope.launch {
            repo.markAsRead(id).onSuccess {
                _notifications.value = _notifications.value.map { n ->
                    if (n.id == id) n.copy(isRead = true) else n
                }
            }
        }
    }
}
