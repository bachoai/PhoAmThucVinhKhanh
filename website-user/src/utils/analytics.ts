import { analyticsApi } from '../api/analyticsApi'
const id = (key: string) => { let value = localStorage.getItem(key); if (!value) { value = `${key}-${crypto.randomUUID()}`; localStorage.setItem(key, value) } return value }
export const track = (eventName: string, lang: string, poiId?: string, metadata?: object) => analyticsApi.collect({ eventName, anonymousId: id('q4-guest'), sessionId: id('q4-session'), pageViewId: crypto.randomUUID(), lang, poiId, metadata }).catch(() => console.warn('Analytics unavailable'))
export const distance = (m: number) => m < 1000 ? `${Math.round(m)} m` : `${(m / 1000).toFixed(1)} km`
