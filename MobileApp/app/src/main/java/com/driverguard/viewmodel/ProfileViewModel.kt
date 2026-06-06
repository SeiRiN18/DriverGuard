package com.driverguard.viewmodel

import android.content.Context
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.driverguard.data.api.RetrofitClient
import com.driverguard.data.local.TokenManager
import com.driverguard.data.model.UserProfile
import com.driverguard.data.repository.UserRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.launch

class ProfileViewModel(context: Context) : ViewModel() {

    private val repo = UserRepository(RetrofitClient.api)
    private val tokenManager = TokenManager(context)

    private val _profile = MutableStateFlow<UserProfile?>(null)
    val profile: StateFlow<UserProfile?> = _profile

    private val _isLoading = MutableStateFlow(false)
    val isLoading: StateFlow<Boolean> = _isLoading

    private val _message = MutableStateFlow<String?>(null)
    val message: StateFlow<String?> = _message

    fun load() {
        viewModelScope.launch {
            _isLoading.value = true
            val userId = tokenManager.userId.first() ?: return@launch
            repo.getProfile(userId)
                .onSuccess { _profile.value = it }
                .onFailure { _message.value = it.message }
            _isLoading.value = false
        }
    }

    fun updateProfile(email: String, password: String) {
        viewModelScope.launch {
            val userId = tokenManager.userId.first() ?: return@launch
            repo.updateProfile(userId, email, password)
                .onSuccess { _message.value = "Профіль оновлено" }
                .onFailure { _message.value = it.message }
        }
    }

    fun clearMessage() { _message.value = null }
}
