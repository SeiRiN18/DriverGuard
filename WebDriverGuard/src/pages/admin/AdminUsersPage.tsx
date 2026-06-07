import { useEffect, useState } from 'react';
import {
  Alert, Box, Button, CircularProgress, Dialog, DialogActions,
  DialogContent, DialogTitle, IconButton, Stack, Table, TableBody,
  TableCell, TableHead, TableRow, TextField, Typography, Paper,
} from '@mui/material';
import { Delete, Edit } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { adminApi, usersApi } from '../../api/client';
import type { User } from '../../types';
import { formatDate } from '../../utils/date';

export default function AdminUsersPage() {
  const { t, i18n } = useTranslation();
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [editUser, setEditUser] = useState<User | null>(null);
  const [editEmail, setEditEmail] = useState('');
  const [editPassword, setEditPassword] = useState('');
  const [deleteId, setDeleteId] = useState<string | null>(null);

  const load = async () => {
    try {
      const res = await adminApi.getUsers();
      setUsers(res.data);
    } catch {
      setError(t('common.error'));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const openEdit = (u: User) => { setEditUser(u); setEditEmail(u.email); setEditPassword(''); };

  const handleEdit = async () => {
    if (!editUser) return;
    try {
      await usersApi.update(editUser.id, editEmail, editPassword || editEmail);
      setEditUser(null);
      load();
    } catch { setError(t('common.error')); }
  };

  const handleDelete = async () => {
    if (!deleteId) return;
    try {
      await usersApi.delete(deleteId);
      setDeleteId(null);
      load();
    } catch { setError(t('common.error')); }
  };

  if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;

  return (
    <Box>
      <Typography variant="h5" fontWeight={600} mb={3}>{t('admin.users.title')}</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>{t('admin.users.email')}</TableCell>
              <TableCell>{t('admin.users.createdAt')}</TableCell>
              <TableCell align="right">{t('admin.users.actions')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.map((u) => (
              <TableRow key={u.id} hover>
                <TableCell>{u.email}</TableCell>
                <TableCell>{formatDate(u.createdAt, i18n.language)}</TableCell>
                <TableCell align="right">
                  <Stack direction="row" justifyContent="flex-end" spacing={1}>
                    <IconButton size="small" onClick={() => openEdit(u)}><Edit fontSize="small" /></IconButton>
                    <IconButton size="small" color="error" onClick={() => setDeleteId(u.id)}><Delete fontSize="small" /></IconButton>
                  </Stack>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>

      <Dialog open={!!editUser} onClose={() => setEditUser(null)} fullWidth>
        <DialogTitle>{t('admin.users.editTitle')}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} mt={1}>
            <TextField label={t('admin.users.newEmail')} type="email" value={editEmail}
              onChange={(e) => setEditEmail(e.target.value)} fullWidth />
            <TextField label={t('admin.users.newPassword')} type="password" value={editPassword}
              onChange={(e) => setEditPassword(e.target.value)} fullWidth />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setEditUser(null)}>{t('common.cancel')}</Button>
          <Button variant="contained" onClick={handleEdit}>{t('admin.users.save')}</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={!!deleteId} onClose={() => setDeleteId(null)}>
        <DialogTitle>{t('admin.users.delete')}</DialogTitle>
        <DialogContent><Typography>{t('admin.users.deleteConfirm')}</Typography></DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteId(null)}>{t('common.cancel')}</Button>
          <Button color="error" variant="contained" onClick={handleDelete}>{t('common.confirm')}</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
