import { getData } from './axiosClient';
import type { MapPackResponse } from '../types/responses';

export const mapsApi = {
  getPackManifest: () => getData<MapPackResponse | null>('/api/v1/maps/pack-manifest'),
};
