package com.driverguard.data.repository

import com.driverguard.data.api.ApiService
import com.driverguard.data.model.FcmTokenRequest
import com.driverguard.data.model.UpdateUserRequest
import com.driverguard.data.model.UserProfile

class UserRepository(private val api: ApiService) {

    suspend fun getProfile(userId: String): Result<UserProfile> =
        runCatching {
            val response = api.getUserProfile(userId)
            if (response.isSuccessful) response.body()!!
            else error("Помилка завантаження профілю: ${response.code()}")
        }

    suspend fun updateProfile(userId: String, email: String, password: String): Result<Unit> =
        runCatching {
            val response = api.updateUserProfile(userId, UpdateUserRequest(email, password))
            if (!response.isSuccessful) error("Помилка оновлення профілю: ${response.code()}")
        }

    suspend fun saveFcmToken(fcmToken: String): Result<Unit> =
        runCatching {
            val response = api.updateFcmToken(FcmTokenRequest(fcmToken))
            if (!response.isSuccessful) error("Помилка збереження FCM токена: ${response.code()}")
        }
}
