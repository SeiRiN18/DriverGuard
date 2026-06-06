package com.driverguard

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import com.driverguard.ui.navigation.NavGraph
import com.driverguard.ui.theme.DriverGuardTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            DriverGuardTheme {
                NavGraph()
            }
        }
    }
}
