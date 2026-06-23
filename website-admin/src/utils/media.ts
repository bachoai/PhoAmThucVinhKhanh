const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL ?? '').trim().replace(/\/$/, '');

export const MEDIA_PLACEHOLDER =
  'data:image/svg+xml;utf8,' +
  encodeURIComponent(`
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 420">
      <defs>
        <linearGradient id="bg" x1="0%" y1="0%" x2="100%" y2="100%">
          <stop offset="0%" stop-color="#FF6B35" stop-opacity="0.95" />
          <stop offset="100%" stop-color="#2EC4B6" stop-opacity="0.95" />
        </linearGradient>
      </defs>
      <rect width="640" height="420" rx="28" fill="url(#bg)" />
      <circle cx="160" cy="120" r="72" fill="rgba(255,255,255,0.2)" />
      <path d="M96 320l100-108 86 76 82-102 180 134H96z" fill="rgba(255,255,255,0.25)" />
      <text x="50%" y="54%" dominant-baseline="middle" text-anchor="middle" fill="#fff"
        font-family="Be Vietnam Pro, Segoe UI, sans-serif" font-size="34" font-weight="700">
        Quan 4 Culinary Tourism
      </text>
      <text x="50%" y="64%" dominant-baseline="middle" text-anchor="middle" fill="rgba(255,255,255,0.88)"
        font-family="Be Vietnam Pro, Segoe UI, sans-serif" font-size="18">
        Demo media placeholder
      </text>
    </svg>
  `);

export function normalizeMediaUrl(url?: string | null) {
  if (!url) {
    return '';
  }

  if (url.startsWith('/uploads')) {
    return apiBaseUrl ? `${apiBaseUrl}${url}` : url;
  }

  if (/^https?:\/\//i.test(url) || url.startsWith('data:') || url.startsWith('blob:')) {
    return url;
  }

  return url.startsWith('/') && apiBaseUrl ? `${apiBaseUrl}${url}` : url;
}

export function getMediaUrlOrPlaceholder(url?: string | null) {
  return normalizeMediaUrl(url) || MEDIA_PLACEHOLDER;
}
