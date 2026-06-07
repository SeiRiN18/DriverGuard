import { useEffect, useState } from 'react';
import {
  Alert, Box, Chip, CircularProgress, Paper, Table, TableBody,
  TableCell, TableHead, TableRow, Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { eventsApi } from '../../api/client';
import type { DriverEvent } from '../../types';
import { formatDate } from '../../utils/date';

const SEVERITY_COLORS: Record<number, 'default' | 'warning' | 'error'> = {
  1: 'default', 2: 'default', 3: 'warning', 4: 'error', 5: 'error',
};

export default function AdminEventsPage() {
  const { t, i18n } = useTranslation();
  const [events, setEvents] = useState<DriverEvent[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    eventsApi.getAll()
      .then((res) => setEvents(res.data))
      .catch(() => setError(t('common.error')))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;

  return (
    <Box>
      <Typography variant="h5" fontWeight={600} mb={3}>{t('admin.events.title')}</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>{t('admin.events.deviceId')}</TableCell>
              <TableCell>{t('admin.events.type')}</TableCell>
              <TableCell>{t('admin.events.severity')}</TableCell>
              <TableCell>{t('admin.events.confidence')}</TableCell>
              <TableCell>{t('admin.events.occurredAt')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {events.map((e) => (
              <TableRow key={e.id} hover>
                <TableCell sx={{ fontFamily: 'monospace', fontSize: '0.75rem' }}>
                  {e.deviceId}
                </TableCell>
                <TableCell>
                  {t(`device.eventType.${e.eventType}`, { defaultValue: e.eventType })}
                </TableCell>
                <TableCell>
                  <Chip
                    label={t(`device.severityLabel.${e.severity}`, { defaultValue: String(e.severity) })}
                    color={SEVERITY_COLORS[e.severity] ?? 'default'}
                    size="small"
                  />
                </TableCell>
                <TableCell>{(e.confidence * 100).toFixed(1)}%</TableCell>
                <TableCell>{formatDate(e.occurredAt, i18n.language)}</TableCell>
              </TableRow>
            ))}
            {events.length === 0 && (
              <TableRow>
                <TableCell colSpan={5} align="center" sx={{ py: 4, color: 'text.secondary' }}>
                  {t('admin.events.noEvents')}
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </Paper>
    </Box>
  );
}
