import axiosClient from './axiosClient';
import type { MapPackResponse } from '../types/responses';

export const mapsApi = {
  getPackManifest: () => axiosClient.get<never, MapPackResponse | null>('/api/v1/maps/pack-manifest'),
};
