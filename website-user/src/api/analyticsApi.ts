import { postData } from './axiosClient'; export const analyticsApi = { collect: (payload: object) => postData<null>('/api/v1/analytics/collect', payload) }
