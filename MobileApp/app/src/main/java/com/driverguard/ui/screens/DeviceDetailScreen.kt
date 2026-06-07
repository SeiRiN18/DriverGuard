package com.driverguard.ui.screens

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.filled.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp
import com.driverguard.data.model.Device
import com.driverguard.data.model.DriverEvent
import com.driverguard.viewmodel.DeviceDetailViewModel
import kotlin.math.roundToInt

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DeviceDetailScreen(
    device: Device,
    viewModel: DeviceDetailViewModel,
    onBack: () -> Unit
) {
    val liveDevice by viewModel.device.collectAsState()
    val currentDevice = liveDevice ?: device
    val events by viewModel.events.collectAsState()
    val config by viewModel.config.collectAsState()
    val isLoading by viewModel.isLoading.collectAsState()
    val error by viewModel.error.collectAsState()
    val configMessage by viewModel.configMessage.collectAsState()
    val deleted by viewModel.deleted.collectAsState()

    var showConfigDialog by remember { mutableStateOf(false) }
    var showDeleteDialog by remember { mutableStateOf(false) }

    LaunchedEffect(deleted) { if (deleted) onBack() }

    val snackbarHostState = remember { SnackbarHostState() }
    LaunchedEffect(configMessage) {
        configMessage?.let {
            snackbarHostState.showSnackbar(it)
            viewModel.clearConfigMessage()
        }
    }

    LaunchedEffect(device.id) { viewModel.load(device.id) }

    if (showDeleteDialog) {
        AlertDialog(
            onDismissRequest = { showDeleteDialog = false },
            title = { Text("Видалити пристрій?") },
            text = { Text("Пристрій \"${device.serialNumber}\" та всі його події будуть видалені. Цю дію неможливо скасувати.") },
            confirmButton = {
                TextButton(
                    onClick = {
                        showDeleteDialog = false
                        viewModel.deleteDevice(device.id)
                    }
                ) { Text("Видалити", color = MaterialTheme.colorScheme.error) }
            },
            dismissButton = { TextButton(onClick = { showDeleteDialog = false }) { Text("Скасувати") } }
        )
    }

    if (showConfigDialog && config != null) {
        ConfigurationDialog(
            drowsiness = config!!.drowsinessThreshold,
            attention = config!!.attentionThreshold,
            onConfirm = { d, a ->
                viewModel.updateConfiguration(device.id, d, a)
                showConfigDialog = false
            },
            onDismiss = { showConfigDialog = false }
        )
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(device.serialNumber, maxLines = 1) },
                navigationIcon = {
                    IconButton(onClick = onBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Назад")
                    }
                },
                actions = {
                    if (config != null) {
                        IconButton(onClick = { showConfigDialog = true }) {
                            Icon(Icons.Default.Tune, contentDescription = "Налаштування")
                        }
                    }
                    IconButton(onClick = { viewModel.load(device.id) }) {
                        Icon(Icons.Default.Refresh, contentDescription = "Оновити")
                    }
                    IconButton(onClick = { showDeleteDialog = true }) {
                        Icon(
                            Icons.Default.Delete,
                            contentDescription = "Видалити",
                            tint = MaterialTheme.colorScheme.error
                        )
                    }
                }
            )
        },
        snackbarHost = { SnackbarHost(snackbarHostState) }
    ) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
        ) {
            DeviceStatusCard(device = currentDevice, config = config, onConfigClick = { showConfigDialog = true })

            Box(modifier = Modifier.weight(1f)) {
                when {
                    isLoading -> CircularProgressIndicator(
                        modifier = Modifier.align(Alignment.Center)
                    )
                    error != null -> Text(
                        text = error!!,
                        color = MaterialTheme.colorScheme.error,
                        modifier = Modifier
                            .align(Alignment.Center)
                            .padding(16.dp)
                    )
                    events.isEmpty() -> Column(
                        modifier = Modifier.align(Alignment.Center),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Icon(
                            Icons.Default.EventNote,
                            contentDescription = null,
                            modifier = Modifier.size(64.dp),
                            tint = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.3f)
                        )
                        Spacer(Modifier.height(12.dp))
                        Text(
                            "Подій ще немає",
                            color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f)
                        )
                    }
                    else -> LazyColumn(
                        contentPadding = PaddingValues(horizontal = 16.dp, vertical = 8.dp),
                        verticalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        items(events) { event -> EventCard(event = event) }
                    }
                }
            }
        }
    }
}

@Composable
private fun DeviceStatusCard(
    device: Device,
    config: com.driverguard.data.model.DeviceConfiguration?,
    onConfigClick: () -> Unit
) {
    Card(
        modifier = Modifier
            .fillMaxWidth()
            .padding(16.dp),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface)
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.SpaceBetween
            ) {
                Column {
                    Text("Статус пристрою", style = MaterialTheme.typography.labelMedium)
                    Text(
                        if (device.isActive) "Активний" else "Неактивний",
                        color = if (device.isActive) MaterialTheme.colorScheme.primary
                                else MaterialTheme.colorScheme.error
                    )
                }
                Icon(
                    if (device.isActive) Icons.Default.Wifi else Icons.Default.WifiOff,
                    contentDescription = null,
                    tint = if (device.isActive) MaterialTheme.colorScheme.primary
                           else MaterialTheme.colorScheme.error
                )
            }

            if (config != null) {
                HorizontalDivider(modifier = Modifier.padding(vertical = 10.dp))
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically
                ) {
                    Column {
                        Text("Пороги виявлення", style = MaterialTheme.typography.labelMedium)
                        Spacer(Modifier.height(4.dp))
                        Text(
                            "Сонливість: ${(config.drowsinessThreshold * 100).roundToInt()}%  " +
                            "Увага: ${(config.attentionThreshold * 100).roundToInt()}%",
                            style = MaterialTheme.typography.bodySmall,
                            color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.7f)
                        )
                    }
                    IconButton(onClick = onConfigClick) {
                        Icon(
                            Icons.Default.Edit,
                            contentDescription = "Редагувати",
                            modifier = Modifier.size(18.dp)
                        )
                    }
                }
            }
        }
    }
}

@Composable
private fun ConfigurationDialog(
    drowsiness: Double,
    attention: Double,
    onConfirm: (Double, Double) -> Unit,
    onDismiss: () -> Unit
) {
    var drowsinessSlider by remember { mutableFloatStateOf(drowsiness.toFloat()) }
    var attentionSlider by remember { mutableFloatStateOf(attention.toFloat()) }

    AlertDialog(
        onDismissRequest = onDismiss,
        title = { Text("Налаштування порогів") },
        text = {
            Column {
                Text(
                    "Поріг сонливості: ${(drowsinessSlider * 100).roundToInt()}%",
                    style = MaterialTheme.typography.bodyMedium
                )
                Slider(
                    value = drowsinessSlider,
                    onValueChange = { drowsinessSlider = it },
                    valueRange = 0.3f..0.9f,
                    steps = 11
                )
                Spacer(Modifier.height(8.dp))
                Text(
                    "Поріг уваги: ${(attentionSlider * 100).roundToInt()}%",
                    style = MaterialTheme.typography.bodyMedium
                )
                Slider(
                    value = attentionSlider,
                    onValueChange = { attentionSlider = it },
                    valueRange = 0.3f..0.9f,
                    steps = 11
                )
                Spacer(Modifier.height(4.dp))
                Text(
                    "Нижчий поріг = більше сповіщень",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f)
                )
            }
        },
        confirmButton = {
            TextButton(onClick = {
                onConfirm(drowsinessSlider.toDouble(), attentionSlider.toDouble())
            }) { Text("Зберегти") }
        },
        dismissButton = { TextButton(onClick = onDismiss) { Text("Скасувати") } }
    )
}

@Composable
private fun EventCard(event: DriverEvent) {
    val (severityColor, severityLabel) = when (event.severity) {
        1 -> Pair(Color(0xFFFFF176), "Низький")
        2 -> Pair(Color(0xFFFFB74D), "Середній")
        3 -> Pair(Color(0xFFFFB74D), "Середній")
        4 -> Pair(Color(0xFFEF5350), "Високий")
        5 -> Pair(Color(0xFFEF5350), "Критичний")
        else -> Pair(MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f), "Невідомий")
    }

    Card(
        modifier = Modifier.fillMaxWidth(),
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp),
            verticalAlignment = Alignment.CenterVertically
        ) {
            Icon(
                imageVector = when (event.eventType.lowercase()) {
                    "drowsiness" -> Icons.Default.Bedtime
                    "attention_loss" -> Icons.Default.VisibilityOff
                    else -> Icons.Default.Warning
                },
                contentDescription = null,
                tint = severityColor,
                modifier = Modifier.size(36.dp)
            )
            Spacer(Modifier.width(12.dp))
            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = when (event.eventType.lowercase()) {
                        "drowsiness" -> "Сонливість"
                        "attention_loss" -> "Втрата уваги"
                        "normal" -> "Норма"
                        else -> event.eventType
                    },
                    style = MaterialTheme.typography.titleSmall
                )
                Text(
                    text = event.occurredAt.take(16).replace("T", " "),
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.6f)
                )
            }
            Column(horizontalAlignment = Alignment.End) {
                Text(
                    text = severityLabel,
                    style = MaterialTheme.typography.labelSmall,
                    color = severityColor
                )
                Text(
                    text = "${(event.confidence * 100).toInt()}%",
                    style = MaterialTheme.typography.bodySmall,
                    color = MaterialTheme.colorScheme.onSurface.copy(alpha = 0.5f)
                )
            }
        }
    }
}
