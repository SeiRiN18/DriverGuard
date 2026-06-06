package com.driverguard.util

import android.util.Base64
import org.json.JSONObject

object JwtDecoder {
    fun getUserId(token: String): String? = runCatching {
        val payload = token.split(".")[1]
        val decoded = Base64.decode(payload, Base64.URL_SAFE or Base64.NO_PADDING)
        JSONObject(String(decoded)).getString("sub")
    }.getOrNull()
}
