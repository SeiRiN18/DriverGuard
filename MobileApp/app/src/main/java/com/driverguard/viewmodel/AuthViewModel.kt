package com.driverguard.viewmodel

import android.content.Context
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.driverguard.data.api.RetrofitClient
import com.driverguard.data.local.TokenManager
import com.driverguard.data.repository.AuthRepository
import com.driverguard.data.repository.UserRepository
import com.driverguard.util.JwtDecoder
import com.google.firebase.messaging.FirebaseMessaging
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.launch
import kotlinx.coroutines.tasks.await

sealed class AuthState {
    object Idle : AuthState()
    object Loading : AuthState()
    object Success : AuthState()
    data class Error(val message: String) : AuthState()
}

class AuthViewModel(context: Context) : ViewModel() {

    private val tokenManager = TokenManager(context)
    private val repo = AuthRepository(RetrofitClient.api)

    private val _state = MutableStateFlow<AuthState>(AuthState.Idle)
    val state: StateFlow<AuthState> = _state

    private val _isLoggedIn = MutableStateFlow(false)
    val isLoggedIn: StateFlow<Boolean> = _isLoggedIn

    init {
        viewModelScope.launch {
            val token = tokenManager.token.first()
            if (!token.isNullOrEmpty()) {
                RetrofitClient.setToken(token)
                _isLoggedIn.value = true
                syncFcmToken()
            }
        }
    }

    fun login(email: String, password: String) {
        viewModelScope.launch {
            _state.value = AuthState.Loading
            repo.login(email, password)
                .onSuccess { response ->
                    val userId = JwtDecoder.getUserId(response.token) ?: ""
                    tokenManager.saveSession(response.token, userId, response.role)
                    RetrofitClient.setToken(response.token)
                    _state.value = AuthState.Success
                    _isLoggedIn.value = true
                    syncFcmToken()
                }
                .onFailure { _state.value = AuthState.Error(it.message ?: "Помилка входу") }
        }
    }

    fun register(email: String, password: String) {
        viewModelScope.launch {
            _state.value = AuthState.Loading
            repo.register(email, password)
                .onSuccess { _state.value = AuthState.Success }
                .onFailure { _state.value = AuthState.Error(it.message ?: "Помилка реєстрації") }
        }
    }

    fun resetPassword(email: String, newPassword: String) {
        viewModelScope.launch {
            _state.value = AuthState.Loading
            repo.resetPassword(email, newPassword)
                .onSuccess { _state.value = AuthState.Success }
                .onFailure { _state.value = AuthState.Error(it.message ?: "Помилка скидання пароля") }
        }
    }

    fun logout() {
        viewModelScope.launch {
            tokenManager.clearSession()
            RetrofitClient.setToken(null)
            _isLoggedIn.value = false
            _state.value = AuthState.Idle
        }
    }

    fun resetState() { _state.value = AuthState.Idle }

    private suspend fun syncFcmToken() {
        runCatching {
            val fcmToken = FirebaseMessaging.getInstance().token.await()
            UserRepository(RetrofitClient.rebuildApi()).saveFcmToken(fcmToken)
        }
    }
}
