import { Tag } from 'antd';

const statusColorMap: Record<string, string> = {
  active: 'success',
  approved: 'success',
  inactive: 'default',
  pending: 'warning',
  rejected: 'error',
  create: 'processing',
  update: 'purple',
};

interface StatusBadgeProps {
  value?: string | boolean | null;
  trueLabel?: string;
  falseLabel?: string;
}

export function StatusBadge({ value, trueLabel = 'Active', falseLabel = 'Inactive' }: StatusBadgeProps) {
  if (typeof value === 'boolean') {
    return (
      <Tag className="status-tag" color={value ? 'success' : 'default'}>
        {value ? trueLabel : falseLabel}
      </Tag>
    );
  }

  const normalized = (value ?? '').toString().toLowerCase();
  const color = statusColorMap[normalized] ?? 'default';
  const label = normalized ? normalized.replace(/_/g, ' ') : '--';

  return (
    <Tag className="status-tag" color={color}>
      {label}
    </Tag>
  );
}
