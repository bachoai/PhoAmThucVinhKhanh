import { useQuery } from '@tanstack/react-query';
import { Card, Col, Row, Typography } from 'antd';
import { Bar, BarChart, CartesianGrid, Pie, PieChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import { analyticsApi } from '../api/analyticsApi';
import { EmptyState } from '../components/common/EmptyState';
import { LoadingScreen } from '../components/common/LoadingScreen';
import { StatCard } from '../components/common/StatCard';
import { useI18n } from '../i18n/provider';
import { PageContainer } from '../components/layout/PageContainer';
import { formatNumber } from '../utils/format';

export function AnalyticsPage() {
  const { t } = useI18n();
  const query = useQuery({ queryKey: ['analytics-summary'], queryFn: analyticsApi.summary });
  if (query.isLoading || !query.data) return <LoadingScreen />;

  const barData = query.data.topPoiViews.map((item) => ({ name: item.poiId.slice(-6), value: item.count }));
  const pieData = [
    { name: t('dashboard_poi_views'), value: query.data.poiViewedCount },
    { name: t('dashboard_audio_plays'), value: query.data.audioPlayedCount },
    { name: 'search_executed', value: query.data.searchExecutedCount },
  ];
  const hasChartData = barData.length > 0 || pieData.some((item) => item.value > 0);

  return (
    <PageContainer title={t('analytics_title')} subtitle={t('analytics_subtitle')}>
      <div className="analytics-metrics">
        <StatCard title="poi_viewed" value={formatNumber(query.data.poiViewedCount)} accent="#FF6B35" subtitle="Lượt mở chi tiết POI" />
        <StatCard title="audio_played" value={formatNumber(query.data.audioPlayedCount)} accent="#2EC4B6" subtitle="Lượt phát audio hoặc thuyết minh" />
        <StatCard title="search_executed" value={formatNumber(query.data.searchExecutedCount)} accent="#4F46E5" subtitle="Số lần tìm kiếm nội dung" />
        <Card className="glass-card">
          <Typography.Title level={5}>Diễn giải dữ liệu</Typography.Title>
          <Typography.Paragraph type="secondary" style={{ marginBottom: 0 }}>
            Dashboard này phản ánh lưu lượng người dùng quan tâm tới POI, audio và hành vi tra cứu trong app demo.
          </Typography.Paragraph>
        </Card>
      </div>
      <Row gutter={18}>
        <Col xs={24} lg={14}>
          <Card className="glass-card chart-card" title={t('analytics_top_poi_views')}>
            {barData.length ? (
              <ResponsiveContainer width="100%" height={280}>
                <BarChart data={barData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="var(--chart-grid)" />
                  <XAxis dataKey="name" stroke="var(--muted-text)" />
                  <YAxis stroke="var(--muted-text)" />
                  <Tooltip contentStyle={{ borderRadius: 16, border: '1px solid var(--glass-border)', background: 'var(--app-surface)', color: 'var(--app-text)' }} />
                  <Bar dataKey="value" fill="#FF6B35" radius={[8, 8, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            ) : (
              <div className="chart-empty">
                <EmptyState title="Chưa có dữ liệu lượt xem" description="Khi người dùng tương tác với POI, biểu đồ này sẽ hiển thị rõ hơn." />
              </div>
            )}
          </Card>
        </Col>
        <Col xs={24} lg={10}>
          <Card className="glass-card chart-card" title={t('analytics_event_distribution')}>
            {hasChartData ? (
              <ResponsiveContainer width="100%" height={280}>
                <PieChart>
                  <Pie data={pieData} dataKey="value" nameKey="name" outerRadius={96} fill="#2EC4B6" label />
                  <Tooltip contentStyle={{ borderRadius: 16, border: '1px solid var(--glass-border)', background: 'var(--app-surface)', color: 'var(--app-text)' }} />
                </PieChart>
              </ResponsiveContainer>
            ) : (
              <div className="chart-empty">
                <EmptyState title="Biểu đồ đang trống" description="Hãy tạo thêm tương tác demo để thống kê event rõ hơn." />
              </div>
            )}
          </Card>
        </Col>
      </Row>
    </PageContainer>
  );
}
