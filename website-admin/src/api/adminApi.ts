import axiosClient from './axiosClient';
import type { UpdateUserRolesRequest, UpdateUserStatusRequest } from '../types/requests';
import type { AdminDashboardResponse, UserResponse } from '../types/responses';

export const adminApi = {
  getDashboardStats: () => axiosClient.get<never, AdminDashboardResponse>('/api/v1/admin/dashboard/stats'),
  getUsers: () => axiosClient.get<never, UserResponse[]>('/api/v1/admin/users'),
  updateUserStatus: (id: string, payload: UpdateUserStatusRequest) =>
    axiosClient.put(`/api/v1/admin/users/${id}/status`, payload),
  updateUserRoles: (id: string, payload: UpdateUserRolesRequest) =>
    axiosClient.put(`/api/v1/admin/users/${id}/roles`, payload),
};
