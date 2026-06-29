import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Navigation } from 'lucide-react';
import { poiApi } from '../api/poiApi';
import { ErrorBox } from '../components/common/ErrorBox';
import { PoiCard } from '../components/common/PoiCard';
import { Spinner } from '../components/common/Spinner';
import { useAppStore } from '../store/appStore';
import { distance, track } from '../utils/analytics';

export default function NearbyPage() {
  const { lang, setLocation } = useAppStore();
  const [radius, setRadius] = useState(3000);
  const [coords, setCoords] = useState<{ lat: number; lng: number }>();
  const [locationError, setLocationError] = useState('');
  const hasValidCoords = Boolean(coords && Number.isFinite(coords.lat) && Number.isFinite(coords.lng));

  const query = useQuery({
    queryKey: ['nearby', coords, radius, lang],
    queryFn: async () => {
      if (!coords || !Number.isFinite(coords.lat) || !Number.isFinite(coords.lng)) {
        return [];
      }

      const data = await poiApi.nearby({ lat: coords.lat, lng: coords.lng, radius, limit: 20, lang });
      return Array.isArray(data) ? data : [];
    },
    enabled: hasValidCoords,
    retry: 1,
  });

  const nearbyPois = Array.isArray(query.data) ? query.data : [];

  const safeTrackNearby = () => {
    try {
      track('nearby_requested', lang, undefined, { radius });
    } catch (error) {
      console.warn('Nearby analytics unavailable', error);
    }
  };

  const formatNearbyDistance = (meters?: number) => {
    if (typeof meters !== 'number' || !Number.isFinite(meters)) {
      return 'Đang cập nhật';
    }

    return distance(meters);
  };

  const locate = (fallback = false) => {
    const setCurrentLocation = (lat: number, lng: number) => {
      setCoords({ lat, lng });
      setLocation({ lat, lng });
      setLocationError('');
      safeTrackNearby();
    };

    if (fallback) {
      setCurrentLocation(10.7578, 106.706);
      return;
    }

    if (!navigator.geolocation) {
      setLocationError('Trình duyệt này không hỗ trợ định vị. Hãy dùng vị trí mặc định Quận 4.');
      return;
    }

    navigator.geolocation.getCurrentPosition(
      (position) => setCurrentLocation(position.coords.latitude, position.coords.longitude),
      () => setLocationError('Không lấy được GPS. Bạn có thể thử lại hoặc dùng vị trí mặc định Quận 4.'),
      { enableHighAccuracy: true, timeout: 10000 },
    );
  };

  return (
    <section className="shell py-12">
      <p className="section-kicker">GỢI Ý QUANH BẠN</p>
      <h1 className="mt-2 text-4xl font-bold">Tìm quán gần tôi</h1>
      <p className="mt-3 max-w-xl text-slate-500">
        Cho phép trình duyệt sử dụng vị trí để tìm những hương vị đáng thử gần bạn nhất.
      </p>

      <div className="mt-7 rounded-3xl bg-ink p-7 text-white">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="font-bold">Bán kính tìm kiếm</p>
            <div className="mt-3 flex gap-2">
              {[1000, 3000, 5000, 10000].map((item) => (
                <button
                  type="button"
                  key={item}
                  onClick={() => setRadius(item)}
                  className={`rounded-full px-3 py-1.5 text-sm ${radius === item ? 'bg-coral' : 'bg-white/10'}`}
                >
                  {item / 1000} km
                </button>
              ))}
            </div>
          </div>

          <div className="flex flex-wrap gap-2">
            <button type="button" onClick={() => locate()} className="btn-primary">
              <Navigation size={17} />
              Lấy vị trí hiện tại
            </button>
            <button
              type="button"
              onClick={() => locate(true)}
              className="btn-secondary !border-white/20 !bg-transparent !text-white"
            >
              Dùng vị trí Quận 4
            </button>
          </div>
        </div>
        {locationError ? <p className="mt-4 text-sm text-amber-200">{locationError}</p> : null}
      </div>

      {query.isLoading ? (
        <Spinner />
      ) : query.isError ? (
        <ErrorBox text="Không tìm được địa điểm gần đây. Thử lại sau." />
      ) : nearbyPois.length ? (
        <div className="mt-8 grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
          {nearbyPois.map((poi) => (
            <div key={poi.id} className="relative">
              <PoiCard poi={poi} />
              <span className="absolute right-5 top-4 rounded-full bg-ink px-2 py-1 text-xs font-bold text-white">
                {formatNearbyDistance(poi.distanceMeters)}
              </span>
            </div>
          ))}
        </div>
      ) : hasValidCoords ? (
        <div className="mt-8 rounded-3xl bg-slate-100 p-12 text-center dark:bg-slate-900">
          <Navigation className="mx-auto text-teal" />
          <h2 className="mt-3 text-xl font-bold">Chưa có địa điểm phù hợp</h2>
          <p className="mt-1 text-slate-500">Hãy tăng bán kính tìm kiếm hoặc thử lại với vị trí khác.</p>
        </div>
      ) : null}
    </section>
  );
}
