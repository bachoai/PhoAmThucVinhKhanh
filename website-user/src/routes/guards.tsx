import type React from 'react';
import { useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Navigate, useLocation } from 'react-router-dom';
import { authApi } from '../api/authApi';
import { Spinner } from '../components/common/Spinner';
import { useAppStore } from '../store/appStore';
import { hasRole } from '../utils/auth';

export function RequireAuth({ children }: { children: React.ReactNode }) {
  const isAuthenticated = useAppStore((state) => state.isAuthenticated);
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to={`/login?next=${encodeURIComponent(`${location.pathname}${location.search}`)}`} replace />;
  }

  return <>{children}</>;
}

export function RequireOwner({ children }: { children: React.ReactNode }) {
  const token = useAppStore((state) => state.token);
  const currentUser = useAppStore((state) => state.currentUser);
  const setCurrentUser = useAppStore((state) => state.setCurrentUser);
  const meQuery = useQuery({
    queryKey: ['owner-guard-me', token],
    queryFn: authApi.me,
    enabled: Boolean(token),
    staleTime: 0,
    refetchOnMount: 'always',
    retry: false,
  });

  useEffect(() => {
    if (meQuery.data) {
      setCurrentUser(meQuery.data);
    }
  }, [meQuery.data, setCurrentUser]);

  const resolvedCurrentUser = meQuery.data ?? currentUser;

  if (meQuery.isLoading && !resolvedCurrentUser) {
    return <Spinner />;
  }

  if (!hasRole(resolvedCurrentUser?.roles, 'Owner')) {
    const message =
      resolvedCurrentUser?.ownerStatus === 'approved'
        ? 'Tài khoản đã được duyệt đối tác nhưng chưa có quyền Owner đầy đủ. Hãy đăng xuất đăng nhập lại hoặc nhờ admin kiểm tra role.'
        : 'Bạn cần được duyệt quyền đối tác trước khi vào trang quản lý đối tác.';

    return <Navigate to="/account" replace state={{ ownerAccessMessage: message }} />;
  }

  return <>{children}</>;
}
