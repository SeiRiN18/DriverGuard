package com.driverguard.data.model

data class LoginRequest(
    val email: String,
    val password: String
)

data class RegisterRequest(
    val email: String,
    val password: String
)

data class LoginResponse(
    val token: String,
    val role: String
)

data class UpdateUserRequest(
    val email: String,
    val password: String
)

data class ResetPasswordRequest(
    val email: String,
    val newPassword: String
)

data class FcmTokenRequest(
    val fcmToken: String
)
