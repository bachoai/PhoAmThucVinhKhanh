import { zodResolver } from '@hookform/resolvers/zod';
import { Button, Form, Input, Select, Upload } from 'antd';
import { UploadCloud } from 'lucide-react';
import { useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useI18n } from '../../i18n/provider';
import type { UploadPoiAudioRequest } from '../../types/requests';
import { SUPPORTED_LANGUAGES } from '../../utils/constants';
import { audioSchema } from '../../utils/validators';

interface AudioFormProps {
  hideLanguage?: boolean;
  loading?: boolean;
  onSubmit: (values: UploadPoiAudioRequest, file?: File) => Promise<unknown>;
}

export function AudioForm({ hideLanguage = false, loading, onSubmit }: AudioFormProps) {
  const { t } = useI18n();
  const [file, setFile] = useState<File | undefined>();
  const { control, handleSubmit } = useForm<UploadPoiAudioRequest>({
    resolver: zodResolver(audioSchema),
    defaultValues: {
      lang: 'vi',
      audioUrl: '',
      voiceName: '',
      sourceType: 'uploaded',
    },
  });

  return (
    <Form layout="vertical" onFinish={handleSubmit(async (values) => onSubmit(values, file))}>
      {hideLanguage ? null : (
        <Controller
          name="lang"
          control={control}
          render={({ field }) => (
            <Form.Item label={t('language')}>
              <Select {...field} options={SUPPORTED_LANGUAGES.map((value) => ({ value, label: value.toUpperCase() }))} />
            </Form.Item>
          )}
        />
      )}
      <Controller name="audioUrl" control={control} render={({ field }) => <Form.Item label={t('audio_url')}><Input {...field} placeholder="Optional when uploading file" /></Form.Item>} />
      <Controller name="voiceName" control={control} render={({ field }) => <Form.Item label={t('voice_name')}><Input {...field} /></Form.Item>} />
      <Controller name="sourceType" control={control} render={({ field }) => <Form.Item label={t('source_type')}><Input {...field} /></Form.Item>} />
      <Form.Item label={t('audio_title')}>
        <Upload beforeUpload={(selectedFile) => { setFile(selectedFile); return false; }} maxCount={1}>
          <Button icon={<UploadCloud size={16} />}>{t('choose_audio_file')}</Button>
        </Upload>
      </Form.Item>
      <Button type="primary" htmlType="submit" loading={loading} block>
        {t('save_audio')}
      </Button>
    </Form>
  );
}
