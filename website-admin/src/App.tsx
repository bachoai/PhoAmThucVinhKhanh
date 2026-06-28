import { App as AntApp, ConfigProvider, theme as antdTheme } from 'antd';
import enUS from 'antd/locale/en_US';
import viVN from 'antd/locale/vi_VN';
import { useEffect } from 'react';
import { I18nProvider } from './i18n/provider';
import { AppRoutes } from './routes/AppRoutes';
import { useLanguageStore } from './store/languageStore';
import { useThemeStore } from './store/themeStore';

function App() {
  const theme = useThemeStore((state) => state.theme);
  const language = useLanguageStore((state) => state.language);

  useEffect(() => {
    document.body.classList.toggle('theme-dark', theme === 'dark');
    document.body.classList.toggle('theme-light', theme === 'light');
  }, [theme]);

  return (
    <ConfigProvider
      locale={language === 'vi' ? viVN : enUS}
      theme={{
        algorithm: theme === 'dark' ? antdTheme.darkAlgorithm : antdTheme.defaultAlgorithm,
        token: {
          colorPrimary: '#FF6B35',
          colorInfo: '#2EC4B6',
          borderRadius: 14,
          fontFamily: '"Be Vietnam Pro", "Segoe UI", sans-serif',
        },
        components: {
          Layout: {
            bodyBg: 'transparent',
            siderBg: theme === 'dark' ? '#0b1220' : '#f8fafc',
            headerBg: theme === 'dark' ? 'rgba(15, 23, 42, 0.72)' : 'rgba(255, 255, 255, 0.78)',
          },
          Table: {
            colorBgContainer: theme === 'dark' ? 'rgba(11, 24, 42, 0.72)' : 'rgba(255, 255, 255, 0.72)',
            headerBg: theme === 'dark' ? 'rgba(255, 107, 53, 0.16)' : 'rgba(255, 107, 53, 0.08)',
            borderColor: theme === 'dark' ? 'rgba(148, 163, 184, 0.16)' : 'rgba(148, 163, 184, 0.18)',
          },
          Modal: {
            contentBg: theme === 'dark' ? '#0f1d33' : '#ffffff',
            headerBg: theme === 'dark' ? '#0f1d33' : '#ffffff',
          },
          Input: {
            activeBorderColor: '#FF6B35',
          },
          Select: {
            optionSelectedBg: theme === 'dark' ? 'rgba(255, 107, 53, 0.18)' : 'rgba(255, 107, 53, 0.12)',
          },
          Card: {
            boxShadowTertiary:
              theme === 'dark'
                ? '0 16px 40px rgba(15, 23, 42, 0.35)'
                : '0 14px 32px rgba(15, 23, 42, 0.08)',
          },
        },
      }}
    >
      <AntApp>
        <I18nProvider>
          <AppRoutes />
        </I18nProvider>
      </AntApp>
    </ConfigProvider>
  );
}

export default App;
