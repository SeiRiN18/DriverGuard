import { useState } from 'react';
import { Box, Button, Card, CardContent, TextField, Typography, Alert, Link as MuiLink, Stack } from '@mui/material';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { authApi } from '../api/client';

export default function ResetPasswordPage() {
  const { t } = useTranslation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await authApi.resetPassword(email, password);
      setSuccess(true);
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
          <Typography variant="h6" textAlign="center" mb={3}>{t('auth.resetPassword')}</Typography>
          {success
            ? <Alert severity="success">Пароль успішно змінено. <MuiLink component={Link} to="/login">Увійти</MuiLink></Alert>
            : <>
                {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
                <Box component="form" onSubmit={handleSubmit}>
                  <Stack spacing={2}>
                    <TextField label={t('auth.email')} type="email" value={email}
                      onChange={(e) => setEmail(e.target.value)} required fullWidth />
                    <TextField label={t('auth.newPassword')} type="password" value={password}
                      onChange={(e) => setPassword(e.target.value)} required fullWidth />
                    <Button type="submit" variant="contained" fullWidth disabled={loading}>
                      {t('auth.resetBtn')}
                    </Button>
                    <MuiLink component={Link} to="/login" variant="body2" textAlign="center">
                      {t('auth.hasAccount')}
                    </MuiLink>
                  </Stack>
                </Box>
              </>
          }
        </CardContent>
      </Card>
    </Box>
  );
}
