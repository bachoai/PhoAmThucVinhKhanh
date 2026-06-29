import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { LoaderCircle, UserRound } from 'lucide-react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { authApi } from '../api/authApi';
import { Field, TextInput } from '../components/common/FormControls';
import { useAppStore } from '../store/appStore';
import type { LoginRequest } from '../types/requests';

export default function LoginPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const setAuth = useAppStore((state) => state.setAuth);
  const [form, setForm] = useState<LoginRequest>({ email: '', password: '' });
  const mutation = useMutation({
    mutationFn: authApi.login,
    onSuccess: (response) => {
      setAuth(response);
      navigate(searchParams.get('next') || '/account', { replace: true });
    },
  });

  const reason = searchParams.get('reason');

  return (
    <section className="shell py-12">
      <div className="mx-auto max-w-xl rounded-[2rem] bg-white p-8 shadow-soft dark:bg-slate-900">
        <p className="section-kicker">AUTH</p>
        <h1 className="mt-2 text-4xl font-bold">Đăng nhập</h1>
        <p className="mt-2 text-slate-500">Sử dụng API `auth/login` và `auth/me` từ hệ thống backend.</p>

        {reason === 'session-expired' ? (
          <div className="mt-6 rounded-2xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-900">
            Phiên đăng nhập đã hết hạn. Hãy đăng nhập lại để tiếp tục.
          </div>
        ) : null}

        <form
          className="mt-8 grid gap-4"
          onSubmit={(event) => {
            event.preventDefault();
            mutation.mutate(form);
          }}
        >
          <Field label="Email">
            <TextInput
              type="email"
              value={form.email}
              onChange={(event) => setForm((current) => ({ ...current, email: event.target.value }))}
              required
            />
          </Field>
          <Field label="Mật khẩu">
            <TextInput
              type="password"
              value={form.password}
              onChange={(event) => setForm((current) => ({ ...current, password: event.target.value }))}
              required
            />
          </Field>

          {mutation.error ? (
            <div className="rounded-2xl bg-rose-50 px-4 py-3 text-sm text-rose-700">
              {(mutation.error as Error).message}
            </div>
          ) : null}

          <button className="btn-primary justify-center" disabled={mutation.isPending}>
            {mutation.isPending ? <LoaderCircle className="animate-spin" size={18} /> : <UserRound size={18} />}
            Đăng nhập
          </button>
        </form>

        <p className="mt-5 text-sm text-slate-500">
          Chưa có tài khoản?{' '}
          <Link to="/register" className="font-bold text-coral">
            Đăng ký ngay
          </Link>
        </p>
      </div>
    </section>
  );
}
