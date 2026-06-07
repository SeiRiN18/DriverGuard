import { useRef, useState } from 'react';
import {
  Alert, Box, Button, Card, CardContent, Divider, Snackbar,
  Stack, Typography,
} from '@mui/material';
import {
  Download as DownloadIcon, Upload as UploadIcon, Backup as BackupIcon,
} from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { adminApi, devicesApi } from '../../api/client';

function downloadJson(data: unknown, filename: string) {
  const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  a.click();
  URL.revokeObjectURL(url);
}

export default function AdminExportPage() {
  const { t } = useTranslation();
  const fileRef = useRef<HTMLInputElement>(null);
  const [importSuccess, setImportSuccess] = useState(false);
  const [error, setError] = useState('');

  const exportUsers = async () => {
    try {
      const res = await adminApi.getUsers();
      downloadJson(res.data, `driverguard-users-${Date.now()}.json`);
    } catch { setError(t('common.error')); }
  };

  const exportDevices = async () => {
    try {
      const res = await adminApi.getDevices();
      downloadJson(res.data, `driverguard-devices-${Date.now()}.json`);
    } catch { setError(t('common.error')); }
  };

  const backupSettings = async () => {
    try {
      const devRes = await adminApi.getDevices();
      const configs = await Promise.allSettled(
        devRes.data.map((d: { id: string }) => devicesApi.getConfiguration(d.id))
      );
      const result = configs
        .filter((c) => c.status === 'fulfilled')
        .map((c) => (c as PromiseFulfilledResult<{ data: unknown }>).data.data);
      downloadJson({ exportedAt: new Date().toISOString(), configurations: result },
        `driverguard-backup-${Date.now()}.json`);
    } catch { setError(t('common.error')); }
  };

  const handleImport = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = (ev) => {
      try {
        const data = JSON.parse(ev.target?.result as string);
        console.info('Imported data:', data);
        setImportSuccess(true);
      } catch {
        setError('Невалідний JSON файл');
      }
    };
    reader.readAsText(file);
    e.target.value = '';
  };

  return (
    <Box maxWidth={700}>
      <Typography variant="h5" fontWeight={600} mb={3}>{t('admin.export.title')}</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}

      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" mb={2}>{t('admin.export.exportSection')}</Typography>
          <Stack spacing={2}>
            <Button variant="outlined" startIcon={<DownloadIcon />} onClick={exportUsers}>
              {t('admin.export.exportUsers')}
            </Button>
            <Button variant="outlined" startIcon={<DownloadIcon />} onClick={exportDevices}>
              {t('admin.export.exportDevices')}
            </Button>
          </Stack>
        </CardContent>
      </Card>

      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" mb={1}>{t('admin.export.backupSection')}</Typography>
          <Typography variant="body2" color="text.secondary" mb={2}>
            {t('admin.export.backupDesc')}
          </Typography>
          <Button variant="outlined" startIcon={<BackupIcon />} onClick={backupSettings}>
            {t('admin.export.backupSettings')}
          </Button>
        </CardContent>
      </Card>

      <Card>
        <CardContent>
          <Typography variant="h6" mb={2}>{t('admin.export.importSection')}</Typography>
          <input ref={fileRef} type="file" accept=".json" hidden onChange={handleImport} />
          <Button variant="outlined" startIcon={<UploadIcon />} onClick={() => fileRef.current?.click()}>
            {t('admin.export.importBtn')}
          </Button>
        </CardContent>
      </Card>

      <Snackbar open={importSuccess} autoHideDuration={3000} onClose={() => setImportSuccess(false)}
        message={t('admin.export.importSuccess')} />
    </Box>
  );
}
