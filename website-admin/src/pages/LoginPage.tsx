import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { Alert, App, Button, Card, Form, Input, Space, Typography } from 'antd';
import { motion } from 'framer-motion';
import { LockKeyhole, Mail, UtensilsCrossed } from 'lucide-react';
import { Controller, useForm } from 'react-hook-form';
import { useLocation, useNavigate } from 'react-router-dom';
import { authApi } from '../api/authApi';
import { LanguageToggle } from '../components/common/LanguageToggle';
import { ThemeToggle } from '../components/common/ThemeToggle';
import { useI18n } from '../i18n/provider';
import { useAuthStore } from '../store/authStore';
import type { LoginRequest } from '../types/requests';
import { loginSchema } from '../utils/validators';

export function LoginPage() {
  const { t } = useI18n();
  const { notification } = App.useApp();
  const setAuth = useAuthStore((state) => state.setAuth);
  const navigate = useNavigate();
  const location = useLocation();
  const { control, handleSubmit } = useForm<LoginRequest>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: 'admin@quan4tourism.local',
      password: 'Admin@123456',
    },
  });

  const loginMutation = useMutation({
    mutationFn: authApi.login,
    onSuccess: (data) => {
      setAuth(data.token, data.user);
      navigate(location.state?.from?.pathname ?? '/admin/dashboard', { replace: true });
    },
    onError: (error: Error) => {
      notification.error({ message: t('login_failed'), description: error.message });
    },
  });

  return (
    <div className="auth-shell">
      <motion.div initial={{ opacity: 0, y: 24 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.32 }}>
        <Card className="auth-card">
          <Space direction="vertical" size="large" style={{ width: '100%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
              <div>
                <div className="brand-badge" style={{ marginBottom: 16 }}>
                  <UtensilsCrossed size={20} />
                </div>
                <Typography.Title level={2} style={{ marginBottom: 4 }}>
                  {t('app_name')}
                </Typography.Title>
                <Typography.Text type="secondary">{t('app_subtitle')}</Typography.Text>
              </div>
              <Space>
                <LanguageToggle />
                <ThemeToggle />
              </Space>
            </div>
            <Alert
              type="info"
              showIcon
              message={t('login_default_admin')}
              description="admin@quan4tourism.local / Admin@123456"
            />
            <Form layout="vertical" onFinish={handleSubmit(async (values) => loginMutation.mutate(values))}>
              <Controller name="email" control={control} render={({ field, fieldState }) => <Form.Item label={t('login_email')} validateStatus={fieldState.error ? 'error' : ''} help={fieldState.error?.message}><Input {...field} prefix={<Mail size={16} />} /></Form.Item>} />
              <Controller name="password" control={control} render={({ field, fieldState }) => <Form.Item label={t('login_password')} validateStatus={fieldState.error ? 'error' : ''} help={fieldState.error?.message}><Input.Password {...field} prefix={<LockKeyhole size={16} />} /></Form.Item>} />
              <Button type="primary" htmlType="submit" size="large" loading={loginMutation.isPending} block>
                {t('login_button')}
              </Button>
            </Form>
          </Space>
        </Card>
      </motion.div>
    </div>
  );
}
