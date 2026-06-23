import { Avatar, Button, Dropdown, Layout, Space, Typography } from 'antd';
import { LogOut, Menu, UserCircle2 } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useI18n } from '../../i18n/provider';
import { useAuthStore } from '../../store/authStore';
import { LanguageToggle } from '../common/LanguageToggle';
import { ThemeToggle } from '../common/ThemeToggle';

interface HeaderProps {
  collapsed: boolean;
  onToggle: () => void;
}

export function Header({ collapsed, onToggle }: HeaderProps) {
  const { t } = useI18n();
  const navigate = useNavigate();
  const { user, clearAuth } = useAuthStore();

  return (
    <Layout.Header className="app-header">
      <div className="header-left">
        <Button icon={<Menu size={16} />} onClick={onToggle} />
        <div>
          <Typography.Text strong>{t('header_title')}</Typography.Text>
          <div className="muted-text">{collapsed ? t('header_subtitle_compact') : t('header_subtitle')}</div>
        </div>
      </div>
      <Space size="middle">
        <LanguageToggle />
        <ThemeToggle />
        <Dropdown
          menu={{
            items: [
              {
                key: 'logout',
                icon: <LogOut size={14} />,
                label: t('logout'),
                onClick: () => {
                  clearAuth();
                  navigate('/login');
                },
              },
            ],
          }}
        >
          <Button className="user-button">
            <Space>
              <Avatar icon={<UserCircle2 size={16} />} />
              <span>{user?.fullName ?? 'Admin'}</span>
            </Space>
          </Button>
        </Dropdown>
      </Space>
    </Layout.Header>
  );
}
