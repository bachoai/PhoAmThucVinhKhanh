import { createContext, useContext } from 'react';
import type { PropsWithChildren } from 'react';
import { useLanguageStore } from '../store/languageStore';
import { messages, type MessageKey } from './messages';

interface I18nContextValue {
  language: 'vi' | 'en';
  t: (key: MessageKey) => string;
}

const I18nContext = createContext<I18nContextValue | null>(null);

export function I18nProvider({ children }: PropsWithChildren) {
  const language = useLanguageStore((state) => state.language);

  const value: I18nContextValue = {
    language,
    t: (key) => messages[language][key],
  };

  return <I18nContext.Provider value={value}>{children}</I18nContext.Provider>;
}

export function useI18n() {
  const context = useContext(I18nContext);
  if (!context) {
    throw new Error('useI18n must be used within I18nProvider');
  }
  return context;
}
