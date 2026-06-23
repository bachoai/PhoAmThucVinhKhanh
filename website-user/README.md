# Quan4 Food Stories

Public website React + TypeScript cho khách du lịch khám phá ẩm thực Quận 4. Công nghệ: Vite, Tailwind, TanStack Query, Zustand, Framer Motion, MapLibre và PWA.

## Run

```powershell
copy .env.example .env
npm ci
npm run dev
```

Website chạy ở `http://localhost:5174` để không xung đột admin (`5173`). `VITE_API_BASE_URL` mặc định là `http://localhost:5163`. URL ảnh/audio relative từ API được tự động ghép với base URL này.

`npm run build` tạo bundle production và service worker PWA.
