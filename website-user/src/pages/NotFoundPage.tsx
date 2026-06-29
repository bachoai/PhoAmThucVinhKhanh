import { Link } from 'react-router-dom';

export default function NotFoundPage() {
  return (
    <section className="shell grid min-h-[55vh] place-items-center text-center">
      <div>
        <p className="text-7xl font-bold text-coral">404</p>
        <h1 className="mt-3 text-2xl font-bold">Không tìm thấy trang này</h1>
        <Link className="btn-primary mt-6" to="/">
          Ve trang chu
        </Link>
      </div>
    </section>
  );
}
