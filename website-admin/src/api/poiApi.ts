import axiosClient from './axiosClient';
import type { CreatePoiRequest, UpdatePoiRequest } from '../types/requests';
import type { NearbyPoiResponse, PoiDetailResponse, PoiResponse } from '../types/responses';

export interface PoiFilters {
  lang?: string;
  keyword?: string;
  categoryId?: string;
  priceRange?: string;
  page?: number;
  pageSize?: number;
}

export const poiApi = {
  loadAll: (params?: PoiFilters) => axiosClient.get<never, PoiResponse[]>('/api/v1/poi/load-all', { params }),
  getById: (id: string, lang?: string) => axiosClient.get<never, PoiDetailResponse>(`/api/v1/poi/${id}`, { params: { lang } }),
  nearby: (params: { lat: number; lng: number; radius?: number; limit?: number; lang?: string }) =>
    axiosClient.get<never, NearbyPoiResponse[]>('/api/v1/poi/nearby', { params }),
  create: (payload: CreatePoiRequest) => axiosClient.post<never, PoiDetailResponse>('/api/v1/admin/pois', payload),
  update: (id: string, payload: UpdatePoiRequest) => axiosClient.put<never, PoiDetailResponse>(`/api/v1/admin/pois/${id}`, payload),
  delete: (id: string) => axiosClient.delete(`/api/v1/admin/pois/${id}`),
  setActive: (id: string, isActive: boolean) =>
    axiosClient.patch(`/api/v1/admin/pois/${id}/active`, null, { params: { isActive } }),
};
