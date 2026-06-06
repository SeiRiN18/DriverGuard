package com.driverguard.ui.navigation

import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.remember
import androidx.compose.ui.platform.LocalContext
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.driverguard.data.api.RetrofitClient
import com.driverguard.ui.screens.DashboardScreen
import com.driverguard.ui.screens.DeviceDetailScreen
import com.driverguard.ui.screens.ForgotPasswordScreen
import com.driverguard.ui.screens.LoginScreen
import com.driverguard.ui.screens.NotificationsScreen
import com.driverguard.ui.screens.ProfileScreen
import com.driverguard.ui.screens.RegisterScreen
import com.driverguard.viewmodel.*

sealed class Screen(val route: String) {
    object Login : Screen("login")
    object Register : Screen("register")
    object ForgotPassword : Screen("forgot_password")
    object Dashboard : Screen("dashboard")
    object DeviceDetail : Screen("device/{deviceId}/{serialNumber}/{isActive}/{lastSeen}") {
        fun createRoute(deviceId: String, serialNumber: String, isActive: Boolean, lastSeen: String) =
            "device/$deviceId/$serialNumber/$isActive/${lastSeen.ifEmpty { "null" }}"
    }
    object Notifications : Screen("notifications")
    object Profile : Screen("profile")
}

@Composable
fun NavGraph() {
    val context = LocalContext.current
    val navController = rememberNavController()

    val authViewModel = remember { AuthViewModel(context) }
    val isLoggedIn by authViewModel.isLoggedIn.collectAsState()

    val startDestination = if (isLoggedIn) Screen.Dashboard.route else Screen.Login.route

    NavHost(navController = navController, startDestination = startDestination) {

        composable(Screen.Login.route) {
            LoginScreen(
                viewModel = authViewModel,
                onLoginSuccess = {
                    navController.navigate(Screen.Dashboard.route) {
                        popUpTo(Screen.Login.route) { inclusive = true }
                    }
                },
                onNavigateToRegister = {
                    navController.navigate(Screen.Register.route)
                },
                onForgotPassword = {
                    navController.navigate(Screen.ForgotPassword.route)
                }
            )
        }

        composable(Screen.ForgotPassword.route) {
            ForgotPasswordScreen(
                viewModel = authViewModel,
                onSuccess = {
                    navController.navigate(Screen.Login.route) {
                        popUpTo(Screen.ForgotPassword.route) { inclusive = true }
                    }
                },
                onBack = { navController.popBackStack() }
            )
        }

        composable(Screen.Register.route) {
            RegisterScreen(
                viewModel = authViewModel,
                onRegisterSuccess = {
                    navController.navigate(Screen.Login.route) {
                        popUpTo(Screen.Register.route) { inclusive = true }
                    }
                },
                onBack = { navController.popBackStack() }
            )
        }

        composable(Screen.Dashboard.route) {
            val dashboardViewModel: DashboardViewModel = viewModel()
            val notificationsViewModel: NotificationsViewModel = viewModel()
            DashboardScreen(
                viewModel = dashboardViewModel,
                onDeviceClick = { device ->
                    navController.navigate(
                        Screen.DeviceDetail.createRoute(
                            device.id, device.serialNumber, device.isActive, device.lastSeenAt ?: ""
                        )
                    )
                },
                onNotificationsClick = { navController.navigate(Screen.Notifications.route) },
                onProfileClick = { navController.navigate(Screen.Profile.route) }
            )
        }

        composable(
            route = Screen.DeviceDetail.route,
            arguments = listOf(
                navArgument("deviceId") { type = NavType.StringType },
                navArgument("serialNumber") { type = NavType.StringType },
                navArgument("isActive") { type = NavType.BoolType },
                navArgument("lastSeen") { type = NavType.StringType }
            )
        ) { backStackEntry ->
            val deviceId = backStackEntry.arguments?.getString("deviceId") ?: ""
            val serialNumber = backStackEntry.arguments?.getString("serialNumber") ?: ""
            val isActive = backStackEntry.arguments?.getBoolean("isActive") ?: false
            val lastSeen = backStackEntry.arguments?.getString("lastSeen").let {
                if (it == "null") null else it
            }
            val device = com.driverguard.data.model.Device(
                id = deviceId,
                serialNumber = serialNumber,
                isActive = isActive,
                lastSeenAt = lastSeen
            )
            val detailViewModel: DeviceDetailViewModel = viewModel()
            DeviceDetailScreen(
                device = device,
                viewModel = detailViewModel,
                onBack = { navController.popBackStack() }
            )
        }

        composable(Screen.Notifications.route) {
            val notificationsViewModel: NotificationsViewModel = viewModel()
            NotificationsScreen(
                viewModel = notificationsViewModel,
                onBack = { navController.popBackStack() }
            )
        }

        composable(Screen.Profile.route) {
            val profileViewModel = remember { ProfileViewModel(context) }
            ProfileScreen(
                viewModel = profileViewModel,
                onBack = { navController.popBackStack() },
                onLogout = {
                    authViewModel.logout()
                    navController.navigate(Screen.Login.route) {
                        popUpTo(0) { inclusive = true }
                    }
                }
            )
        }
    }
}
