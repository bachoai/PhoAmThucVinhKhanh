import { Empty } from 'antd';
import type { ReactNode } from 'react';

interface EmptyStateProps {
  title: string;
  description: string;
  action?: ReactNode;
}

export function EmptyState({ title, description, action }: EmptyStateProps) {
  return (
    <div className="empty-state-card">
      <Empty description={<span>{title}</span>} />
      <p className="muted-text">{description}</p>
      {action ? <div>{action}</div> : null}
    </div>
  );
}
