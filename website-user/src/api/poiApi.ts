import { getData } from './axiosClient'
import type { NearbyPoi, Poi, PoiDetail } from '../types/responses'
export const poiApi = { list: (params: object) => getData<Poi[]>('/api/v1/poi/load-all', params), detail: (id: string, lang: string) => getData<PoiDetail>(`/api/v1/poi/${id}`, { lang }), nearby: (params: object) => getData<NearbyPoi[]>('/api/v1/poi/nearby', params) }
