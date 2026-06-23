# Quan4 Food Stories

Public website React + TypeScript cho khách du lịch khám phá ẩm thực Quận 4. Công nghệ: Vite, Tailwind, TanStack Query, Zustand, Framer Motion, MapLibre và PWA.

## Run

```powershell
copy .env.example .env
npm ci
npm run dev
```

Website chạy ở `http://localhost:5174` để không xung đột admin (`5173`). `VITE_API_BASE_URL` mặc định là `http://localhost:5163`. URL ảnh/audio relative từ API được tự động ghép với base URL này.

Bản đồ dùng OpenStreetMap thật cho local/demo. Khi triển khai hoặc có lưu lượng đáng kể, tạo key MapTiler và đặt `VITE_MAPTILER_KEY` để dùng tile provider có SLA; không dùng tile công cộng OpenStreetMap cho production tải cao.

`npm run build` tạo bundle production và service worker PWA.
