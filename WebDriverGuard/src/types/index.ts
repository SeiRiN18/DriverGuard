export interface LoginResponse {
  token: string;
  role: string;
}

export interface User {
  id: string;
  email: string;
  createdAt: string;
}

export interface Device {
  id: string;
  serialNumber: string;
  isActive: boolean;
  lastSeenAt: string | null;
}

export interface CreateDeviceResponse {
  id: string;
  serialNumber: string;
  apiKey: string;
}

export interface DeviceConfiguration {
  deviceId: string;
  drowsinessThreshold: number;
  attentionThreshold: number;
  updatedAt: string;
}

export interface DriverEvent {
  id: string;
  eventType: string;
  severity: number;
  confidence: number;
  occurredAt: string;
}

export interface AppNotification {
  id: string;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}

export interface AdminStats {
  users: number;
  devices: number;
  events: number;
  criticalEvents: number;
  notifications: number;
  unreadNotifications: number;
}

export interface AuthUser {
  id: string;
  email: string;
  role: string;
}
