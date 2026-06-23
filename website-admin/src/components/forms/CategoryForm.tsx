import { zodResolver } from '@hookform/resolvers/zod';
import { Button, Form, Input, InputNumber, Switch } from 'antd';
import { Controller, useForm } from 'react-hook-form';
import { useI18n } from '../../i18n/provider';
import type { UpdateCategoryRequest } from '../../types/requests';
import { categorySchema } from '../../utils/validators';

type CategoryFormValues = UpdateCategoryRequest;

interface CategoryFormProps {
  initialValues?: CategoryFormValues;
  onSubmit: (values: CategoryFormValues) => Promise<unknown>;
  loading?: boolean;
}

export function CategoryForm({ initialValues, onSubmit, loading }: CategoryFormProps) {
  const { t } = useI18n();
  const { control, handleSubmit } = useForm<CategoryFormValues>({
    resolver: zodResolver(categorySchema),
    defaultValues: initialValues ?? {
      code: '',
      name: '',
      description: '',
      iconUrl: '',
      sortOrder: 0,
      isActive: true,
    },
  });

  return (
    <Form layout="vertical" onFinish={handleSubmit(async (values) => onSubmit(values))}>
      <Controller name="code" control={control} render={({ field, fieldState }) => <Form.Item label={t('code')} validateStatus={fieldState.error ? 'error' : ''} help={fieldState.error?.message}><Input {...field} /></Form.Item>} />
      <Controller name="name" control={control} render={({ field, fieldState }) => <Form.Item label={t('name')} validateStatus={fieldState.error ? 'error' : ''} help={fieldState.error?.message}><Input {...field} /></Form.Item>} />
      <Controller name="description" control={control} render={({ field }) => <Form.Item label={t('description')}><Input.TextArea rows={3} {...field} /></Form.Item>} />
      <Controller name="iconUrl" control={control} render={({ field }) => <Form.Item label={t('icon_url')}><Input {...field} /></Form.Item>} />
      <Controller name="sortOrder" control={control} render={({ field }) => <Form.Item label={t('sort_order')}><InputNumber min={0} style={{ width: '100%' }} value={field.value} onChange={(value) => field.onChange(value ?? 0)} /></Form.Item>} />
      <Controller name="isActive" control={control} render={({ field }) => <Form.Item label={t('active')}><Switch checked={field.value} onChange={field.onChange} /></Form.Item>} />
      <Button type="primary" htmlType="submit" loading={loading} block>
        {t('save_category')}
      </Button>
    </Form>
  );
}
