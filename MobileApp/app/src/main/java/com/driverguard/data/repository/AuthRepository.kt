package com.driverguard.data.repository

import com.driverguard.data.api.ApiService
import com.driverguard.data.model.LoginRequest
import com.driverguard.data.model.LoginResponse
import com.driverguard.data.model.RegisterRequest
import com.driverguard.data.model.ResetPasswordRequest

class AuthRepository(private val api: ApiService) {

    suspend fun login(email: String, password: String): Result<LoginResponse> =
        runCatching {
            val response = api.login(LoginRequest(email, password))
            if (response.isSuccessful) response.body()!!
            else error("Невірний email або пароль")
        }

    suspend fun register(email: String, password: String): Result<Unit> =
        runCatching {
            val response = api.register(RegisterRequest(email, password))
            if (!response.isSuccessful) error("Помилка реєстрації: ${response.code()}")
        }

    suspend fun resetPassword(email: String, newPassword: String): Result<Unit> =
        runCatching {
            val response = api.resetPassword(ResetPasswordRequest(email, newPassword))
            if (!response.isSuccessful) error("Користувача з таким email не знайдено")
        }
}
