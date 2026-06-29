import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { LoaderCircle, ShieldCheck } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { authApi } from '../api/authApi';
import { Field, TextInput } from '../components/common/FormControls';
import { useAppStore } from '../store/appStore';
import type { RegisterRequest } from '../types/requests';

export default function RegisterPage() {
  const navigate = useNavigate();
  const setAuth = useAppStore((state) => state.setAuth);
  const [form, setForm] = useState<RegisterRequest>({
    fullName: '',
    email: '',
    password: '',
    phoneNumber: '',
  });
  const mutation = useMutation({
    mutationFn: authApi.register,
    onSuccess: (response) => {
      setAuth(response);
      navigate('/account', { replace: true });
    },
  });

  return (
    <section className="shell py-12">
      <div className="mx-auto max-w-2xl rounded-[2rem] bg-white p-8 shadow-soft dark:bg-slate-900">
        <p className="section-kicker">AUTH</p>
        <h1 className="mt-2 text-4xl font-bold">Tạo tài khoản</h1>
        <p className="mt-2 text-slate-500">
          Website public gọi `POST /api/v1/auth/register` và sẽ tự đăng nhập ngay sau khi tạo tài khoản thành công.
        </p>

        <form
          className="mt-8 grid gap-4 md:grid-cols-2"
          onSubmit={(event) => {
            event.preventDefault();
            mutation.mutate(form);
          }}
        >
          <Field label="Họ và tên">
            <TextInput
              value={form.fullName}
              onChange={(event) => setForm((current) => ({ ...current, fullName: event.target.value }))}
              required
            />
          </Field>
          <Field label="Số điện thoại">
            <TextInput
              value={form.phoneNumber}
              onChange={(event) => setForm((current) => ({ ...current, phoneNumber: event.target.value }))}
            />
          </Field>
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
              minLength={6}
            />
          </Field>

          {mutation.error ? (
            <div className="rounded-2xl bg-rose-50 px-4 py-3 text-sm text-rose-700 md:col-span-2">
              {(mutation.error as Error).message}
            </div>
          ) : null}

          <button className="btn-primary justify-center md:col-span-2" disabled={mutation.isPending}>
            {mutation.isPending ? <LoaderCircle className="animate-spin" size={18} /> : <ShieldCheck size={18} />}
            Tạo tài khoản và đăng nhập
          </button>
        </form>
      </div>
    </section>
  );
}
