import { useEffect, useState } from 'react';
import {
  Alert, Box, Button, Chip, CircularProgress, Paper, Snackbar, Stack,
  Table, TableBody, TableCell, TableHead, TableRow, Tooltip, Typography,
} from '@mui/material';
import { Sync as SyncIcon } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { adminApi } from '../../api/client';
import type { Device } from '../../types';
import { formatDate } from '../../utils/date';

interface AdminDevice extends Device {
  userId?: string;
}

export default function AdminDevicesPage() {
  const { t, i18n } = useTranslation();
  const [devices, setDevices] = useState<AdminDevice[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [checkingOffline, setCheckingOffline] = useState(false);
  const [offlineChecked, setOfflineChecked] = useState(false);

  const load = async (silent = false) => {
    try {
      const res = await adminApi.getDevices();
      setDevices(res.data);
    } catch {
      if (!silent) setError(t('common.error'));
    } finally {
      if (!silent) setLoading(false);
    }
  };

  useEffect(() => {
    load(false);
    const id = setInterval(() => load(true), 5000);
    return () => clearInterval(id);
  }, []);

  const handleCheckOffline = async () => {
    setCheckingOffline(true);
    try {
      await adminApi.checkOffline();
      setOfflineChecked(true);
      await load(true);
    } catch {
      setError(t('common.error'));
    } finally {
      setCheckingOffline(false);
    }
  };

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;

  return (
    <Box>
      <Stack direction="row" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5" fontWeight={600}>{t('admin.devices.title')}</Typography>
        <Tooltip title={t('admin.devices.checkOfflineDesc')}>
          <Button
            variant="outlined"
            startIcon={<SyncIcon />}
            onClick={handleCheckOffline}
            disabled={checkingOffline}
          >
            {t('admin.devices.checkOffline')}
          </Button>
        </Tooltip>
      </Stack>

      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}

      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>{t('admin.devices.serialNumber')}</TableCell>
              <TableCell>{t('admin.devices.owner')}</TableCell>
              <TableCell>{t('admin.devices.status')}</TableCell>
              <TableCell>{t('admin.devices.lastSeen')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {devices.map((d) => (
              <TableRow key={d.id} hover>
                <TableCell>{d.serialNumber}</TableCell>
                <TableCell sx={{ fontFamily: 'monospace', fontSize: '0.75rem' }}>
                  {d.userId ?? '—'}
                </TableCell>
                <TableCell>
                  <Chip
                    label={d.isActive ? t('admin.devices.active') : t('admin.devices.inactive')}
                    color={d.isActive ? 'success' : 'default'} size="small"
                  />
                </TableCell>
                <TableCell>
                  {d.lastSeenAt ? formatDate(d.lastSeenAt, i18n.language) : '—'}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>

      <Snackbar open={offlineChecked} autoHideDuration={3000} onClose={() => setOfflineChecked(false)}
        message={t('admin.devices.checked')} />
    </Box>
  );
}
