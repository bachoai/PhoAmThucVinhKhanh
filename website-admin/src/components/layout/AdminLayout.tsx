import { Layout } from 'antd';
import { useState } from 'react';
import { Outlet } from 'react-router-dom';
import { Header } from './Header';
import { Sidebar } from './Sidebar';

export function AdminLayout() {
  const [collapsed, setCollapsed] = useState(false);
  return (
    <Layout className="app-layout">
      <Sidebar collapsed={collapsed} />
      <Layout>
        <Header collapsed={collapsed} onToggle={() => setCollapsed((value) => !value)} />
        <Layout.Content className="app-content">
          <Outlet />
        </Layout.Content>
      </Layout>
    </Layout>
  );
}
