import axiosClient from './axiosClient';
import type { CreateCategoryRequest, UpdateCategoryRequest } from '../types/requests';
import type { CategoryResponse } from '../types/responses';

export const categoryApi = {
  getAll: () => axiosClient.get<never, CategoryResponse[]>('/api/v1/categories'),
  create: (payload: CreateCategoryRequest) => axiosClient.post<never, CategoryResponse>('/api/v1/categories', payload),
  update: (id: string, payload: UpdateCategoryRequest) =>
    axiosClient.put<never, CategoryResponse>(`/api/v1/categories/${id}`, payload),
  delete: (id: string) => axiosClient.delete(`/api/v1/categories/${id}`),
};
