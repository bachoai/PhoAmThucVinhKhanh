import { useEffect, useRef, useState } from 'react'
import maplibregl from 'maplibre-gl'
import type { Poi } from '../../types/responses'

const mapTilerKey = import.meta.env.VITE_MAPTILER_KEY as string | undefined
const mapStyle: string | maplibregl.StyleSpecification = mapTilerKey
  ? `https://api.maptiler.com/maps/streets-v2/style.json?key=${mapTilerKey}`
  : {
      version: 8,
      sources: {
        osm: {
          type: 'raster',
          tiles: ['https://tile.openstreetmap.org/{z}/{x}/{y}.png'],
          tileSize: 256,
          attribution: '© OpenStreetMap contributors'
        }
      },
      layers: [{ id: 'osm', type: 'raster', source: 'osm' }]
    }

function popupContent(poi: Poi) {
  const root = document.createElement('div')
  const title = document.createElement('strong')
  title.textContent = poi.name
  const address = document.createElement('span')
  address.textContent = poi.address
  const link = document.createElement('a')
  link.href = `/poi/${encodeURIComponent(poi.id)}`
  link.textContent = 'Xem chi tiết →'
  root.append(title, document.createElement('br'), address, document.createElement('br'), link)
  return root
}

export function PoiMap({ pois, userLocation }: { pois: Poi[]; userLocation?: { lat: number; lng: number } }) {
  const node = useRef<HTMLDivElement>(null)
  const [failed, setFailed] = useState(false)

  useEffect(() => {
    if (!node.current || !pois.length) return
    let map: maplibregl.Map | undefined
    try {
      map = new maplibregl.Map({ container: node.current, style: mapStyle, center: [106.706, 10.7578], zoom: 14 })
      map.on('error', () => setFailed(true))
      pois.filter(p => Number.isFinite(p.latitude) && Number.isFinite(p.longitude)).forEach(p =>
        new maplibregl.Marker({ color: '#FF6B35' }).setLngLat([p.longitude, p.latitude]).setPopup(new maplibregl.Popup({ offset: 22 }).setDOMContent(popupContent(p))).addTo(map!))
      if (userLocation) new maplibregl.Marker({ color: '#2EC4B6' }).setLngLat([userLocation.lng, userLocation.lat]).setPopup(new maplibregl.Popup().setText('Vị trí của bạn')).addTo(map)
    } catch { setFailed(true) }
    return () => map?.remove()
  }, [pois, userLocation])

  if (failed) return <div className="grid min-h-[430px] place-items-center rounded-[2rem] bg-slate-100 p-8 text-center text-slate-500 dark:bg-slate-900">Không thể tải nền bản đồ. Bạn vẫn có thể xem danh sách POI bên cạnh.</div>
  return <div ref={node} className="min-h-[430px] overflow-hidden rounded-[2rem] bg-slate-200 dark:bg-slate-800" aria-label="Bản đồ POI Quận 4" />
}
