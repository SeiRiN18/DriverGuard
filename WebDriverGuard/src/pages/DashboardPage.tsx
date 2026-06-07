import { useEffect, useState } from 'react';
import {
  Box, Button, Card, CardActionArea, CardContent, Chip, CircularProgress,
  Dialog, DialogActions, DialogContent, DialogTitle, Grid, IconButton,
  Snackbar, Stack, TextField, Typography, Alert,
} from '@mui/material';
import { Add as AddIcon, Router as RouterIcon, ContentCopy as CopyIcon } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { devicesApi } from '../api/client';
import type { Device, CreateDeviceResponse } from '../types';
import { formatDate } from '../utils/date';

export default function DashboardPage() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const [devices, setDevices] = useState<Device[]>([]);
  const [loading, setLoading] = useState(true);
  const [addOpen, setAddOpen] = useState(false);
  const [serial, setSerial] = useState('');
  const [newDevice, setNewDevice] = useState<CreateDeviceResponse | null>(null);
  const [copied, setCopied] = useState(false);
  const [error, setError] = useState('');

  const load = async () => {
    try {
      const res = await devicesApi.getMy();
      setDevices(res.data);
    } catch {
      setError(t('common.error'));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const handleAdd = async () => {
    if (!serial.trim()) return;
    try {
      const res = await devicesApi.create(serial.trim());
      setNewDevice(res.data);
      setSerial('');
      setAddOpen(false);
      load();
    } catch {
      setError(t('common.error'));
    }
  };

  const copyKey = () => {
    if (newDevice) { navigator.clipboard.writeText(newDevice.apiKey); setCopied(true); }
  };

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;

  return (
    <Box>
      <Stack direction="row" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5" fontWeight={600}>{t('dashboard.title')}</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setAddOpen(true)}>
          {t('dashboard.addDevice')}
        </Button>
      </Stack>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {devices.length === 0
        ? <Typography color="text.secondary" textAlign="center" mt={8}>{t('dashboard.noDevices')}</Typography>
        : <Grid container spacing={2}>
            {devices.map((d) => (
              <Grid item xs={12} sm={6} md={4} key={d.id}>
                <Card>
                  <CardActionArea onClick={() => navigate(`/devices/${d.id}`)}>
                    <CardContent>
                      <Stack direction="row" spacing={2} alignItems="center">
                        <RouterIcon color={d.isActive ? 'primary' : 'disabled'} sx={{ fontSize: 40 }} />
                        <Box flex={1}>
                          <Typography fontWeight={600}>{d.serialNumber}</Typography>
                          <Chip
                            label={d.isActive ? t('dashboard.active') : t('dashboard.inactive')}
                            color={d.isActive ? 'success' : 'default'}
                            size="small" sx={{ mt: 0.5 }}
                          />
                          {d.lastSeenAt && (
                            <Typography variant="caption" display="block" color="text.secondary" mt={0.5}>
                              {t('dashboard.lastSeen')}: {formatDate(d.lastSeenAt, i18n.language)}
                            </Typography>
                          )}
                        </Box>
                      </Stack>
                    </CardContent>
                  </CardActionArea>
                </Card>
              </Grid>
            ))}
          </Grid>
      }

      <Dialog open={addOpen} onClose={() => setAddOpen(false)}>
        <DialogTitle>{t('dashboard.addDevice')}</DialogTitle>
        <DialogContent>
          <TextField label={t('dashboard.serialNumber')} value={serial}
            onChange={(e) => setSerial(e.target.value)} fullWidth sx={{ mt: 1 }} />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAddOpen(false)}>{t('common.cancel')}</Button>
          <Button variant="contained" onClick={handleAdd}>{t('common.confirm')}</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={!!newDevice} onClose={() => setNewDevice(null)}>
        <DialogTitle>{t('dashboard.addDevice')}</DialogTitle>
        <DialogContent>
          <Alert severity="warning" sx={{ mb: 2 }}>{t('dashboard.newDeviceKey')}</Alert>
          <Stack direction="row" alignItems="center" spacing={1}
            sx={{ bgcolor: 'grey.100', p: 1.5, borderRadius: 1 }}>
            <Typography variant="body2" sx={{ flex: 1, wordBreak: 'break-all', fontFamily: 'monospace' }}>
              {newDevice?.apiKey}
            </Typography>
            <IconButton onClick={copyKey} size="small">
              <CopyIcon fontSize="small" />
            </IconButton>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button variant="contained" onClick={() => setNewDevice(null)}>{t('common.confirm')}</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={copied} autoHideDuration={2000} onClose={() => setCopied(false)}
        message={t('dashboard.copied')} />
    </Box>
  );
}
