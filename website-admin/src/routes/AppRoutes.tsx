import { Navigate, Route, Routes } from 'react-router-dom';
import { ProtectedRoute } from '../components/common/ProtectedRoute';
import { AdminLayout } from '../components/layout/AdminLayout';
import { AnalyticsPage } from '../pages/AnalyticsPage';
import { AudioPage } from '../pages/AudioPage';
import { CategoryPage } from '../pages/CategoryPage';
import { DashboardPage } from '../pages/DashboardPage';
import { LocalizationPage } from '../pages/LocalizationPage';
import { LoginPage } from '../pages/LoginPage';
import { MapsPage } from '../pages/MapsPage';
import { NotFoundPage } from '../pages/NotFoundPage';
import { OwnerRegistrationPage } from '../pages/OwnerRegistrationPage';
import { OwnerSubmissionPage } from '../pages/OwnerSubmissionPage';
import { PoiCreatePage } from '../pages/PoiCreatePage';
import { PoiDetailPage } from '../pages/PoiDetailPage';
import { PoiEditPage } from '../pages/PoiEditPage';
import { PoiListPage } from '../pages/PoiListPage';
import { UserManagementPage } from '../pages/UserManagementPage';
import { useAuthStore } from '../store/authStore';

function IndexRedirect() {
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  return <Navigate to={isAuthenticated ? '/admin/dashboard' : '/login'} replace />;
}

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<IndexRedirect />} />
      <Route path="/login" element={<LoginPage />} />
      <Route element={<ProtectedRoute />}>
        <Route element={<AdminLayout />}>
          <Route path="/admin/dashboard" element={<DashboardPage />} />
          <Route path="/admin/categories" element={<CategoryPage />} />
          <Route path="/admin/pois" element={<PoiListPage />} />
          <Route path="/admin/pois/create" element={<PoiCreatePage />} />
          <Route path="/admin/pois/:id" element={<PoiDetailPage />} />
          <Route path="/admin/pois/:id/edit" element={<PoiEditPage />} />
          <Route path="/admin/owner-registrations" element={<OwnerRegistrationPage />} />
          <Route path="/admin/submissions" element={<OwnerSubmissionPage />} />
          <Route path="/admin/users" element={<UserManagementPage />} />
          <Route path="/admin/audio" element={<AudioPage />} />
          <Route path="/admin/localizations" element={<LocalizationPage />} />
          <Route path="/admin/analytics" element={<AnalyticsPage />} />
          <Route path="/admin/maps" element={<MapsPage />} />
        </Route>
      </Route>
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
