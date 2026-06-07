import { useEffect, useState } from 'react';
import {
  Alert, Box, Chip, CircularProgress, Paper, Table, TableBody,
  TableCell, TableHead, TableRow, Typography,
} from '@mui/material';
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

  useEffect(() => {
    adminApi.getDevices()
      .then((res) => setDevices(res.data))
      .catch(() => setError(t('common.error')))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;

  return (
    <Box>
      <Typography variant="h5" fontWeight={600} mb={3}>{t('admin.devices.title')}</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

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
    </Box>
  );
}
