import { getData } from './axiosClient'; import type { Category } from '../types/responses'; export const categoryApi = { list: () => getData<Category[]>('/api/v1/categories') }
