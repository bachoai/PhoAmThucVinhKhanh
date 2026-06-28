# Quan4 Web Admin

Admin frontend cho `Quan4 Culinary Tourism System`.

## Stack

- React 19
- TypeScript
- Vite
- Ant Design v5
- Zustand
- TanStack Query
- Framer Motion
- Recharts
- Lucide React

## Environment

Tạo `.env` từ file mẫu:

```powershell
Copy-Item .env.example .env
```

`src/utils/media.ts` sẽ tự chuẩn hóa media URL:

- `/uploads/...` sẽ được nối với `VITE_API_BASE_URL`
- `http/https`, `blob`, `data` được giữ nguyên
- URL rỗng dùng placeholder nội bộ cho ảnh demo

## Run

```powershell
npm ci
npm run dev
```

Mặc định:

- Admin UI: `http://localhost:5173`
- Backend: `http://localhost:5163`

## Build

```bash
npm run build
```

## Demo UI cập nhật

- Concept đồng bộ theo màu:
  - Primary: `#FF6B35`
  - Secondary: `#2EC4B6`
- Light/dark mode rõ chữ cho card, table, modal, input, chart
- Dashboard có stat card gradient nhẹ, hover motion, quick actions thật
- POI list có fallback ảnh, badge trạng thái/price/category, empty state
- POI detail có hero image, audio card, contact/opening hours/gallery
- Owner registrations và submissions có modal chi tiết + confirm approve/reject
- Analytics có tooltip dễ đọc, empty state, card mô tả dữ liệu

## Main Routes

- `/login`
- `/admin/dashboard`
- `/admin/categories`
- `/admin/pois`
- `/admin/pois/create`
- `/admin/pois/:id`
- `/admin/pois/:id/edit`
- `/admin/owner-registrations`
- `/admin/submissions`
- `/admin/users`
- `/admin/audio`
- `/admin/localizations`
- `/admin/analytics`
- `/admin/maps`

## Default Admin

- Email: `admin@quan4tourism.local`
- Password: `Admin@123456`

## Notes

- Route và API hiện tại được giữ nguyên.
- Theme vẫn lưu bằng `localStorage` qua Zustand persist.
- Nếu backend trả media relative path, UI sẽ render đúng host qua `normalizeMediaUrl`.
