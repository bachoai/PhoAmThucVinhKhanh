import { analyticsApi } from '../api/analyticsApi';

const GUEST_KEY = 'q4-guest';
const SESSION_KEY = 'q4-session';
const PAGE_VIEW_CACHE_WINDOW_MS = 2000;

const recentPageViews = new Map<string, { pageViewId: string; createdAt: number }>();
const recentPoiViewSends = new Map<string, number>();

const id = (key: string) => {
  let value = localStorage.getItem(key);
  if (!value) {
    value = `${key}-${crypto.randomUUID()}`;
    localStorage.setItem(key, value);
  }
  return value;
};

const pruneExpiredEntries = <T>(store: Map<string, T>, now: number, getTimestamp: (value: T) => number) => {
  for (const [key, value] of store) {
    if (now - getTimestamp(value) > PAGE_VIEW_CACHE_WINDOW_MS) {
      store.delete(key);
    }
  }
};

function collect(eventName: string, lang: string, payload?: { poiId?: string; metadata?: object; pageViewId?: string }) {
  return analyticsApi
    .collect({
      eventName,
      anonymousId: id(GUEST_KEY),
      sessionId: id(SESSION_KEY),
      pageViewId: payload?.pageViewId ?? crypto.randomUUID(),
      lang,
      poiId: payload?.poiId,
      metadata: payload?.metadata ?? {},
    })
    .catch(() => console.warn('Analytics unavailable'));
}

export const track = (eventName: string, lang: string, poiId?: string, metadata?: object) =>
  collect(eventName, lang, { poiId, metadata });

export const createPoiPageViewId = (pageKey: string, poiId: string, source?: string | null) => {
  const now = Date.now();
  pruneExpiredEntries(recentPageViews, now, (value) => value.createdAt);

  const cacheKey = `poi:${pageKey}:${poiId}:${source ?? 'direct'}`;
  const existing = recentPageViews.get(cacheKey);
  if (existing) {
    return existing.pageViewId;
  }

  const pageViewId = crypto.randomUUID();
  recentPageViews.set(cacheKey, { pageViewId, createdAt: now });
  return pageViewId;
};

export const trackPoiView = (lang: string, poiId: string, pageViewId: string, source?: string | null) => {
  const now = Date.now();
  pruneExpiredEntries(recentPoiViewSends, now, (value) => value);

  const lastSentAt = recentPoiViewSends.get(pageViewId);
  if (lastSentAt && now - lastSentAt <= PAGE_VIEW_CACHE_WINDOW_MS) {
    return Promise.resolve();
  }

  recentPoiViewSends.set(pageViewId, now);
  return collect('poi_viewed', lang, {
    poiId,
    pageViewId,
    metadata: source ? { source } : undefined,
  });
};

export const sendPresencePing = (
  lang: string,
  metadata: {
    path: string;
    title?: string;
    isAuthenticated: boolean;
  },
  pageViewId: string,
) =>
  collect('presence_ping', lang, {
    pageViewId,
    metadata,
  });

export const distance = (m: number) => (m < 1000 ? `${Math.round(m)} m` : `${(m / 1000).toFixed(1)} km`);
