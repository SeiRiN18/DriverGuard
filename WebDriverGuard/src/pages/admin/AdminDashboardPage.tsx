import { useEffect, useState } from 'react';
import { Alert, Box, Button, Card, CardContent, CircularProgress, Grid, Snackbar, Typography } from '@mui/material';
import { People, Router, SignalWifi4Bar, EventNote } from '@mui/icons-material';
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
        <Box sx={{ color: 'primary.main', opacity: 0.7 }}>{icon}</Box>
      </Box>
    </CardContent>
  </Card>
);

export default function AdminDashboardPage() {
  const { t } = useTranslation();
  const [stats, setStats] = useState<AdminStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [checked, setChecked] = useState(false);

  useEffect(() => {
    adminApi.getStats()
      .then((res) => setStats(res.data))
      .catch(() => setError(t('common.error')))
      .finally(() => setLoading(false));
  }, []);

  const checkOffline = async () => {
    try { await adminApi.checkOffline(); setChecked(true); }
    catch { setError(t('common.error')); }
  };

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5" fontWeight={600}>{t('admin.stats.title')}</Typography>
        <Button variant="outlined" onClick={checkOffline}>{t('admin.stats.checkOffline')}</Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {stats && (
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard label={t('admin.stats.totalUsers')} value={stats.totalUsers}
              icon={<People sx={{ fontSize: 48 }} />} />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard label={t('admin.stats.totalDevices')} value={stats.totalDevices}
              icon={<Router sx={{ fontSize: 48 }} />} />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard label={t('admin.stats.activeDevices')} value={stats.activeDevices}
              icon={<SignalWifi4Bar sx={{ fontSize: 48 }} />} />
          </Grid>
          <Grid item xs={12} sm={6} md={3}>
            <StatCard label={t('admin.stats.totalEvents')} value={stats.totalEvents}
              icon={<EventNote sx={{ fontSize: 48 }} />} />
          </Grid>
        </Grid>
      )}

      <Snackbar open={checked} autoHideDuration={3000} onClose={() => setChecked(false)}
        message={t('admin.stats.checked')} />
    </Box>
  );
}
