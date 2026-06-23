import dayjs from 'dayjs';

export function formatDateTime(value?: string | null) {
  return value ? dayjs(value).format('DD/MM/YYYY HH:mm') : '--';
}

export function formatNumber(value: number) {
  return new Intl.NumberFormat('vi-VN').format(value);
}

export function fileSizeLabel(sizeBytes: number) {
  if (sizeBytes < 1024) return `${sizeBytes} B`;
  if (sizeBytes < 1024 * 1024) return `${(sizeBytes / 1024).toFixed(1)} KB`;
  return `${(sizeBytes / (1024 * 1024)).toFixed(1)} MB`;
}
