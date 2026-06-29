import { useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { authApi } from '../api/authApi';
import { useAppStore } from '../store/appStore';

export function SessionBootstrap() {
  const token = useAppStore((state) => state.token);
  const setCurrentUser = useAppStore((state) => state.setCurrentUser);
  const meQuery = useQuery({
    queryKey: ['auth-me', token],
    queryFn: authApi.me,
    enabled: Boolean(token),
    staleTime: 300000,
    retry: false,
  });

  useEffect(() => {
    if (meQuery.data) {
      setCurrentUser(meQuery.data);
    }
  }, [meQuery.data, setCurrentUser]);

  return null;
}
