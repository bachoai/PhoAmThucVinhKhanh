import { useEffect, useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link, useSearchParams } from 'react-router-dom';
import { MapPin, Navigation } from 'lucide-react';
import { mapsApi } from '../api/mapsApi';
import { poiApi } from '../api/poiApi';
import { routeApi } from '../api/routeApi';
import { Categories } from '../components/common/Categories';
import { ErrorBox } from '../components/common/ErrorBox';
import { Spinner } from '../components/common/Spinner';
import { PoiMap } from '../components/map/PoiMap';
import { useAppStore } from '../store/appStore';
import { distance } from '../utils/analytics';

export default function MapPage() {
  const { lang, location, setLocation } = useAppStore();
  const [searchParams, setSearchParams] = useSearchParams();
  const [geoError, setGeoError] = useState('');
  const [isLocating, setIsLocating] = useState(false);
  const [categoryId, setCategoryId] = useState(searchParams.get('category') || '');
  const [priceRange, setPriceRange] = useState(searchParams.get('price') || '');
  const selectedPoiId = searchParams.get('poi') || '';
  const { data: pois = [], isLoading, isError, error } = useQuery({
    queryKey: ['map-pois', lang],
    queryFn: () => poiApi.list({ lang }),
  });
  const mapPackQuery = useQuery({
    queryKey: ['map-pack-manifest'],
    queryFn: mapsApi.getPackManifest,
    retry: false,
    staleTime: 300000,
  });

  useEffect(() => {
    setCategoryId(searchParams.get('category') || '');
    setPriceRange(searchParams.get('price') || '');
  }, [searchParams]);

  const filteredPois = useMemo(
    () =>
      pois.filter((poi) => {
        if (categoryId && poi.categoryId !== categoryId) {
          return false;
        }
        if (priceRange && poi.priceRange !== priceRange) {
          return false;
        }
        return true;
      }),
    [categoryId, pois, priceRange],
  );

  const selectedPoi = filteredPois.find((poi) => poi.id === selectedPoiId);
  const routeQuery = useQuery({
    queryKey: ['map-route', selectedPoiId, location?.lat, location?.lng],
    queryFn: () =>
      routeApi.between(
        { lat: location!.lat, lng: location!.lng },
        { lat: selectedPoi!.latitude, lng: selectedPoi!.longitude },
      ),
    enabled: Boolean(selectedPoi && location),
    staleTime: 30000,
    retry: 1,
  });

  useEffect(() => {
    if (!navigator.geolocation) {
      setGeoError('Trình duyệt này không hỗ trợ GPS.');
      return;
    }

    const syncLocation = (position: GeolocationPosition) => {
      setLocation({
        lat: position.coords.latitude,
        lng: position.coords.longitude,
      });
      setGeoError('');
      setIsLocating(false);
    };

    const showError = () => {
      setGeoError('Không lấy được vị trí hiện tại. Hãy bật GPS và thử lại.');
      setIsLocating(false);
    };

    setIsLocating(true);
    navigator.geolocation.getCurrentPosition(syncLocation, showError, {
      enableHighAccuracy: true,
      timeout: 15000,
      maximumAge: 15000,
    });

    const watchId = navigator.geolocation.watchPosition(syncLocation, () => undefined, {
      enableHighAccuracy: true,
      timeout: 20000,
      maximumAge: 15000,
    });

    return () => navigator.geolocation.clearWatch(watchId);
  }, [setLocation]);

  useEffect(() => {
    if (!selectedPoiId || selectedPoi || !filteredPois.length) {
      return;
    }

    const next = new URLSearchParams(searchParams);
    next.delete('poi');
    setSearchParams(next, { replace: true });
  }, [filteredPois, searchParams, selectedPoi, selectedPoiId, setSearchParams]);

  const updateFilters = (nextCategoryId: string, nextPriceRange: string) => {
    const next = new URLSearchParams(searchParams);
    if (nextCategoryId) {
      next.set('category', nextCategoryId);
    } else {
      next.delete('category');
    }
    if (nextPriceRange) {
      next.set('price', nextPriceRange);
    } else {
      next.delete('price');
    }
    setSearchParams(next, { replace: true });
  };

  const focusPoi = (poiId: string) => {
    const next = new URLSearchParams(searchParams);
    next.set('poi', poiId);
    setSearchParams(next, { replace: true });
  };

  const clearFocusPoi = () => {
    const next = new URLSearchParams(searchParams);
    next.delete('poi');
    setSearchParams(next, { replace: true });
  };

  const refreshLocation = () => {
    if (!navigator.geolocation) {
      setGeoError('Trình duyệt này không hỗ trợ GPS.');
      return;
    }

    setIsLocating(true);
    navigator.geolocation.getCurrentPosition(
      (position) => {
        setLocation({
          lat: position.coords.latitude,
          lng: position.coords.longitude,
        });
        setGeoError('');
        setIsLocating(false);
      },
      () => {
        setGeoError('Không lấy được vị trí hiện tại. Hãy bật GPS và thử lại.');
        setIsLocating(false);
      },
      {
        enableHighAccuracy: true,
        timeout: 15000,
        maximumAge: 0,
      },
    );
  };

  const durationText = routeQuery.data
    ? routeQuery.data.durationSeconds >= 3600
      ? `${Math.floor(routeQuery.data.durationSeconds / 3600)}h ${Math.round((routeQuery.data.durationSeconds % 3600) / 60)}m`
      : `${Math.max(1, Math.round(routeQuery.data.durationSeconds / 60))} phút`
    : null;

  return (
    <section className="shell py-12">
      <p className="section-kicker">ĐỊNH VỊ HƯƠNG VỊ</p>
      <h1 className="mt-2 text-4xl font-bold">Bản đồ ẩm thực</h1>
      <p className="mt-2 text-slate-500">Lấy vị trí thật của bạn, chọn một POI và xem đường đi ngay trên bản đồ.</p>

      <div className="mt-5 grid gap-3 rounded-[2rem] bg-white p-5 shadow-soft dark:bg-slate-900">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <p className="text-xs font-bold uppercase tracking-[0.2em] text-coral">Bộ lọc bản đồ</p>
            <p className="mt-1 text-sm text-slate-500">
              {mapPackQuery.data
                ? `Offline pack sẵn sàng: ${mapPackQuery.data.name} v${mapPackQuery.data.version}`
                : 'Đang dùng tile online cho local/demo. Có thể cấu hình MapTiler bằng VITE_MAPTILER_KEY.'}
            </p>
          </div>
          <div className="flex flex-wrap gap-2">
            <button type="button" className="btn-primary" onClick={refreshLocation} disabled={isLocating}>
              <Navigation size={17} />
              {isLocating ? 'Đang lấy GPS' : 'Tìm quanh tôi'}
            </button>
            <Link className="btn-secondary" to="/nearby">
              <MapPin size={17} />
              Mở Nearby
            </Link>
          </div>
        </div>

        <Categories selected={categoryId} onSelect={(id) => updateFilters(id || '', priceRange)} />

        <div className="flex flex-wrap gap-2">
          {['$', '$$', '$$$'].map((price) => (
            <button
              type="button"
              key={price}
              onClick={() => updateFilters(categoryId, priceRange === price ? '' : price)}
              className={`pill ${priceRange === price ? 'border-coral bg-orange-50 text-coral' : ''}`}
            >
              {price}
            </button>
          ))}
        </div>
      </div>

      <div className="mt-6 grid gap-4 rounded-[2rem] bg-white p-5 shadow-soft dark:bg-slate-900 md:grid-cols-[1.2fr_.8fr_auto] md:items-center">
        <div>
          <p className="text-xs font-bold uppercase tracking-[0.2em] text-coral">GPS thời gian thực</p>
          <p className="mt-2 text-sm text-slate-500">
            {location
              ? `Vị trí hiện tại: ${location.lat.toFixed(5)}, ${location.lng.toFixed(5)}`
              : isLocating
                ? 'Đang lấy vị trí hiện tại của bạn...'
                : 'Chưa lấy được vị trí. Bấm nút bên phải để bật định vị.'}
          </p>
          {geoError ? <p className="mt-2 text-sm text-rose-600">{geoError}</p> : null}
        </div>
        <div className="rounded-2xl bg-slate-50 p-4 dark:bg-slate-800">
          {selectedPoi ? (
            <>
              <p className="text-xs font-bold uppercase tracking-[0.2em] text-teal">POI đang chọn</p>
              <p className="mt-2 text-lg font-bold">{selectedPoi.name}</p>
              <p className="mt-1 text-sm text-slate-500">{selectedPoi.address}</p>
              {routeQuery.data ? (
                <div className="mt-3 flex flex-wrap gap-2 text-sm">
                  <span className="pill border-teal text-teal">{distance(routeQuery.data.distanceMeters)}</span>
                  <span className="pill">{durationText}</span>
                </div>
              ) : routeQuery.isLoading ? (
                <p className="mt-2 text-sm text-slate-500">Đang tìm đường đi trên mạng lưới đường phố...</p>
              ) : routeQuery.isError ? (
                <p className="mt-2 text-sm text-rose-600">{(routeQuery.error as Error).message}</p>
              ) : location ? (
                <p className="mt-2 text-sm text-slate-500">Đang sẵn sàng chỉ đường ngay khi có route.</p>
              ) : (
                <p className="mt-2 text-sm text-slate-500">Cần vị trí thật của bạn để vẽ đường đi.</p>
              )}
            </>
          ) : (
            <>
              <p className="text-xs font-bold uppercase tracking-[0.2em] text-teal">Chưa chọn điểm đến</p>
              <p className="mt-2 text-sm text-slate-500">Hãy bấm vào marker hoặc một dòng trong danh sách để xem route và số km.</p>
            </>
          )}
        </div>
        <div className="flex flex-wrap gap-2 md:justify-end">
          {selectedPoi ? (
            <button type="button" className="btn-secondary" onClick={clearFocusPoi}>
              Bỏ chọn điểm
            </button>
          ) : null}
        </div>
      </div>

      <div className="mt-7 grid gap-5 lg:grid-cols-[1.3fr_.7fr]">
        <PoiMap
          pois={filteredPois}
          userLocation={location}
          selectedPoiId={selectedPoiId || undefined}
          routeGeometry={routeQuery.data?.geometry}
          onSelectPoi={focusPoi}
        />
        <div className="max-h-[620px] space-y-3 overflow-y-auto pr-1 md:max-h-[700px] xl:max-h-[760px]">
          {isLoading ? (
            <Spinner />
          ) : isError ? (
            <ErrorBox text={(error as Error | undefined)?.message || 'Không tải được dữ liệu bản đồ.'} />
          ) : filteredPois.length ? (
            filteredPois.map((poi) => (
              <div
                key={poi.id}
                className={`rounded-2xl bg-white p-4 shadow-sm transition dark:bg-slate-900 ${selectedPoiId === poi.id ? 'ring-2 ring-teal' : 'hover:shadow-soft'}`}
              >
                <button type="button" className="block w-full text-left" onClick={() => focusPoi(poi.id)}>
                  <div className="flex flex-wrap items-center gap-2">
                    <p className="font-bold">{poi.name}</p>
                    <span className="pill border-coral text-coral">{poi.priceRange}</span>
                  </div>
                  <p className="mt-1 flex items-center gap-1 text-sm text-slate-500">
                    <MapPin size={14} />
                    {poi.address}
                  </p>
                </button>
                <div className="mt-3 flex flex-wrap gap-2">
                  <button type="button" className="pill border-teal text-teal" onClick={() => focusPoi(poi.id)}>
                    {selectedPoiId === poi.id ? 'Đang chỉ đường' : 'Chỉ đường trên bản đồ'}
                  </button>
                  <Link to={`/poi/${poi.id}`} className="pill">
                    Xem chi tiết
                  </Link>
                </div>
              </div>
            ))
          ) : (
            <div className="rounded-3xl bg-slate-100 p-8 text-center dark:bg-slate-900">
              <p className="font-bold">Không có POI phù hợp bộ lọc hiện tại</p>
            </div>
          )}
        </div>
      </div>
    </section>
  );
}
