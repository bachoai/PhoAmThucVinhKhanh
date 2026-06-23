export type Lang = 'vi' | 'en' | 'zh' | 'ja' | 'ko'
export interface ApiResponse<T> { success: boolean; message: string; data: T }
export interface Category { id: string; code: string; name: string; description?: string; iconUrl?: string }
export interface PoiImage { url: string; caption?: string; isThumbnail?: boolean }
export interface Poi { id: string; name: string; description: string; categoryId: string; address: string; ward?: string; district?: string; city?: string; priceRange: string; rating: number; reviewCount: number; priority: number; latitude: number; longitude: number; tags: string[]; images: PoiImage[] }
export interface PoiDetail extends Poi { openingHours: { dayOfWeek: string; openTime: string; closeTime: string; isClosed: boolean }[]; contactInfo?: { phone?: string; email?: string; facebookUrl?: string; websiteUrl?: string }; audioStatus?: string }
export interface NearbyPoi extends Poi { distanceMeters: number }
export interface PoiAudio { id: string; poiId: string; lang: string; audioUrl: string; voiceName?: string; durationSeconds?: number }
