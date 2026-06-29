import { getData } from './axiosClient';
import type {
  AudioLanguageResponse,
  AudioPackManifestResponse,
  PoiAudio,
} from '../types/responses';

const getPoiAudio = (id: string, lang: string) =>
  getData<PoiAudio | null>(`/api/v1/poi/${id}/audio`, { lang });

export const audioApi = {
  getLanguages: () => getData<AudioLanguageResponse[]>('/api/v1/audio/languages'),
  getPackManifest: () => getData<AudioPackManifestResponse>('/api/v1/audio/pack-manifest'),
  getPoiAudio,
  byPoi: getPoiAudio,
};
