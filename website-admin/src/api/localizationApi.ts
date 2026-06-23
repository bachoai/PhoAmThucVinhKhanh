import axiosClient from './axiosClient';
import type { CreateLocalizationRequest, UpdateLocalizationRequest } from '../types/requests';
import type { PoiLocalizationResponse } from '../types/responses';

export const localizationApi = {
  getByPoi: (poiId: string) =>
    axiosClient.get<never, PoiLocalizationResponse[]>(`/api/v1/admin/pois/${poiId}/localizations`),
  create: (poiId: string, payload: CreateLocalizationRequest) =>
    axiosClient.post<never, PoiLocalizationResponse>(`/api/v1/admin/pois/${poiId}/localizations`, payload),
  update: (poiId: string, lang: string, payload: UpdateLocalizationRequest) =>
    axiosClient.put<never, PoiLocalizationResponse>(`/api/v1/admin/pois/${poiId}/localizations/${lang}`, payload),
  delete: (poiId: string, lang: string) =>
    axiosClient.delete(`/api/v1/admin/pois/${poiId}/localizations/${lang}`),
};
