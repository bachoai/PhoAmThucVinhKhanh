import { Button, Form, Input, InputNumber, Select, Switch } from 'antd';
import { Controller, useForm } from 'react-hook-form';
import { useQuery } from '@tanstack/react-query';
import { poiApi } from '../../api/poiApi';
import { useI18n } from '../../i18n/provider';
import type { CreateQrActivationRequest, UpdateQrActivationRequest } from '../../types/requests';
import type { QrActivationResponse } from '../../types/responses';

type QrActivationFormValues = CreateQrActivationRequest;

interface QrActivationFormProps {
  initialValues?: QrActivationResponse | null;
  loading?: boolean;
  onSubmit: (values: CreateQrActivationRequest | UpdateQrActivationRequest) => Promise<unknown>;
}

const SCAN_MODE_OPTIONS = [
  { value: 'prefer_audio', label: 'prefer_audio' },
  { value: 'audio', label: 'audio' },
  { value: 'tts', label: 'tts' },
] as const;

export function QrActivationForm({ initialValues, loading, onSubmit }: QrActivationFormProps) {
  const { t } = useI18n();
  const poisQuery = useQuery({ queryKey: ['pois'], queryFn: () => poiApi.loadAll() });
  const { control, handleSubmit } = useForm<QrActivationFormValues>({
    defaultValues: {
      code: initialValues?.code ?? '',
      poiId: initialValues?.poiId ?? '',
      title: initialValues?.title ?? '',
      stopZone: initialValues?.stopZone ?? '',
      stopAddress: initialValues?.stopAddress ?? '',
      sortOrder: initialValues?.sortOrder ?? 0,
      description: initialValues?.description ?? '',
      scanMode: initialValues?.scanMode ?? 'prefer_audio',
      isActive: initialValues?.isActive ?? true,
    },
  });

  return (
    <Form
      layout="vertical"
      onFinish={handleSubmit(async (values) => {
        await onSubmit({
          code: values.code.trim(),
          poiId: values.poiId,
          title: values.title.trim(),
          stopZone: values.stopZone.trim(),
          stopAddress: values.stopAddress?.trim() || undefined,
          sortOrder: values.sortOrder,
          description: values.description?.trim() || undefined,
          scanMode: values.scanMode,
          isActive: values.isActive,
        });
      })}
    >
      <Controller name="code" control={control} render={({ field }) => <Form.Item label={t('code')}><Input {...field} placeholder="KHANHHOI-01" /></Form.Item>} />
      <Controller
        name="poiId"
        control={control}
        render={({ field }) => (
          <Form.Item label="POI">
            <Select
              {...field}
              showSearch
              loading={poisQuery.isLoading}
              options={(poisQuery.data ?? []).map((item) => ({ value: item.id, label: `${item.name} (${item.id.slice(-6)})` }))}
            />
          </Form.Item>
        )}
      />
      <Controller name="title" control={control} render={({ field }) => <Form.Item label={t('name')}><Input {...field} /></Form.Item>} />
      <Controller name="stopZone" control={control} render={({ field }) => <Form.Item label={t('qr_stop_zone')}><Input {...field} placeholder="Khanh Hoi" /></Form.Item>} />
      <Controller name="stopAddress" control={control} render={({ field }) => <Form.Item label={t('qr_stop_address')}><Input {...field} placeholder="Ben Van Don - cau Khanh Hoi" /></Form.Item>} />
      <Controller
        name="sortOrder"
        control={control}
        render={({ field }) => (
          <Form.Item label={t('sort_order')}>
            <InputNumber min={0} style={{ width: '100%' }} value={field.value} onChange={(value) => field.onChange(value ?? 0)} />
          </Form.Item>
        )}
      />
      <Controller name="description" control={control} render={({ field }) => <Form.Item label={t('description')}><Input.TextArea rows={3} {...field} /></Form.Item>} />
      <Controller
        name="scanMode"
        control={control}
        render={({ field }) => (
          <Form.Item label={t('scan_mode')}>
            <Select {...field} options={SCAN_MODE_OPTIONS.map((item) => ({ value: item.value, label: item.label }))} />
          </Form.Item>
        )}
      />
      <Controller name="isActive" control={control} render={({ field }) => <Form.Item label={t('active')}><Switch checked={field.value} onChange={field.onChange} /></Form.Item>} />
      <Button type="primary" htmlType="submit" block loading={loading}>
        {initialValues ? t('type_update') : t('type_create')}
      </Button>
    </Form>
  );
}
