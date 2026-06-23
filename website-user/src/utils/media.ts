import { API_BASE_URL } from '../api/axiosClient'
const fallback = 'https://images.unsplash.com/photo-1559339352-11d035aa65de?auto=format&fit=crop&w=1000&q=80'
export const normalizeMediaUrl = (url?: string | null) => !url ? fallback : /^https?:\/\//.test(url) ? url : `${API_BASE_URL}/${url.replace(/^\//, '')}`
export const poiImage = (poi: { images?: { url: string; isThumbnail?: boolean }[] }) => normalizeMediaUrl(poi.images?.find(i => i.isThumbnail)?.url || poi.images?.[0]?.url)
