import { Button, Result } from 'antd';
import { useNavigate } from 'react-router-dom';
import { useI18n } from '../i18n/provider';

export function NotFoundPage() {
  const { t } = useI18n();
  const navigate = useNavigate();
  return <Result status="404" title="404" subTitle={t('not_found')} extra={<Button type="primary" onClick={() => navigate('/')}>{t('back_home')}</Button>} />;
}
