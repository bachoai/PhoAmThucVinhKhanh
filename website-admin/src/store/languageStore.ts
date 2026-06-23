import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';

export type AppLanguage = 'vi' | 'en';

interface LanguageState {
  language: AppLanguage;
  setLanguage: (language: AppLanguage) => void;
  toggleLanguage: () => void;
}

export const useLanguageStore = create<LanguageState>()(
  persist(
    (set) => ({
      language: 'vi',
      setLanguage: (language) => set({ language }),
      toggleLanguage: () =>
        set((state) => ({ language: state.language === 'vi' ? 'en' : 'vi' })),
    }),
    {
      name: 'quan4-admin-language',
      storage: createJSONStorage(() => localStorage),
    },
  ),
);
