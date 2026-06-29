import { Suspense, lazy } from 'react';
import type { ReactNode } from 'react';
import { AnimatePresence } from 'framer-motion';
import { Route, Routes } from 'react-router-dom';
import { Spinner } from '../components/common/Spinner';
import { PublicLayout } from '../components/layout/PublicLayout';
import AboutPage from '../pages/AboutPage';
import ExplorePage from '../pages/ExplorePage';
import HomePage from '../pages/HomePage';
import LoginPage from '../pages/LoginPage';
import NearbyPage from '../pages/NearbyPage';
import NotFoundPage from '../pages/NotFoundPage';
import PoiDetailPage from '../pages/PoiDetailPage';
import RegisterPage from '../pages/RegisterPage';
import { RequireAuth, RequireOwner } from './guards';
import { RouteErrorBoundary } from './RouteErrorBoundary';

const MapPage = lazy(() => import('../pages/MapPage'));
const ToursPage = lazy(() => import('../pages/ToursPage'));
const QrPage = lazy(() => import('../pages/QrPage'));
const AccountPage = lazy(() => import('../pages/AccountPage'));
const OwnerPage = lazy(() => import('../pages/OwnerPage'));

function LazyPage({ children }: { children: ReactNode }) {
  return (
    <Suspense
      fallback={
        <section className="shell py-12">
          <Spinner />
        </section>
      }
    >
      {children}
    </Suspense>
  );
}

export function AppRoutes() {
  return (
    <AnimatePresence mode="wait">
      <Routes>
        <Route element={<PublicLayout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/explore" element={<ExplorePage />} />
          <Route path="/poi/:id" element={<PoiDetailPage />} />
          <Route
            path="/nearby"
            element={
              <RouteErrorBoundary fallbackText="Trang Gần tôi vừa gặp lỗi hiển thị. Hãy tải lại trang hoặc thử lấy vị trí lại.">
                <NearbyPage />
              </RouteErrorBoundary>
            }
          />
          <Route
            path="/map"
            element={
              <LazyPage>
                <MapPage />
              </LazyPage>
            }
          />
          <Route
            path="/tours"
            element={
              <LazyPage>
                <ToursPage />
              </LazyPage>
            }
          />
          <Route
            path="/qr"
            element={
              <LazyPage>
                <QrPage />
              </LazyPage>
            }
          />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route
            path="/account"
            element={
              <RequireAuth>
                <LazyPage>
                  <AccountPage />
                </LazyPage>
              </RequireAuth>
            }
          />
          <Route
            path="/owner"
            element={
              <RequireAuth>
                <RequireOwner>
                  <LazyPage>
                    <OwnerPage />
                  </LazyPage>
                </RequireOwner>
              </RequireAuth>
            }
          />
          <Route path="/about" element={<AboutPage />} />
          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </AnimatePresence>
  );
}
