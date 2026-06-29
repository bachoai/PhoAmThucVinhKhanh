export function StatusPill({
  status,
}: {
  status: 'pending' | 'approved' | 'rejected' | string;
}) {
  const normalizedStatus = status?.trim().toLowerCase() || 'none';
  const classes =
    normalizedStatus === 'approved'
      ? 'bg-emerald-100 text-emerald-700'
      : normalizedStatus === 'rejected'
        ? 'bg-rose-100 text-rose-700'
        : normalizedStatus === 'none'
          ? 'bg-slate-200 text-slate-700'
          : 'bg-amber-100 text-amber-700';

  const labelMap: Record<string, string> = {
    none: 'chưa đăng ký',
    pending: 'đang chờ duyệt',
    approved: 'đã duyệt',
    rejected: 'bị từ chối',
  };
  const label = labelMap[normalizedStatus] ?? status;

  return <span className={`rounded-full px-3 py-1 text-xs font-bold uppercase ${classes}`}>{label}</span>;
}
