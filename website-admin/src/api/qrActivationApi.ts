import axiosClient from './axiosClient';
import type { CreateQrActivationRequest, UpdateQrActivationRequest } from '../types/requests';
import type { QrActivationResponse } from '../types/responses';

export const qrActivationApi = {
  getAll: () => axiosClient.get<never, QrActivationResponse[]>('/api/v1/admin/qr-activations'),
  getById: (id: string) => axiosClient.get<never, QrActivationResponse>(`/api/v1/admin/qr-activations/${id}`),
  create: (payload: CreateQrActivationRequest) => axiosClient.post<never, QrActivationResponse>('/api/v1/admin/qr-activations', payload),
  update: (id: string, payload: UpdateQrActivationRequest) => axiosClient.put<never, QrActivationResponse>(`/api/v1/admin/qr-activations/${id}`, payload),
  delete: (id: string) => axiosClient.delete(`/api/v1/admin/qr-activations/${id}`),
};
