package com.driverguard.data.api

import com.driverguard.data.model.*
import retrofit2.Response
import retrofit2.http.*

interface ApiService {

    // --- Auth ---
    @POST("api/auth/login")
    suspend fun login(@Body request: LoginRequest): Response<LoginResponse>

    @POST("api/auth/register")
    suspend fun register(@Body request: RegisterRequest): Response<Unit>

    // --- Devices ---
    @GET("api/devices/my")
    suspend fun getMyDevices(): Response<List<Device>>

    @POST("api/devices")
    suspend fun createDevice(@Body request: CreateDeviceRequest): Response<CreateDeviceResponse>

    @GET("api/devices/{deviceId}/configuration")
    suspend fun getDeviceConfiguration(@Path("deviceId") deviceId: String): Response<DeviceConfiguration>

    @PUT("api/devices/{deviceId}/configuration")
    suspend fun updateDeviceConfiguration(
        @Path("deviceId") deviceId: String,
        @Body request: UpdateDeviceConfigurationRequest
    ): Response<Unit>

    @DELETE("api/devices/{deviceId}")
    suspend fun deleteDevice(@Path("deviceId") deviceId: String): Response<Unit>

    // --- Events ---
    @GET("api/events/device/{deviceId}")
    suspend fun getEventsByDevice(@Path("deviceId") deviceId: String): Response<List<DriverEvent>>

    // --- Notifications ---
    @GET("api/notifications/my")
    suspend fun getMyNotifications(): Response<List<AppNotification>>

    @PUT("api/notifications/{id}/read")
    suspend fun markNotificationRead(@Path("id") id: String): Response<Unit>

    // --- Users ---
    @GET("api/users/{id}")
    suspend fun getUserProfile(@Path("id") id: String): Response<UserProfile>

    @PUT("api/users/{id}")
    suspend fun updateUserProfile(
        @Path("id") id: String,
        @Body request: UpdateUserRequest
    ): Response<Unit>

    @PUT("api/users/me/fcm-token")
    suspend fun updateFcmToken(@Body request: FcmTokenRequest): Response<Unit>

    // --- Password reset ---
    @POST("api/auth/reset-password")
    suspend fun resetPassword(@Body request: ResetPasswordRequest): Response<Unit>
}
