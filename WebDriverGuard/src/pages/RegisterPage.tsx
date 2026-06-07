import { useState } from 'react';
import { Box, Button, Card, CardContent, TextField, Typography, Alert, Link as MuiLink, Stack } from '@mui/material';
import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { authApi } from '../api/client';

export default function RegisterPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirm, setConfirm] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (password !== confirm) { setError('Паролі не збігаються'); return; }
    setError('');
    setLoading(true);
    try {
      await authApi.register(email, password);
      navigate('/login');
    } catch {
      setError(t('common.error'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', bgcolor: 'grey.100' }}>
      <Card sx={{ width: 400, p: 2 }}>
        <CardContent>
          <Typography variant="h5" fontWeight={700} color="primary" textAlign="center" mb={3}>
            {t('app.name')}
          </Typography>
          <Typography variant="h6" textAlign="center" mb={3}>{t('auth.register')}</Typography>
          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
          <Box component="form" onSubmit={handleSubmit}>
            <Stack spacing={2}>
              <TextField label={t('auth.email')} type="email" value={email}
                onChange={(e) => setEmail(e.target.value)} required fullWidth />
              <TextField label={t('auth.password')} type="password" value={password}
                onChange={(e) => setPassword(e.target.value)} required fullWidth />
              <TextField label={t('auth.confirmPassword')} type="password" value={confirm}
                onChange={(e) => setConfirm(e.target.value)} required fullWidth />
              <Button type="submit" variant="contained" fullWidth disabled={loading} size="large">
                {t('auth.registerBtn')}
              </Button>
              <MuiLink component={Link} to="/login" variant="body2" textAlign="center">
                {t('auth.hasAccount')}
              </MuiLink>
            </Stack>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
