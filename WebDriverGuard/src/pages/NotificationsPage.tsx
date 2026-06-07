import { useEffect, useState } from 'react';
import {
  Alert, Badge, Box, Button, Card, CardContent,
  CircularProgress, Divider, Stack, Typography,
} from '@mui/material';
import { Notifications as NotifIcon } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { notificationsApi } from '../api/client';
import type { AppNotification } from '../types';
import { formatDate } from '../utils/date';

export default function NotificationsPage() {
  const { t, i18n } = useTranslation();
  const [notifications, setNotifications] = useState<AppNotification[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const load = async () => {
    try {
      const res = await notificationsApi.getMy();
      setNotifications(res.data);
    } catch {
      setError(t('common.error'));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const markRead = async (id: string) => {
    try {
      await notificationsApi.markRead(id);
      setNotifications((prev) => prev.map((n) => n.id === id ? { ...n, isRead: true } : n));
    } catch { /* ignore */ }
  };

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;

  const unread = notifications.filter((n) => !n.isRead).length;

  return (
    <Box>
      <Stack direction="row" alignItems="center" spacing={2} mb={3}>
        <Typography variant="h5" fontWeight={600}>{t('notifications.title')}</Typography>
        {unread > 0 && <Badge badgeContent={unread} color="error"><NotifIcon /></Badge>}
      </Stack>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {notifications.length === 0
        ? <Typography color="text.secondary" textAlign="center" mt={8}>{t('notifications.noNotifications')}</Typography>
        : <Stack spacing={2}>
            {notifications.map((n) => (
              <Card key={n.id} sx={{ opacity: n.isRead ? 0.6 : 1, borderLeft: n.isRead ? 'none' : '4px solid #1565C0' }}>
                <CardContent>
                  <Stack direction="row" justifyContent="space-between" alignItems="flex-start">
                    <Box flex={1}>
                      <Typography fontWeight={n.isRead ? 400 : 600}>{n.title}</Typography>
                      <Typography variant="body2" color="text.secondary" mt={0.5}>{n.message}</Typography>
                      <Typography variant="caption" color="text.secondary" mt={1} display="block">
                        {formatDate(n.createdAt, i18n.language)}
                      </Typography>
                    </Box>
                    {!n.isRead && (
                      <Button size="small" onClick={() => markRead(n.id)} sx={{ ml: 2, whiteSpace: 'nowrap' }}>
                        {t('notifications.markRead')}
                      </Button>
                    )}
                  </Stack>
                </CardContent>
              </Card>
            ))}
          </Stack>
      }
    </Box>
  );
}
