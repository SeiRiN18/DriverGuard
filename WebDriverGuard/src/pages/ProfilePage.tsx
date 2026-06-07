import { useEffect, useState } from 'react';
import { Alert, Box, Button, Card, CardContent, CircularProgress, Stack, TextField, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usersApi } from '../api/client';
import { useAuth } from '../context/AuthContext';
import { formatDate } from '../utils/date';
import type { User } from '../types';

export default function ProfilePage() {
  const { t, i18n } = useTranslation();
  const { user } = useAuth();
  const [profile, setProfile] = useState<User | null>(null);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!user?.id) return;
    usersApi.getById(user.id)
      .then((res) => { setProfile(res.data); setEmail(res.data.email); })
      .catch(() => setError(t('common.error')))
      .finally(() => setLoading(false));
  }, [user?.id]);

  const handleSave = async () => {
    if (!user?.id) return;
    setSaving(true);
    try {
      await usersApi.update(user.id, email, password || profile?.email || '');
      setSuccess(true);
      setPassword('');
      setTimeout(() => setSuccess(false), 3000);
    } catch {
      setError(t('common.error'));
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;

  return (
    <Box maxWidth={500}>
      <Typography variant="h5" fontWeight={600} mb={3}>{t('profile.title')}</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }}>{t('profile.saved')}</Alert>}
      <Card>
        <CardContent>
          {profile && (
            <Typography variant="body2" color="text.secondary" mb={2}>
              {t('profile.memberSince')}: {formatDate(profile.createdAt, i18n.language)}
            </Typography>
          )}
          <Stack spacing={2}>
            <TextField label={t('profile.email')} type="email" value={email}
              onChange={(e) => setEmail(e.target.value)} fullWidth />
            <TextField label={t('profile.password')} type="password" value={password}
              onChange={(e) => setPassword(e.target.value)} fullWidth
              helperText="Залиште порожнім, щоб не змінювати" />
            <Button variant="contained" onClick={handleSave} disabled={saving}>
              {t('profile.save')}
            </Button>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  );
}
