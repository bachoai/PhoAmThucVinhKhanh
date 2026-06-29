import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { ArrowRight, Compass, MapPin, QrCode, Route as RouteIcon, Search, Sparkles, UserRound, Volume2 } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';
import { categoryApi } from '../api/categoryApi';
import { poiApi } from '../api/poiApi';
import { Categories } from '../components/common/Categories';
import { ErrorBox } from '../components/common/ErrorBox';
import { PoiCard } from '../components/common/PoiCard';
import { Spinner } from '../components/common/Spinner';
import { heroImage } from '../constants/heroImage';
import { getCopy } from '../i18n/copy';
import { useAppStore } from '../store/appStore';

export default function HomePage() {
  const navigate = useNavigate();
  const { lang } = useAppStore();
  const ui = getCopy(lang);
  const { data: pois = [], isLoading, isError } = useQuery({
    queryKey: ['pois', lang],
    queryFn: () => poiApi.list({ lang }),
  });
  const { data: categories = [] } = useQuery({
    queryKey: ['categories'],
    queryFn: categoryApi.list,
  });
  const [keyword, setKeyword] = useState('');

  return (
    <>
      <section className="shell pt-8 sm:pt-12">
        <div className="relative isolate overflow-hidden rounded-[2.5rem] bg-ink px-6 py-16 text-white sm:px-12 lg:py-24">
          <img
            src={heroImage}
            className="absolute inset-0 -z-10 h-full w-full object-cover opacity-35"
            alt="Món ăn Quận 4"
          />
          <div className="absolute inset-0 -z-10 bg-gradient-to-r from-slate-950 via-slate-950/75 to-transparent" />

          <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} className="max-w-2xl">
            <p className="section-kicker !text-white/70">{ui.home.heroKicker}</p>
            <h1 className="mt-4 font-serif text-4xl font-bold leading-tight sm:text-6xl">
              {ui.home.heroTitleLead} <span className="text-coral">{ui.home.heroTitleAccent}</span>
            </h1>
            <p className="mt-5 max-w-xl text-base leading-7 text-slate-200 sm:text-lg">
              {ui.home.heroDescription}
            </p>

            <div className="mt-8 flex flex-wrap gap-3">
              <Link className="btn-primary" to="/explore">
                {ui.home.heroExplore} <ArrowRight size={18} />
              </Link>
              <Link className="btn-secondary !border-white/20 !bg-white/10 !text-white" to="/qr">
                <QrCode size={18} />
                {ui.home.heroQr}
              </Link>
              <Link className="btn-secondary !border-white/20 !bg-white/10 !text-white" to="/tours">
                <RouteIcon size={18} />
                {ui.home.heroTours}
              </Link>
            </div>
          </motion.div>

          <form
            onSubmit={(event) => {
              event.preventDefault();
              navigate(`/explore${keyword ? `?q=${encodeURIComponent(keyword)}` : ''}`);
            }}
            className="mt-10 flex max-w-xl items-center gap-2 rounded-2xl bg-white p-2 shadow-soft"
          >
            <Search className="ml-2 text-slate-400" />
            <input
              value={keyword}
              onChange={(event) => setKeyword(event.target.value)}
              placeholder={ui.home.heroSearchPlaceholder}
              className="min-w-0 flex-1 bg-transparent py-2 text-slate-900 outline-none"
            />
            <button className="btn-primary shrink-0">
              {ui.common.search}
            </button>
          </form>
        </div>
      </section>

      <section className="shell py-14">
        <p className="section-kicker">{ui.home.categoryKicker}</p>
        <h2 className="mt-2 text-3xl font-bold sm:text-4xl">{ui.home.categoryTitle}</h2>
        <div className="mt-6">
          <Categories onSelect={(id) => navigate(`/explore${id ? `?category=${id}` : ''}`)} />
        </div>
      </section>

      <section className="shell">
        <div className="flex flex-wrap items-end justify-between gap-4">
          <div>
            <p className="section-kicker">{ui.home.featuredKicker}</p>
            <h2 className="mt-2 text-3xl font-bold sm:text-4xl">{ui.home.featuredTitle}</h2>
          </div>
          <div className="flex gap-3">
            <Link to="/explore" className="hidden font-bold text-coral sm:block">
              {ui.home.viewAll}
            </Link>
            <Link to="/tours" className="hidden font-bold text-teal sm:block">
              {ui.layout.publicTours}
            </Link>
          </div>
        </div>

        {isLoading ? (
          <Spinner />
        ) : isError ? (
          <ErrorBox />
        ) : (
          <div className="mt-7 grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
            {pois.slice(0, 6).map((poi) => (
              <PoiCard
                key={poi.id}
                poi={poi}
                category={categories.find((category) => category.id === poi.categoryId)?.name}
              />
            ))}
          </div>
        )}
      </section>

      <section className="shell py-20">
        <div className="grid gap-5 md:grid-cols-4">
          {[
            [Compass, ui.home.featureMapTitle, ui.home.featureMapText],
            [Volume2, ui.home.featureAudioTitle, ui.home.featureAudioText],
            [Sparkles, ui.home.featureI18nTitle, ui.home.featureI18nText],
            [QrCode, ui.home.featureQrTitle, ui.home.featureQrText],
          ].map(([Icon, title, text]) => {
            const CurrentIcon = Icon as typeof Compass;

            return (
              <div
                key={title as string}
                className="rounded-3xl border border-slate-200 bg-white p-7 dark:border-slate-800 dark:bg-slate-900"
              >
                <span className="grid h-12 w-12 place-items-center rounded-2xl bg-teal/10 text-teal">
                  <CurrentIcon />
                </span>
                <h3 className="mt-5 text-xl font-bold">{title as string}</h3>
                <p className="mt-2 leading-6 text-slate-500">{text as string}</p>
              </div>
            );
          })}
        </div>

        <div className="mt-8 grid gap-6 rounded-[2rem] bg-teal p-8 text-ink md:grid-cols-[1fr_auto_auto_auto] md:items-center">
          <div>
            <p className="section-kicker !text-ink/70">{ui.home.contractKicker}</p>
            <h2 className="mt-2 text-3xl font-bold">{ui.home.contractTitle}</h2>
          </div>
          <Link className="btn-secondary !border-ink !bg-ink !text-white" to="/map">
            {ui.home.mapCta} <MapPin size={18} />
          </Link>
          <Link className="btn-secondary !border-ink !bg-white/20 !text-ink" to="/qr">
            {ui.home.qrCta} <QrCode size={18} />
          </Link>
          <Link className="btn-secondary !border-ink !bg-white/20 !text-ink" to="/account">
            {ui.home.accountCta} <UserRound size={18} />
          </Link>
        </div>
      </section>
    </>
  );
}
