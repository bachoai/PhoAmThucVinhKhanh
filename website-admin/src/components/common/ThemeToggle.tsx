import { Button } from 'antd';
import { Moon, SunMedium } from 'lucide-react';
import { useI18n } from '../../i18n/provider';
import { useThemeStore } from '../../store/themeStore';

export function ThemeToggle() {
  const { theme, toggleTheme } = useThemeStore();
  const { t } = useI18n();
  return (
    <Button className="theme-toggle-button" icon={theme === 'dark' ? <SunMedium size={16} /> : <Moon size={16} />} onClick={toggleTheme}>
      {theme === 'dark' ? t('theme_light') : t('theme_dark')}
    </Button>
  );
}
