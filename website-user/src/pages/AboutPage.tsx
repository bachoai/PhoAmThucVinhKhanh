export default function AboutPage() {
  return (
    <section className="shell py-12">
      <p className="section-kicker">CÂU CHUYỆN DỰ ÁN</p>
      <h1 className="mt-2 max-w-3xl text-4xl font-bold leading-tight">
        Một người bạn đồng hành cho hành trình ăn ngon ở Quận 4.
      </h1>

      <div className="mt-10 grid gap-8 lg:grid-cols-2">
        <div className="rounded-[2rem] bg-orange-100 p-8 dark:bg-orange-500/10">
          <h2 className="text-2xl font-bold">Mục tiêu</h2>
          <p className="mt-4 leading-7 text-slate-600 dark:text-slate-300">
            Hệ thống giúp du khách tìm, nghe và cảm nhận những điểm ẩm thực địa phương một cách trực quan qua web, mobile và bản đồ thông minh.
          </p>
        </div>

        <div className="rounded-[2rem] bg-teal/10 p-8">
          <h2 className="text-2xl font-bold">Có gì trong trải nghiệm?</h2>
          <ul className="mt-4 grid gap-3 text-slate-600 dark:text-slate-300">
            {[
              'POI ẩm thực được tuyển chọn',
              'GPS và gợi ý gần bạn',
              'Audio thuyết minh đa ngôn ngữ',
              'Luồng quét QR và mở liên kết sâu',
              'Tour công khai và không gian đối tác',
            ].map((item) => (
              <li key={item} className="flex gap-2">
                <span className="text-teal">*</span>
                {item}
              </li>
            ))}
          </ul>
        </div>
      </div>
    </section>
  );
}
