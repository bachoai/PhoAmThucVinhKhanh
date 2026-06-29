import { useEffect, useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { ArrowRight, LoaderCircle, LogOut, ShieldCheck } from 'lucide-react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { authApi } from '../api/authApi';
import { ErrorBox } from '../components/common/ErrorBox';
import { Field, TextArea, TextInput } from '../components/common/FormControls';
import { Spinner } from '../components/common/Spinner';
import { StatusPill } from '../components/common/StatusPill';
import { useAppStore } from '../store/appStore';
import { hasRole } from '../utils/auth';

type AccountLocationState = {
  ownerAccessMessage?: string;
};

export default function AccountPage() {
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const location = useLocation();
  const token = useAppStore((state) => state.token);
  const currentUser = useAppStore((state) => state.currentUser);
  const setCurrentUser = useAppStore((state) => state.setCurrentUser);
  const logout = useAppStore((state) => state.logout);
  const [ownerNotice, setOwnerNotice] = useState('');
  const [ownerForm, setOwnerForm] = useState({
    businessName: '',
    businessAddress: '',
    phoneNumber: currentUser?.phoneNumber || '',
    description: '',
  });
  const registerOwnerMutation = useMutation({
    mutationFn: authApi.registerOwner,
    onSuccess: async () => {
      const me = await authApi.me();
      setCurrentUser(me);
      queryClient.setQueryData(['auth-me', token], me);
      queryClient.setQueryData(['account-me', token], me);
      await queryClient.invalidateQueries({ queryKey: ['owner-dashboard'] });
      setOwnerNotice('Đã gửi yêu cầu đối tác thành công. Trạng thái tài khoản đã chuyển sang chờ duyệt.');
    },
  });
  const currentUserQuery = useQuery({
    queryKey: ['account-me', token],
    queryFn: authApi.me,
    enabled: Boolean(token),
    staleTime: 0,
    refetchOnMount: 'always',
    retry: false,
  });

  useEffect(() => {
    if (currentUserQuery.data) {
      setCurrentUser(currentUserQuery.data);
      setOwnerForm((current) => ({
        ...current,
        phoneNumber: currentUserQuery.data.phoneNumber || current.phoneNumber,
      }));
    }
  }, [currentUserQuery.data, setCurrentUser]);

  const redirectNotice = (location.state as AccountLocationState | null)?.ownerAccessMessage || '';
  const resolvedCurrentUser = currentUserQuery.data ?? currentUser;
  const canOpenOwnerWorkspace = hasRole(resolvedCurrentUser?.roles, 'Owner');
  const ownerApprovedWithoutRole =
    resolvedCurrentUser?.ownerStatus === 'approved' && !canOpenOwnerWorkspace;
  const isOwnerPending = resolvedCurrentUser?.ownerStatus === 'pending';
  const infoNotice = useMemo(() => ownerNotice || redirectNotice, [ownerNotice, redirectNotice]);

  if (currentUserQuery.isError) {
    return (
      <section className="shell py-12">
        <ErrorBox text={(currentUserQuery.error as Error).message} />
      </section>
    );
  }

  if (!resolvedCurrentUser) {
    return (
      <section className="shell py-12">
        <div className="rounded-[2rem] bg-white p-8 shadow-soft dark:bg-slate-900">
          <Spinner />
        </div>
      </section>
    );
  }

  return (
    <section className="shell py-12">
      <div className="grid gap-6 lg:grid-cols-[.95fr_1.05fr]">
        <div className="rounded-[2rem] bg-white p-8 shadow-soft dark:bg-slate-900">
          <p className="section-kicker">TÀI KHOẢN</p>
          <h1 className="mt-2 text-4xl font-bold">{resolvedCurrentUser.fullName}</h1>
          <p className="mt-2 text-slate-500">{resolvedCurrentUser.email}</p>

          <div className="mt-6 grid gap-3">
            <div className="rounded-2xl bg-slate-100 p-4 dark:bg-slate-800">
              <p className="text-xs font-bold uppercase tracking-[0.2em] text-coral">Vai trò</p>
              <p className="mt-1 font-semibold">{resolvedCurrentUser.roles.join(', ')}</p>
            </div>
            <div className="rounded-2xl bg-slate-100 p-4 dark:bg-slate-800">
              <p className="text-xs font-bold uppercase tracking-[0.2em] text-coral">Trạng thái đối tác</p>
              <div className="mt-2">
                <StatusPill status={resolvedCurrentUser.ownerStatus} />
              </div>
            </div>
          </div>

          {infoNotice ? (
            <div className="mt-6 rounded-2xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-900">
              {infoNotice}
            </div>
          ) : null}

          <div className="mt-6 flex flex-wrap gap-3">
            {canOpenOwnerWorkspace ? (
              <Link className="btn-primary" to="/owner">
                Không gian đối tác <ArrowRight size={18} />
              </Link>
            ) : null}
            <button
              className="btn-secondary"
              onClick={() => {
                logout();
                navigate('/', { replace: true });
              }}
            >
              <LogOut size={18} />
              Đăng xuất
            </button>
          </div>
        </div>

        <div className="rounded-[2rem] bg-white p-8 shadow-soft dark:bg-slate-900">
          {canOpenOwnerWorkspace ? (
            <>
              <p className="section-kicker">ĐỐI TÁC ĐÃ ĐƯỢC DUYỆT</p>
              <h2 className="mt-2 text-3xl font-bold">Tài khoản này đã là đối tác</h2>
              <p className="mt-2 text-slate-500">
                Admin đã xác nhận quyền đối tác cho tài khoản này. Bạn không cần gửi lại biểu mẫu đăng ký.
              </p>
              <div className="mt-6 rounded-2xl bg-emerald-50 p-5 text-emerald-800">
                <p className="font-bold">Trang quản lý đối tác đã sẵn sàng.</p>
                <p className="mt-1 text-sm">
                  Bạn có thể vào không gian đối tác để xem địa điểm, lượt xem, lượt nghe âm thanh và lượt quét QR.
                </p>
              </div>
              <div className="mt-6">
                <Link className="btn-primary" to="/owner">
                  Mở không gian đối tác <ArrowRight size={18} />
                </Link>
              </div>
            </>
          ) : ownerApprovedWithoutRole ? (
            <>
              <p className="section-kicker">ĐỐI TÁC CHƯA SẴN SÀNG</p>
              <h2 className="mt-2 text-3xl font-bold">Tài khoản đã được duyệt nhưng chưa có role Owner đầy đủ</h2>
              <p className="mt-2 text-slate-500">
                Hệ thống đang ghi nhận trạng thái đối tác đã được duyệt, nhưng tài khoản hiện tại chưa có role
                `Owner` để mở trang quản lý đối tác.
              </p>
              <div className="mt-6 rounded-2xl bg-amber-50 p-5 text-amber-900">
                <p className="font-bold">Nếu đây là tài khoản vừa được duyệt:</p>
                <p className="mt-1 text-sm">
                  Hãy đăng xuất đăng nhập lại. Nếu vẫn không vào được, admin cần kiểm tra role `Owner` trên tài khoản
                  này.
                </p>
              </div>
            </>
          ) : isOwnerPending ? (
            <>
              <p className="section-kicker">ĐANG CHỜ DUYỆT</p>
              <h2 className="mt-2 text-3xl font-bold">Yêu cầu đối tác của bạn đang được xem xét</h2>
              <p className="mt-2 text-slate-500">
                Bạn đã gửi yêu cầu đối tác thành công. Khi admin duyệt xong, tài khoản sẽ có role `Owner` và truy cập
                được trang quản lý đối tác.
              </p>
              <div className="mt-6 rounded-2xl bg-sky-50 p-5 text-sky-900">
                <p className="font-bold">Chưa cần gửi lại biểu mẫu.</p>
                <p className="mt-1 text-sm">Bạn chỉ cần quay lại trang này sau khi admin duyệt hoặc đăng nhập lại.</p>
              </div>
            </>
          ) : (
            <>
              <p className="section-kicker">ĐĂNG KÝ ĐỐI TÁC</p>
              <h2 className="mt-2 text-3xl font-bold">Gửi yêu cầu đối tác</h2>
              <p className="mt-2 text-slate-500">
                Trang này kết nối trực tiếp tới `POST /api/v1/auth/register-owner`. Sau khi được duyệt, tài khoản sẽ
                có role `Owner`.
              </p>

              <form
                className="mt-8 grid gap-4"
                onSubmit={(event) => {
                  event.preventDefault();
                  setOwnerNotice('');
                  registerOwnerMutation.mutate(ownerForm);
                }}
              >
                <Field label="Tên cơ sở">
                  <TextInput
                    value={ownerForm.businessName}
                    onChange={(event) => setOwnerForm((current) => ({ ...current, businessName: event.target.value }))}
                    required
                  />
                </Field>
                <Field label="Địa chỉ kinh doanh">
                  <TextInput
                    value={ownerForm.businessAddress}
                    onChange={(event) =>
                      setOwnerForm((current) => ({ ...current, businessAddress: event.target.value }))
                    }
                    required
                  />
                </Field>
                <Field label="Số điện thoại">
                  <TextInput
                    value={ownerForm.phoneNumber}
                    onChange={(event) => setOwnerForm((current) => ({ ...current, phoneNumber: event.target.value }))}
                    required
                  />
                </Field>
                <Field label="Mô tả">
                  <TextArea
                    value={ownerForm.description}
                    onChange={(event) => setOwnerForm((current) => ({ ...current, description: event.target.value }))}
                    placeholder="Mô tả ngắn về cơ sở của bạn"
                  />
                </Field>

                {registerOwnerMutation.error ? (
                  <div className="rounded-2xl bg-rose-50 px-4 py-3 text-sm text-rose-700">
                    {(registerOwnerMutation.error as Error).message}
                  </div>
                ) : null}

                <button className="btn-primary justify-center" disabled={registerOwnerMutation.isPending}>
                  {registerOwnerMutation.isPending ? (
                    <LoaderCircle className="animate-spin" size={18} />
                  ) : (
                    <ShieldCheck size={18} />
                  )}
                  Gửi yêu cầu đối tác
                </button>
              </form>
            </>
          )}
        </div>
      </div>
    </section>
  );
}
