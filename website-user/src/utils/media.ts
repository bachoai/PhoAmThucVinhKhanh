import { API_BASE_URL } from '../api/axiosClient';

const fallback = 'https://images.unsplash.com/photo-1559339352-11d035aa65de?auto=format&fit=crop&w=1000&q=80';

export const normalizeMediaUrl = (url?: string | null) => {
  if (!url) {
    return fallback;
  }

  if (/^(https?:|data:|blob:)/i.test(url)) {
    return url;
  }

  if (url.startsWith('//')) {
    return `https:${url}`;
  }

  return new URL(url.replace(/^\.\//, ''), `${API_BASE_URL}/`).toString();
};

export const poiImage = (poi: { images?: { url: string; isThumbnail?: boolean }[] }) =>
  normalizeMediaUrl(poi.images?.find((image) => image.isThumbnail)?.url || poi.images?.[0]?.url);
