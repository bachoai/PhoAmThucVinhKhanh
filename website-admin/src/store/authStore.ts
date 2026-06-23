import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';
import type { CurrentUserResponse } from '../types/responses';
import { clearAccessToken, setAccessToken } from '../utils/token';

interface AuthState {
  token: string | null;
  user: CurrentUserResponse | null;
  isAuthenticated: boolean;
  setAuth: (token: string, user: CurrentUserResponse) => void;
  clearAuth: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      user: null,
      isAuthenticated: false,
      setAuth: (token, user) => {
        setAccessToken(token);
        set({ token, user, isAuthenticated: true });
      },
      clearAuth: () => {
        clearAccessToken();
        set({ token: null, user: null, isAuthenticated: false });
      },
    }),
    {
      name: 'quan4-admin-auth',
      storage: createJSONStorage(() => localStorage),
    },
  ),
);
