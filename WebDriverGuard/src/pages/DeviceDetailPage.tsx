import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Alert, Box, Button, Card, CardContent, Chip, CircularProgress,
  Dialog, DialogActions, DialogContent, DialogTitle, Divider,
  IconButton, Slider, Stack, Table, TableBody, TableCell,
  TableHead, TableRow, Typography,
} from '@mui/material';
import { ArrowBack, Delete, Refresh, Wifi, WifiOff } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { devicesApi, eventsApi } from '../api/client';
import type { Device, DeviceConfiguration, DriverEvent } from '../types';
import { formatDate } from '../utils/date';

const SEVERITY_COLOR: Record<number, string> = { 1: '#FFF176', 2: '#FFB74D', 3: '#FFB74D', 4: '#EF5350', 5: '#B71C1C' };

export default function DeviceDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();

  const [device, setDevice] = useState<Device | null>(null);
  const [events, setEvents] = useState<DriverEvent[]>([]);
  const [config, setConfig] = useState<DeviceConfiguration | null>(null);
  const [loading, setLoading] = useState(true);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [configOpen, setConfigOpen] = useState(false);
  const [drowsiness, setDrowsiness] = useState(0.5);
  const [attention, setAttention] = useState(0.5);
  const [saved, setSaved] = useState(false);
  const [error, setError] = useState('');

  const load = async () => {
    if (!id) return;
    try {
      const [dRes, eRes, cRes] = await Promise.all([
        devicesApi.getById(id),
        eventsApi.getByDevice(id),
        devicesApi.getConfiguration(id),
      ]);
      setDevice(dRes.data);
      setEvents(eRes.data);
      setConfig(cRes.data);
      setDrowsiness(cRes.data.drowsinessThreshold);
      setAttention(cRes.data.attentionThreshold);
    } catch {
      setError(t('common.error'));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, [id]);

  const handleDelete = async () => {
    if (!id) return;
    try { await devicesApi.delete(id); navigate('/dashboard'); }
    catch { setError(t('common.error')); }
  };

  const handleSaveConfig = async () => {
    if (!id) return;
    try {
      await devicesApi.updateConfiguration(id, drowsiness, attention);
      setSaved(true);
      setConfigOpen(false);
      setTimeout(() => setSaved(false), 3000);
    } catch { setError(t('common.error')); }
  };

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;

  return (
    <Box>
      <Stack direction="row" alignItems="center" spacing={1} mb={3}>
        <IconButton onClick={() => navigate('/dashboard')}><ArrowBack /></IconButton>
        <Typography variant="h5" fontWeight={600}>{device?.serialNumber}</Typography>
        <Box flex={1} />
        <IconButton onClick={load}><Refresh /></IconButton>
        <IconButton onClick={() => setDeleteOpen(true)} color="error"><Delete /></IconButton>
      </Stack>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {saved && <Alert severity="success" sx={{ mb: 2 }}>{t('profile.saved')}</Alert>}

      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Stack direction="row" justifyContent="space-between" alignItems="center">
            <Box>
              <Typography variant="overline">{t('device.status')}</Typography>
              <Stack direction="row" alignItems="center" spacing={1}>
                {device?.isActive ? <Wifi color="primary" /> : <WifiOff color="error" />}
                <Chip
                  label={device?.isActive ? t('dashboard.active') : t('dashboard.inactive')}
                  color={device?.isActive ? 'success' : 'error'} size="small"
                />
              </Stack>
            </Box>
            {config && (
              <Box textAlign="right">
                <Typography variant="overline">{t('device.configuration')}</Typography>
                <Typography variant="body2">
                  {t('device.drowsinessThreshold')}: {Math.round(config.drowsinessThreshold * 100)}%
                </Typography>
                <Typography variant="body2">
                  {t('device.attentionThreshold')}: {Math.round(config.attentionThreshold * 100)}%
                </Typography>
                <Button size="small" onClick={() => setConfigOpen(true)} sx={{ mt: 0.5 }}>
                  {t('device.save')}
                </Button>
              </Box>
            )}
          </Stack>
        </CardContent>
      </Card>

      <Typography variant="h6" mb={2}>{t('device.events')}</Typography>
      {events.length === 0
        ? <Typography color="text.secondary">{t('device.noEvents')}</Typography>
        : <Card>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>{t('device.eventType.drowsiness').split('/')[0]}</TableCell>
                  <TableCell>{t('device.severity')}</TableCell>
                  <TableCell>{t('device.confidence')}</TableCell>
                  <TableCell>Час</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {events.map((ev) => (
                  <TableRow key={ev.id}>
                    <TableCell>
                      {ev.eventType === 'drowsiness' ? t('device.eventType.drowsiness')
                        : ev.eventType === 'attention_loss' ? t('device.eventType.attention_loss')
                        : t('device.eventType.normal')}
                    </TableCell>
                    <TableCell>
                      <Chip label={t(`device.severityLabel.${ev.severity}`) || ev.severity}
                        size="small" sx={{ bgcolor: SEVERITY_COLOR[ev.severity] || '#ccc', color: '#000' }} />
                    </TableCell>
                    <TableCell>{Math.round(ev.confidence * 100)}%</TableCell>
                    <TableCell>{formatDate(ev.occurredAt, i18n.language)}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </Card>
      }

      <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
        <DialogTitle>{t('device.delete')}</DialogTitle>
        <DialogContent><Typography>{t('device.deleteConfirm')}</Typography></DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteOpen(false)}>{t('common.cancel')}</Button>
          <Button color="error" variant="contained" onClick={handleDelete}>{t('common.confirm')}</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={configOpen} onClose={() => setConfigOpen(false)} fullWidth>
        <DialogTitle>{t('device.configuration')}</DialogTitle>
        <DialogContent>
          <Box mt={2}>
            <Typography>{t('device.drowsinessThreshold')}: {Math.round(drowsiness * 100)}%</Typography>
            <Slider value={drowsiness} min={0.3} max={0.9} step={0.05}
              onChange={(_, v) => setDrowsiness(v as number)} />
            <Divider sx={{ my: 2 }} />
            <Typography>{t('device.attentionThreshold')}: {Math.round(attention * 100)}%</Typography>
            <Slider value={attention} min={0.3} max={0.9} step={0.05}
              onChange={(_, v) => setAttention(v as number)} />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfigOpen(false)}>{t('common.cancel')}</Button>
          <Button variant="contained" onClick={handleSaveConfig}>{t('device.save')}</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
