import React from 'react';
import ReactDOM from 'react-dom/client';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import './i18n/index';
import App from './App';

const theme = createTheme({
  palette: {
    mode: 'dark',
    primary: { main: '#4FC3F7' },
    secondary: { main: '#81C784' },
    background: {
      default: '#121212',
      paper: '#1E1E1E',
    },
    error: { main: '#CF6679' },
  },
  typography: {
    fontFamily: 'Roboto, sans-serif',
  },
});

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <App />
    </ThemeProvider>
  </React.StrictMode>,
);
