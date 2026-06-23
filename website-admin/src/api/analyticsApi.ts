import axiosClient from './axiosClient';
import type { CollectAnalyticsRequest } from '../types/requests';
import type { AnalyticsSummaryResponse } from '../types/responses';

export const analyticsApi = {
  collect: (payload: CollectAnalyticsRequest) => axiosClient.post('/api/v1/analytics/collect', payload),
  summary: () => axiosClient.get<never, AnalyticsSummaryResponse>('/api/v1/admin/analytics/summary'),
};
