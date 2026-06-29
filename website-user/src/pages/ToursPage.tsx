import { useQuery } from '@tanstack/react-query';
import { Clock3, Route as RouteIcon } from 'lucide-react';
import { Link } from 'react-router-dom';
import { poiApi } from '../api/poiApi';
import { tourApi } from '../api/tourApi';
import { ErrorBox } from '../components/common/ErrorBox';
import { Spinner } from '../components/common/Spinner';
import { heroImage } from '../constants/heroImage';
import { useAppStore } from '../store/appStore';
import type { Poi, TourResponse } from '../types/responses';
import { normalizeMediaUrl, poiImage } from '../utils/media';

function TourCard({
  tour,
  poiLookup,
}: {
  tour: TourResponse;
  poiLookup: Record<string, Poi>;
}) {
  const cover = tour.coverImageUrl
    ? normalizeMediaUrl(tour.coverImageUrl)
    : poiLookup[tour.stops[0]?.poiId]
      ? poiImage(poiLookup[tour.stops[0].poiId])
      : heroImage;

  return (
    <article className="overflow-hidden rounded-[2rem] bg-white shadow-soft dark:bg-slate-900">
      <img src={cover} alt={tour.title} className="h-56 w-full object-cover" />
      <div className="p-6">
        <div className="flex flex-wrap items-center gap-2">
          <span className="pill border-coral text-coral">{tour.lang.toUpperCase()}</span>
          <span className="pill">{tour.estimatedDurationMinutes} phút</span>
          <span className="pill">{tour.stops.length} điểm dừng</span>
        </div>

        <h2 className="mt-4 text-2xl font-bold">{tour.title}</h2>
        <p className="mt-2 leading-7 text-slate-600 dark:text-slate-300">{tour.description}</p>

        <div className="mt-5 space-y-3">
          {tour.stops
            .slice()
            .sort((left, right) => left.order - right.order)
            .map((stop) => {
              const poi = poiLookup[stop.poiId];
              return (
                <div
                  key={`${tour.id}-${stop.poiId}-${stop.order}`}
                  className="rounded-2xl border border-slate-200 p-4 dark:border-slate-800"
                >
                  <div className="flex items-center justify-between gap-4">
                    <div>
                      <p className="text-xs font-bold uppercase tracking-[0.2em] text-coral">Điểm dừng {stop.order + 1}</p>
                      <h3 className="mt-1 font-bold">{stop.title || poi?.name || stop.poiId}</h3>
                      <p className="mt-1 text-sm text-slate-500">{poi?.address || 'POI sẽ được tải khi bạn bấm vào'}</p>
                    </div>
                    <span className="pill">
                      <Clock3 size={14} className="mr-1 inline" />
                      {stop.estimatedStayMinutes} phút
                    </span>
                  </div>

                  <div className="mt-3">
                    <Link className="text-sm font-bold text-coral" to={`/poi/${stop.poiId}`}>
                      Mở chi tiết POI
                    </Link>
                  </div>
                </div>
              );
            })}
        </div>
      </div>
    </article>
  );
}

export default function ToursPage() {
  const { lang } = useAppStore();
  const toursQuery = useQuery({
    queryKey: ['public-tours', lang],
    queryFn: () => tourApi.list(lang),
  });
  const poisQuery = useQuery({
    queryKey: ['tour-pois', lang],
    queryFn: () => poiApi.list({ lang }),
  });

  const poiLookup = Object.fromEntries((poisQuery.data || []).map((poi) => [poi.id, poi])) as Record<string, Poi>;

  return (
    <section className="shell py-12">
      <p className="section-kicker">TOUR CÔNG KHAI</p>
      <h1 className="mt-2 text-4xl font-bold">Lịch trình ẩm thực công khai</h1>
      <p className="mt-2 max-w-2xl text-slate-500">
        Backend đã có endpoint tour công khai, nên website này hiện đã mở sẵn luồng xem tour và các điểm dừng.
      </p>

      {toursQuery.isLoading || poisQuery.isLoading ? (
        <Spinner />
      ) : toursQuery.isError ? (
        <ErrorBox text={(toursQuery.error as Error).message} />
      ) : toursQuery.data?.length ? (
        <div className="mt-8 grid gap-6 lg:grid-cols-2">
          {toursQuery.data.map((tour) => (
            <TourCard key={tour.id} tour={tour} poiLookup={poiLookup} />
          ))}
        </div>
      ) : (
        <div className="mt-8 rounded-3xl bg-slate-100 p-12 text-center dark:bg-slate-900">
          <RouteIcon className="mx-auto text-teal" />
          <h2 className="mt-3 text-xl font-bold">Chưa có tour công khai</h2>
          <p className="mt-1 text-slate-500">Admin có thể tạo tour mới trong trang quản trị.</p>
        </div>
      )}
    </section>
  );
}
