import { useQuery } from '@tanstack/react-query';
import { Button, Card, Descriptions } from 'antd';
import { mapsApi } from '../api/mapsApi';
import { EmptyState } from '../components/common/EmptyState';
import { useI18n } from '../i18n/provider';
import { PageContainer } from '../components/layout/PageContainer';
import { fileSizeLabel, formatDateTime } from '../utils/format';
import { normalizeMediaUrl } from '../utils/media';

export function MapsPage() {
  const { t } = useI18n();
  const query = useQuery({ queryKey: ['map-pack'], queryFn: mapsApi.getPackManifest });

  return (
    <PageContainer title={t('maps_title')} subtitle={t('maps_subtitle')}>
      <Card className="glass-card">
        {query.data ? (
          <Descriptions column={1}>
            <Descriptions.Item label={t('map_name')}>{query.data.name}</Descriptions.Item>
            <Descriptions.Item label={t('map_version')}>{query.data.version}</Descriptions.Item>
            <Descriptions.Item label={t('status')}>{query.data.isActive ? t('yes') : t('no')}</Descriptions.Item>
            <Descriptions.Item label={t('map_size')}>{fileSizeLabel(query.data.sizeBytes)}</Descriptions.Item>
            <Descriptions.Item label="SHA256">{query.data.sha256}</Descriptions.Item>
            <Descriptions.Item label={t('created_at')}>{formatDateTime(query.data.publishedAt)}</Descriptions.Item>
            <Descriptions.Item label={t('map_download')}>
              <Button type="link" href={normalizeMediaUrl(query.data.downloadUrl)} target="_blank">
                {t('open_download_url')}
              </Button>
            </Descriptions.Item>
          </Descriptions>
        ) : (
          <EmptyState title={t('no_map_pack')} description={t('no_map_pack_desc')} />
        )}
      </Card>
    </PageContainer>
  );
}
