import type { ContactInfo, OpeningHour, PoiImage } from './common';

export interface CurrentUserResponse {
  id: string;
  fullName: string;
  email: string;
  phoneNumber?: string | null;
  avatarUrl?: string | null;
  roles: string[];
  isActive: boolean;
  emailVerified: boolean;
  ownerStatus: string;
}

export interface AuthResponse {
  token: string;
  user: CurrentUserResponse;
}

export interface CategoryResponse {
  id: string;
  code: string;
  name: string;
  description?: string | null;
  iconUrl?: string | null;
  sortOrder: number;
  isActive: boolean;
}

export interface PoiResponse {
  id: string;
  name: string;
  description: string;
  categoryId: string;
  address: string;
  ward: string;
  district: string;
  city: string;
  priceRange: string;
  rating: number;
  reviewCount: number;
  priority: number;
  latitude: number;
  longitude: number;
  tags: string[];
  images: PoiImage[];
  isActive: boolean;
}

export interface PoiDetailResponse extends PoiResponse {
  openingHours: OpeningHour[];
  contactInfo?: ContactInfo | null;
  ownerId?: string | null;
  audioStatus: string;
}

export interface NearbyPoiResponse extends PoiResponse {
  distanceMeters: number;
}

export interface UserResponse extends CurrentUserResponse {
  lastLoginAt?: string | null;
  createdAt: string;
}

export interface OwnerRegistrationResponse {
  id: string;
  userId: string;
  businessName: string;
  businessAddress: string;
  phoneNumber: string;
  description?: string | null;
  status: string;
  adminNote?: string | null;
  createdAt: string;
}

export interface OwnerSubmissionResponse {
  id: string;
  ownerId: string;
  poiId?: string | null;
  submissionType: string;
  poiName: string;
  status: string;
  adminNote?: string | null;
  createdAt: string;
}

export interface AdminDashboardResponse {
  totalUsers: number;
  totalOwners: number;
  totalPois: number;
  totalActivePois: number;
  pendingOwnerRegistrations: number;
  pendingSubmissions: number;
  totalPoiViews: number;
  totalAudioPlays: number;
}

export interface PoiLocalizationResponse {
  id: string;
  poiId: string;
  lang: string;
  name: string;
  description: string;
  audioUrl?: string | null;
  isFallback: boolean;
}

export interface PoiAudioResponse {
  id: string;
  poiId: string;
  lang: string;
  audioUrl: string;
  voiceName?: string | null;
  sourceType: string;
  status: string;
  durationSeconds: number;
  fileSizeBytes: number;
}

export interface AudioLanguageResponse {
  code: string;
  name: string;
}

export interface TopPoiAnalyticsResponse {
  poiId: string;
  count: number;
}

export interface AnalyticsSummaryResponse {
  poiViewedCount: number;
  audioPlayedCount: number;
  searchExecutedCount: number;
  topPoiViews: TopPoiAnalyticsResponse[];
  topPoiAudioPlays: TopPoiAnalyticsResponse[];
}

export interface MediaFileResponse {
  id: string;
  fileName: string;
  originalFileName: string;
  url: string;
  contentType: string;
  fileType: string;
  sizeBytes: number;
}

export interface MapPackResponse {
  id: string;
  version: string;
  name: string;
  downloadUrl: string;
  sha256: string;
  sizeBytes: number;
  isActive: boolean;
  publishedAt?: string | null;
}

export interface AudioPackManifestResponse {
  version: string;
  generatedAt: string;
  items: Array<{
    poiId: string;
    poiName: string;
    audios: Array<{ lang: string; audioUrl: string; status: string }>;
  }>;
}
