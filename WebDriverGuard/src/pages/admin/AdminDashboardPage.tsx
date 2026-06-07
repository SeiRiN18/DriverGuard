import { useEffect, useState } from 'react';
import { Alert, Box, Card, CardContent, CircularProgress, Grid, Typography } from '@mui/material';
import { People, Router, EventNote, Notifications, Warning } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { adminApi } from '../../api/client';
import type { AdminStats } from '../../types';

const StatCard = ({ label, value, icon }: { label: string; value: number; icon: React.ReactNode }) => (
  <Card>
    <CardContent>
      <Box display="flex" justifyContent="space-between" alignItems="center">
        <Box>
          <Typography variant="h4" fontWeight={700}>{value}</Typography>
          <Typography variant="body2" color="text.secondary">{label}</Typography>
        </Box>
        <Box sx={{ color: 'primary.main', opacity: 0.6 }}>{icon}</Box>
      </Box>
    </CardContent>
  </Card>
);

export default function AdminDashboardPage() {
  const { t } = useTranslation();
  const [stats, setStats] = useState<AdminStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    adminApi.getStats()
      .then((res) => setStats(res.data))
      .catch(() => setError(t('common.error')))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;

  return (
    <Box>
      <Typography variant="h5" fontWeight={600} mb={3}>{t('admin.stats.title')}</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {stats && (
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6} md={4}>
            <StatCard label={t('admin.stats.totalUsers')} value={stats.users}
              icon={<People sx={{ fontSize: 48 }} />} />
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <StatCard label={t('admin.stats.totalDevices')} value={stats.devices}
              icon={<Router sx={{ fontSize: 48 }} />} />
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <StatCard label={t('admin.stats.totalEvents')} value={stats.events}
              icon={<EventNote sx={{ fontSize: 48 }} />} />
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <StatCard label={t('admin.stats.criticalEvents')} value={stats.criticalEvents}
              icon={<Warning sx={{ fontSize: 48 }} />} />
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <StatCard label={t('admin.stats.totalNotifications')} value={stats.notifications}
              icon={<Notifications sx={{ fontSize: 48 }} />} />
          </Grid>
          <Grid item xs={12} sm={6} md={4}>
            <StatCard label={t('admin.stats.unreadNotifications')} value={stats.unreadNotifications}
              icon={<Notifications sx={{ fontSize: 48 }} color="error" />} />
          </Grid>
        </Grid>
      )}
    </Box>
  );
}
