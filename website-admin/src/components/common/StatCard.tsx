import { Card, Statistic } from 'antd';
import { motion } from 'framer-motion';
import type { ReactNode } from 'react';

interface StatCardProps {
  title: string;
  value: number | string;
  prefix?: ReactNode;
  accent?: string;
  subtitle?: string;
}

export function StatCard({ title, value, prefix, accent, subtitle }: StatCardProps) {
  return (
    <motion.div whileHover={{ y: -5, scale: 1.01 }} transition={{ duration: 0.18 }}>
      <Card className="glass-card stat-card" style={accent ? { ['--stat-accent' as string]: accent } : undefined}>
        <Statistic title={title} value={value} prefix={prefix} />
        {subtitle ? <div className="stat-card-subtitle">{subtitle}</div> : null}
      </Card>
    </motion.div>
  );
}
