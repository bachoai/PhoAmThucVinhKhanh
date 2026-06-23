import axios from 'axios'
import type { ApiResponse } from '../types/responses'
export const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || 'http://localhost:5163').replace(/\/$/, '')
const client = axios.create({ baseURL: API_BASE_URL, timeout: 15000 })
export async function getData<T>(url: string, params?: object): Promise<T> { const { data } = await client.get<ApiResponse<T>>(url, { params }); if (!data.success) throw new Error(data.message || 'Không thể tải dữ liệu'); return data.data }
export async function postData<T>(url: string, body: object): Promise<T> { const { data } = await client.post<ApiResponse<T>>(url, body); if (!data.success) throw new Error(data.message || 'Không thể gửi dữ liệu'); return data.data }
