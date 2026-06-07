import {
  AppBar, Box, CssBaseline, Divider, Drawer, IconButton,
  List, ListItemButton, ListItemIcon, ListItemText, Toolbar,
  Typography, Button, Tooltip,
} from '@mui/material';
import {
  Menu as MenuIcon, Router as RouterIcon, Notifications as NotifIcon,
  Person as PersonIcon, Dashboard as DashIcon, People as PeopleIcon,
  ImportExport as ExportIcon, Logout as LogoutIcon, Language as LangIcon,
  EventNote as EventNoteIcon,
} from '@mui/icons-material';
import { useState } from 'react';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../context/AuthContext';

const DRAWER_WIDTH = 240;

export default function Layout() {
  const { t, i18n } = useTranslation();
  const { logout, isAdmin } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [mobileOpen, setMobileOpen] = useState(false);

  const toggleLang = () => i18n.changeLanguage(i18n.language === 'uk' ? 'en' : 'uk');

  const userLinks = [
    { label: t('nav.dashboard'), icon: <RouterIcon />, path: '/dashboard' },
    { label: t('nav.notifications'), icon: <NotifIcon />, path: '/notifications' },
    { label: t('nav.profile'), icon: <PersonIcon />, path: '/profile' },
  ];

  const adminLinks = [
    { label: t('nav.admin'), icon: <DashIcon />, path: '/admin' },
    { label: t('nav.adminUsers'), icon: <PeopleIcon />, path: '/admin/users' },
    { label: t('nav.adminDevices'), icon: <RouterIcon />, path: '/admin/devices' },
    { label: t('nav.adminEvents'), icon: <EventNoteIcon />, path: '/admin/events' },
    { label: t('nav.adminExport'), icon: <ExportIcon />, path: '/admin/export' },
  ];

  const drawerContent = (
    <Box>
      <Toolbar>
        <Typography variant="h6" fontWeight={700} color="primary">
          {t('app.name')}
        </Typography>
      </Toolbar>
      <Divider />
      <List dense>
        {userLinks.map((link) => (
          <ListItemButton
            key={link.path}
            selected={location.pathname === link.path}
            onClick={() => { navigate(link.path); setMobileOpen(false); }}
          >
            <ListItemIcon>{link.icon}</ListItemIcon>
            <ListItemText primary={link.label} />
          </ListItemButton>
        ))}
      </List>
      {isAdmin && (
        <>
          <Divider />
          <List dense>
            {adminLinks.map((link) => (
              <ListItemButton
                key={link.path}
                selected={location.pathname === link.path}
                onClick={() => { navigate(link.path); setMobileOpen(false); }}
              >
                <ListItemIcon>{link.icon}</ListItemIcon>
                <ListItemText primary={link.label} />
              </ListItemButton>
            ))}
          </List>
        </>
      )}
    </Box>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton color="inherit" edge="start" onClick={() => setMobileOpen(!mobileOpen)}
            sx={{ mr: 2, display: { sm: 'none' } }}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            {t('app.name')}
          </Typography>
          <Tooltip title={i18n.language === 'uk' ? 'English' : 'Українська'}>
            <Button color="inherit" onClick={toggleLang} startIcon={<LangIcon />} size="small">
              {i18n.language === 'uk' ? 'EN' : 'UK'}
            </Button>
          </Tooltip>
          <IconButton color="inherit" onClick={logout}>
            <LogoutIcon />
          </IconButton>
        </Toolbar>
      </AppBar>

      <Box component="nav" sx={{ width: { sm: DRAWER_WIDTH }, flexShrink: { sm: 0 } }}>
        <Drawer variant="temporary" open={mobileOpen} onClose={() => setMobileOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{ display: { xs: 'block', sm: 'none' }, '& .MuiDrawer-paper': { width: DRAWER_WIDTH } }}>
          {drawerContent}
        </Drawer>
        <Drawer variant="permanent"
          sx={{ display: { xs: 'none', sm: 'block' }, '& .MuiDrawer-paper': { width: DRAWER_WIDTH, boxSizing: 'border-box' } }}
          open>
          {drawerContent}
        </Drawer>
      </Box>

      <Box component="main" sx={{ flexGrow: 1, p: 3, width: { sm: `calc(100% - ${DRAWER_WIDTH}px)` }, mt: 8 }}>
        <Outlet />
      </Box>
    </Box>
  );
}
