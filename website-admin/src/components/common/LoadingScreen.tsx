import { Skeleton, Space } from 'antd';

export function LoadingScreen() {
  return (
    <Space direction="vertical" size="large" style={{ width: '100%' }}>
      <Skeleton.Input active block size="large" />
      <Skeleton active paragraph={{ rows: 10 }} />
    </Space>
  );
}
