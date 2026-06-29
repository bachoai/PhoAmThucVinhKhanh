import { useEffect, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Search, Utensils } from 'lucide-react';
import { useLocation, useNavigate } from 'react-router-dom';
import { poiApi } from '../api/poiApi';
import { Categories } from '../components/common/Categories';
import { ErrorBox } from '../components/common/ErrorBox';
import { PoiCard } from '../components/common/PoiCard';
import { Spinner } from '../components/common/Spinner';
import { getCopy } from '../i18n/copy';
import { useAppStore } from '../store/appStore';
import { track } from '../utils/analytics';

function buildSearch(keyword: string, categoryId: string, priceRange: string) {
  const next = new URLSearchParams();
  if (keyword) {
    next.set('q', keyword);
  }
  if (categoryId) {
    next.set('category', categoryId);
  }
  if (priceRange) {
    next.set('price', priceRange);
  }
  return next.toString();
}

export default function ExplorePage() {
  const { lang } = useAppStore();
  const navigate = useNavigate();
  const location = useLocation();
  const ui = getCopy(lang);
  const params = new URLSearchParams(location.search);
  const [keyword, setKeyword] = useState(params.get('q') || '');
  const [categoryId, setCategoryId] = useState(params.get('category') || '');
  const [priceRange, setPriceRange] = useState(params.get('price') || '');

  useEffect(() => {
    const nextParams = new URLSearchParams(location.search);
    setKeyword(nextParams.get('q') || '');
    setCategoryId(nextParams.get('category') || '');
    setPriceRange(nextParams.get('price') || '');
  }, [location.search]);

  const query = useQuery({
    queryKey: ['explore', lang, keyword, categoryId, priceRange],
    queryFn: () => {
      const hasFilters = Boolean(keyword || categoryId || priceRange);
      const payload = {
        lang,
        keyword: keyword || undefined,
        categoryId: categoryId || undefined,
        priceRange: priceRange || undefined,
      };

      return hasFilters ? poiApi.search(payload) : poiApi.list({ lang });
    },
  });

  const pushFilters = (nextKeyword: string, nextCategoryId: string, nextPriceRange: string) => {
    const search = buildSearch(nextKeyword, nextCategoryId, nextPriceRange);
    navigate(`/explore${search ? `?${search}` : ''}`);
  };

  const runSearch = (event: React.FormEvent) => {
    event.preventDefault();
    try {
      track('search_executed', lang, undefined, {
        hasKeyword: Boolean(keyword),
        categoryId: categoryId || undefined,
        priceRange: priceRange || undefined,
      });
    } catch (error) {
      console.warn('Search analytics unavailable', error);
    }
    pushFilters(keyword, categoryId, priceRange);
  };

  return (
    <section className="shell py-12">
      <p className="section-kicker">{ui.explore.kicker}</p>
      <h1 className="mt-2 text-4xl font-bold">{ui.explore.title}</h1>
      <p className="mt-3 max-w-2xl text-slate-500">
        {ui.explore.subtitle}
      </p>

      <form
        onSubmit={runSearch}
        className="mt-7 flex rounded-2xl border border-slate-200 bg-white p-2 dark:border-slate-800 dark:bg-slate-900"
      >
        <Search className="m-2 text-slate-400" />
        <input
          value={keyword}
          onChange={(event) => setKeyword(event.target.value)}
          className="min-w-0 flex-1 bg-transparent outline-none"
          placeholder={ui.explore.searchPlaceholder}
        />
        <button className="btn-primary shrink-0">
          {ui.common.search}
        </button>
      </form>

      <div className="mt-6">
        <Categories
          selected={categoryId}
          onSelect={(id) => pushFilters(keyword, id || '', priceRange)}
        />
      </div>

      <div className="mt-4 flex gap-2">
        <span className="self-center text-sm font-semibold uppercase tracking-[0.2em] text-slate-500">
          {ui.explore.priceLabel}
        </span>
        {['$', '$$', '$$$'].map((price) => (
          <button
            type="button"
            key={price}
            onClick={() => pushFilters(keyword, categoryId, priceRange === price ? '' : price)}
            className={`pill ${priceRange === price ? 'border-coral bg-orange-50 text-coral' : ''}`}
          >
            {price}
          </button>
        ))}
      </div>

      {query.isLoading ? (
        <Spinner />
      ) : query.isError ? (
        <ErrorBox text={(query.error as Error).message} />
      ) : query.data?.length ? (
        <div className="mt-8 grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
          {query.data.map((poi) => (
            <PoiCard key={poi.id} poi={poi} />
          ))}
        </div>
      ) : (
        <div className="mt-8 rounded-3xl bg-slate-100 p-12 text-center dark:bg-slate-900">
          <Utensils className="mx-auto text-teal" />
          <h2 className="mt-3 text-xl font-bold">{ui.explore.emptyTitle}</h2>
          <p className="mt-1 text-slate-500">{ui.explore.emptyDescription}</p>
        </div>
      )}
    </section>
  );
}
