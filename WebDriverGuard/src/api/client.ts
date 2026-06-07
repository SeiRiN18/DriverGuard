import axios from 'axios';

const BASE_URL = import.meta.env.VITE_API_URL || 'https://driverguard-api-ynl5.onrender.com';

const api = axios.create({ baseURL: BASE_URL });

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (r) => r,
  (err) => {
    if (err.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(err);
  },
);

export const authApi = {
  login: (email: string, password: string) =>
    api.post<{ token: string; role: string }>('/api/auth/login', { email, password }),
  register: (email: string, password: string) =>
    api.post('/api/auth/register', { email, password }),
  resetPassword: (email: string, newPassword: string) =>
    api.post('/api/auth/reset-password', { email, newPassword }),
};

export const devicesApi = {
  getMy: () => api.get('/api/devices/my'),
  getById: (id: string) => api.get(`/api/devices/${id}`),
  create: (serialNumber: string) => api.post('/api/devices', { serialNumber }),
  delete: (id: string) => api.delete(`/api/devices/${id}`),
  getConfiguration: (id: string) => api.get(`/api/devices/${id}/configuration`),
  updateConfiguration: (id: string, drowsinessThreshold: number, attentionThreshold: number) =>
    api.put(`/api/devices/${id}/configuration`, { drowsinessThreshold, attentionThreshold }),
};

export const eventsApi = {
  getByDevice: (deviceId: string) => api.get(`/api/events/device/${deviceId}`),
};

export const notificationsApi = {
  getMy: () => api.get('/api/notifications/my'),
  markRead: (id: string) => api.put(`/api/notifications/${id}/read`),
};

export const usersApi = {
  getById: (id: string) => api.get(`/api/users/${id}`),
  update: (id: string, email: string, password: string) =>
    api.put(`/api/users/${id}`, { email, password }),
  delete: (id: string) => api.delete(`/api/users/${id}`),
};

export const adminApi = {
  getUsers: () => api.get('/api/admin/users'),
  getDevices: () => api.get('/api/admin/devices'),
  getStats: () => api.get('/api/admin/stats'),
  checkOffline: () => api.post('/api/admin/devices/check-offline'),
};

export default api;
