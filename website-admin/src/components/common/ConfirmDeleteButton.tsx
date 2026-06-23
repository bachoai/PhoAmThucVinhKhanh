import { Button, Popconfirm } from 'antd';
import { Trash2 } from 'lucide-react';
import { useI18n } from '../../i18n/provider';

interface ConfirmDeleteButtonProps {
  onConfirm: () => void;
  loading?: boolean;
}

export function ConfirmDeleteButton({ onConfirm, loading }: ConfirmDeleteButtonProps) {
  const { t } = useI18n();
  return (
    <Popconfirm title={t('delete_confirm_title')} description={t('delete_confirm_desc')} onConfirm={onConfirm}>
      <Button danger icon={<Trash2 size={15} />} loading={loading} />
    </Popconfirm>
  );
}
