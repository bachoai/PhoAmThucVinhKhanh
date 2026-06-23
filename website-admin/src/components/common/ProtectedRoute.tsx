import { Result, Spin } from 'antd';
import { useEffect, useState } from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { useI18n } from '../../i18n/provider';
import { useAuthStore } from '../../store/authStore';

export function ProtectedRoute() {
  const { t } = useI18n();
  const { isAuthenticated, token, user, setAuth, clearAuth } = useAuthStore();
  const [checking, setChecking] = useState(Boolean(token && !user));
  const location = useLocation();

  useEffect(() => {
    if (!token || user) return;
    let active = true;
    authApi
      .me()
      .then((currentUser) => {
        if (active) setAuth(token, currentUser);
      })
      .catch(() => {
        if (active) clearAuth();
      })
      .finally(() => {
        if (active) setChecking(false);
      });
    return () => {
      active = false;
    };
  }, [clearAuth, setAuth, token, user]);

  if (checking) {
    return (
      <div className="center-screen">
        <Spin size="large" />
      </div>
    );
  }

  if (!isAuthenticated || !token) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  if (!user?.roles.includes('Admin')) {
    return <Result status="403" title="403" subTitle={t('auth_admin_only')} />;
  }

  return <Outlet />;
}
