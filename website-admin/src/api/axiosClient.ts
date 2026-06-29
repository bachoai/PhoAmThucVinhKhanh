import axios from 'axios';
import type { AxiosError } from 'axios';
import type { ApiResponse, ErrorResponse } from '../types/common';
import { useAuthStore } from '../store/authStore';
import { getAccessToken } from '../utils/token';

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:5163').replace(/\/$/, '');

const axiosClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
});

axiosClient.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

axiosClient.interceptors.response.use(
  (response) => {
    const payload = response.data as ApiResponse<unknown>;
    if (payload && typeof payload === 'object' && 'success' in payload && 'data' in payload) {
      return payload.data;
    }
    return response.data;
  },
  (error: AxiosError<ErrorResponse>) => {
    if (error.response?.status === 401) {
      useAuthStore.getState().clearAuth();
      window.location.replace('/login');
    }

    const message =
      error.response?.data?.errors?.[0] ??
      error.response?.data?.message ??
      error.message ??
      'Request failed';

    return Promise.reject(new Error(message));
  },
);

export default axiosClient;
