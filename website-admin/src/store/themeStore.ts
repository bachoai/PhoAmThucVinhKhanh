import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';

export type AppTheme = 'light' | 'dark';

interface ThemeState {
  theme: AppTheme;
  toggleTheme: () => void;
}

export const useThemeStore = create<ThemeState>()(
  persist(
    (set) => ({
      theme: 'dark',
      toggleTheme: () => set((state) => ({ theme: state.theme === 'dark' ? 'light' : 'dark' })),
    }),
    {
      name: 'quan4-admin-theme',
      storage: createJSONStorage(() => localStorage),
    },
  ),
);
