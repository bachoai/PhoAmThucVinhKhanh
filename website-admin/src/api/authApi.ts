import axiosClient from './axiosClient';
import type { LoginRequest } from '../types/requests';
import type { AuthResponse, CurrentUserResponse } from '../types/responses';

export const authApi = {
  login: (payload: LoginRequest) => axiosClient.post<never, AuthResponse>('/api/v1/admin/auth/login', payload),
  me: () => axiosClient.get<never, CurrentUserResponse>('/api/v1/auth/me'),
};
