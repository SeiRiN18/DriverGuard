import { useState } from 'react';
import { Box, Button, Card, CardContent, TextField, Typography, Alert, Link as MuiLink, Stack } from '@mui/material';
import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { authApi } from '../api/client';
import { useAuth } from '../context/AuthContext';

export default function LoginPage() {
  const { t } = useTranslation();
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await authApi.login(email, password);
      login(res.data.token);
      navigate(res.data.role === 'Admin' ? '/admin' : '/dashboard');
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
          <Typography variant="h6" textAlign="center" mb={3}>{t('auth.login')}</Typography>
          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
          <Box component="form" onSubmit={handleSubmit}>
            <Stack spacing={2}>
              <TextField label={t('auth.email')} type="email" value={email}
                onChange={(e) => setEmail(e.target.value)} required fullWidth />
              <TextField label={t('auth.password')} type="password" value={password}
                onChange={(e) => setPassword(e.target.value)} required fullWidth />
              <Button type="submit" variant="contained" fullWidth disabled={loading} size="large">
                {t('auth.loginBtn')}
              </Button>
              <Stack direction="row" justifyContent="space-between">
                <MuiLink component={Link} to="/register" variant="body2">
                  {t('auth.noAccount')}
                </MuiLink>
                <MuiLink component={Link} to="/reset-password" variant="body2">
                  {t('auth.forgotPassword')}
                </MuiLink>
              </Stack>
            </Stack>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
