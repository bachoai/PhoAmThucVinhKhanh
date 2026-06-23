export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface ErrorResponse {
  success: false;
  message: string;
  errors: string[];
}

export interface PagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface CoordinateRequest {
  latitude: number;
  longitude: number;
}

export interface OpeningHour {
  dayOfWeek: string;
  openTime: string;
  closeTime: string;
  isClosed: boolean;
}

export interface PoiImage {
  url: string;
  caption?: string | null;
  isThumbnail: boolean;
}

export interface ContactInfo {
  phone?: string | null;
  email?: string | null;
  facebookUrl?: string | null;
  websiteUrl?: string | null;
}
