package com.driverguard.service

import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.PendingIntent
import android.content.Context
import android.content.Intent
import androidx.core.app.NotificationCompat
import com.driverguard.MainActivity
import com.driverguard.R
import com.driverguard.data.api.RetrofitClient
import com.driverguard.data.local.TokenManager
import com.driverguard.data.repository.UserRepository
import com.google.firebase.messaging.FirebaseMessagingService
import com.google.firebase.messaging.RemoteMessage
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.launch

class DriverGuardFirebaseService : FirebaseMessagingService() {

    companion object {
        const val CHANNEL_ID = "driverguard_alerts"
        private const val CHANNEL_NAME = "DriverGuard Alerts"
    }

    override fun onMessageReceived(message: RemoteMessage) {
        val title = message.notification?.title ?: "DriverGuard"
        val body  = message.notification?.body  ?: return

        showNotification(title, body)
    }

    // Called when FCM token is refreshed — sync to backend
    override fun onNewToken(token: String) {
        CoroutineScope(Dispatchers.IO).launch {
            val tokenManager = TokenManager(applicationContext)
            val jwt = tokenManager.token.first() ?: return@launch
            RetrofitClient.setToken(jwt)
            UserRepository(RetrofitClient.rebuildApi()).saveFcmToken(token)
        }
    }

    private fun showNotification(title: String, body: String) {
        val manager = getSystemService(Context.NOTIFICATION_SERVICE) as NotificationManager

        val channel = NotificationChannel(
            CHANNEL_ID,
            CHANNEL_NAME,
            NotificationManager.IMPORTANCE_HIGH
        ).apply { description = "Driver safety alerts" }
        manager.createNotificationChannel(channel)

        val intent = Intent(this, MainActivity::class.java).apply {
            flags = Intent.FLAG_ACTIVITY_CLEAR_TOP
        }
        val pending = PendingIntent.getActivity(
            this, 0, intent,
            PendingIntent.FLAG_ONE_SHOT or PendingIntent.FLAG_IMMUTABLE
        )

        val notification = NotificationCompat.Builder(this, CHANNEL_ID)
            .setSmallIcon(R.drawable.ic_notification)
            .setContentTitle(title)
            .setContentText(body)
            .setAutoCancel(true)
            .setPriority(NotificationCompat.PRIORITY_HIGH)
            .setContentIntent(pending)
            .build()

        manager.notify(System.currentTimeMillis().toInt(), notification)
    }
}
