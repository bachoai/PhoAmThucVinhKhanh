import { Segmented } from 'antd';
import { useLanguageStore } from '../../store/languageStore';
import { useI18n } from '../../i18n/provider';

export function LanguageToggle() {
  const language = useLanguageStore((state) => state.language);
  const setLanguage = useLanguageStore((state) => state.setLanguage);
  const { t } = useI18n();

  return (
    <Segmented
      size="middle"
      value={language}
      onChange={(value) => setLanguage(value as 'vi' | 'en')}
      options={[
        { label: t('language_vi'), value: 'vi' },
        { label: t('language_en'), value: 'en' },
      ]}
    />
  );
}
