import { useQuery } from '@tanstack/react-query';
import { Card, Col, Row, Space, Tag, Typography } from 'antd';
import { Bar, BarChart, CartesianGrid, Pie, PieChart, ResponsiveContainer, Scatter, ScatterChart, Tooltip, XAxis, YAxis, ZAxis } from 'recharts';
import { analyticsApi } from '../api/analyticsApi';
import { EmptyState } from '../components/common/EmptyState';
import { LoadingScreen } from '../components/common/LoadingScreen';
import { StatCard } from '../components/common/StatCard';
import { useI18n } from '../i18n/provider';
import { PageContainer } from '../components/layout/PageContainer';
import { formatDateTime, formatNumber } from '../utils/format';

export function AnalyticsPage() {
  const { t } = useI18n();
  const query = useQuery({ queryKey: ['analytics-summary'], queryFn: analyticsApi.summary });
  if (query.isLoading || !query.data) return <LoadingScreen />;

  const barData = query.data.topPoiViews.map((item) => ({ name: item.poiId.slice(-6), value: item.count }));
  const audioBarData = query.data.topPoiAudioPlays.map((item) => ({ name: item.poiId.slice(-6), value: item.count }));
  const pieData = [
    { name: t('dashboard_poi_views'), value: query.data.poiViewedCount },
    { name: t('dashboard_audio_plays'), value: query.data.audioPlayedCount },
    { name: 'search_executed', value: query.data.searchExecutedCount },
  ];
  const heatmapData = query.data.heatmapPoints.map((item) => ({
    lng: item.longitude,
    lat: item.latitude,
    count: item.count,
  }));
  const hasChartData = barData.length > 0 || pieData.some((item) => item.value > 0);

  return (
    <PageContainer title={t('analytics_title')} subtitle={t('analytics_subtitle')}>
      <div className="analytics-metrics">
        <StatCard title="poi_viewed" value={formatNumber(query.data.poiViewedCount)} accent="#FF6B35" subtitle="Lượt mở chi tiết POI" />
        <StatCard title="audio_played" value={formatNumber(query.data.audioPlayedCount)} accent="#2EC4B6" subtitle="Lượt phát audio hoặc thuyết minh" />
        <StatCard title="search_executed" value={formatNumber(query.data.searchExecutedCount)} accent="#4F46E5" subtitle="Số lần tìm kiếm nội dung" />
        <StatCard
          title={t('analytics_average_listen_duration')}
          value={`${query.data.averageListenDurationSeconds.toFixed(1)}s`}
          accent="#E76F51"
          subtitle="Thời gian nghe thực tế trung bình mỗi lượt thuyết minh"
        />
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
      <Row gutter={18} style={{ marginTop: 18 }}>
        <Col xs={24} lg={12}>
          <Card className="glass-card chart-card" title={t('analytics_top_poi_audio')}>
            {audioBarData.length ? (
              <ResponsiveContainer width="100%" height={280}>
                <BarChart data={audioBarData}>
                  <CartesianGrid strokeDasharray="3 3" stroke="var(--chart-grid)" />
                  <XAxis dataKey="name" stroke="var(--muted-text)" />
                  <YAxis stroke="var(--muted-text)" />
                  <Tooltip contentStyle={{ borderRadius: 16, border: '1px solid var(--glass-border)', background: 'var(--app-surface)', color: 'var(--app-text)' }} />
                  <Bar dataKey="value" fill="#2EC4B6" radius={[8, 8, 0, 0]} />
                </BarChart>
              </ResponsiveContainer>
            ) : (
              <div className="chart-empty">
                <EmptyState title="Chưa có dữ liệu lượt nghe" description="Khi audio hoặc TTS được phát, biểu đồ này sẽ hiển thị top POI được nghe nhiều nhất." />
              </div>
            )}
          </Card>
        </Col>
        <Col xs={24} lg={12}>
          <Card className="glass-card chart-card" title={t('analytics_heatmap')}>
            {heatmapData.length ? (
              <ResponsiveContainer width="100%" height={280}>
                <ScatterChart margin={{ top: 12, right: 12, bottom: 12, left: 0 }}>
                  <CartesianGrid strokeDasharray="3 3" stroke="var(--chart-grid)" />
                  <XAxis type="number" dataKey="lng" name="lng" stroke="var(--muted-text)" />
                  <YAxis type="number" dataKey="lat" name="lat" stroke="var(--muted-text)" />
                  <ZAxis type="number" dataKey="count" range={[60, 280]} />
                  <Tooltip cursor={{ strokeDasharray: '3 3' }} contentStyle={{ borderRadius: 16, border: '1px solid var(--glass-border)', background: 'var(--app-surface)', color: 'var(--app-text)' }} />
                  <Scatter data={heatmapData} fill="#F4A261" />
                </ScatterChart>
              </ResponsiveContainer>
            ) : (
              <div className="chart-empty">
                <EmptyState title={t('analytics_heatmap')} description={t('analytics_heatmap_empty')} />
              </div>
            )}
          </Card>
        </Col>
      </Row>
      <Row gutter={18} style={{ marginTop: 18 }}>
        <Col span={24}>
          <Card className="glass-card" title={t('analytics_recent_routes')}>
            {query.data.recentRouteTraces.length ? (
              <Space direction="vertical" size="middle" style={{ width: '100%' }}>
                {query.data.recentRouteTraces.map((route) => (
                  <Card key={`${route.anonymousId}-${route.sessionId ?? 'no-session'}`} size="small">
                    <Space direction="vertical" style={{ width: '100%' }}>
                      <Space wrap>
                        <Tag color="blue">{route.anonymousId.slice(-8)}</Tag>
                        <Tag>{route.sessionId?.slice(-8) ?? 'no-session'}</Tag>
                        <Tag color="gold">{route.pointCount} points</Tag>
                      </Space>
                      <Typography.Text type="secondary">
                        {formatDateTime(route.startedAt)} {' -> '} {formatDateTime(route.endedAt)}
                      </Typography.Text>
                      <Typography.Paragraph style={{ marginBottom: 0 }}>
                        {route.points.map((point) => `(${point.latitude.toFixed(5)}, ${point.longitude.toFixed(5)})`).join(' -> ')}
                      </Typography.Paragraph>
                    </Space>
                  </Card>
                ))}
              </Space>
            ) : (
              <EmptyState title={t('analytics_recent_routes')} description={t('analytics_routes_empty')} />
            )}
          </Card>
        </Col>
      </Row>
    </PageContainer>
  );
}
