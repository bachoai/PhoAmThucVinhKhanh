# Quan4 Culinary Tourism System

Monorepo gá»“m:

- Backend API ASP.NET Core + MongoDB
- Admin web React/Vite
- Public web React/Vite
- Mobile app .NET MAUI

## Cáº¥u trĂºc chĂ­nh

- `backend/Quan4CulinaryTourism.Api`: backend API
- `website-admin`: trang quáº£n trá»‹
- `website-user`: website public cho du khĂ¡ch
- `mobile/Quan4CulinaryTourism.Mobile`: á»©ng dá»¥ng mobile
- `docs/API_CONTRACT.md`: há»£p Ä‘á»“ng API vĂ  cĂ¡c flow chĂ­nh
- `scripts/create-clean-zip.ps1`: táº¡o file zip sáº¡ch trÆ°á»›c khi bĂ n giao/demo

## Cá»•ng máº·c Ä‘á»‹nh khi cháº¡y local

| ThĂ nh pháº§n | URL / káº¿t ná»‘i |
| --- | --- |
| MongoDB | `mongodb://localhost:27017` |
| API backend | `http://localhost:5163` |
| Swagger | `http://localhost:5163/swagger` |
| Admin web | `http://localhost:5173` |
| Public web | `http://localhost:5174` |

## YĂªu cáº§u mĂ´i trÆ°á»ng

- Docker Desktop hoáº·c Docker Engine + Docker Compose
- .NET SDK 10
- Node.js 20+
- .NET MAUI workload náº¿u cáº§n cháº¡y mobile

## Cháº¡y local

CĂ¡c lá»‡nh dÆ°á»›i Ä‘Ă¢y viáº¿t theo PowerShell vĂ  nĂªn cháº¡y tá»« repo root:

```powershell
cd D:\bac\PhoAmThucVinhKhanh
```

### 1. MongoDB

```powershell
docker compose up mongo -d
```

### 2. Backend API

```powershell
cd backend\Quan4CulinaryTourism.Api
dotnet restore
dotnet run
```

Ghi chĂº:

- Swagger chá»‰ báº­t trong `Development`.
- Backend tá»± load file `.env` trong thÆ° má»¥c backend náº¿u cĂ³.
- `appsettings.Development.json` Ä‘Ă£ cĂ³ cáº¥u hĂ¬nh local Ä‘á»ƒ cháº¡y nhanh.
- Náº¿u database trá»‘ng, backend sáº½ seed role, admin, category vĂ  dá»¯ liá»‡u demo.

TĂ i khoáº£n admin local máº·c Ä‘á»‹nh:

- Email: `admin@quan4tourism.local`
- Password: `Admin@123456`

### 3. Admin web

```powershell
Copy-Item website-admin\.env.example website-admin\.env
cd website-admin
npm ci
npm run dev
```

Admin web cháº¡y táº¡i `http://localhost:5173`.

### 4. Public web

```powershell
Copy-Item website-user\.env.example website-user\.env
cd website-user
npm ci
npm run dev
```

Public web cháº¡y táº¡i `http://localhost:5174`.

### 5. Mobile

Build Windows:

```powershell
cd mobile\Quan4CulinaryTourism.Mobile
dotnet build -f net10.0-windows10.0.19041.0
```

Build Android:

```powershell
cd mobile\Quan4CulinaryTourism.Mobile
dotnet build -f net10.0-android
```

API base URL nĂªn dĂ¹ng:

- Windows: `http://localhost:5163`
- Android Emulator: `http://10.0.2.2:5163`
- MĂ¡y tháº­t: LAN IP cá»§a mĂ¡y Ä‘ang cháº¡y backend, vĂ­ dá»¥ `http://192.168.1.50:5163`

## Cháº¡y backend báº±ng Docker

Náº¿u muá»‘n cháº¡y cáº£ MongoDB vĂ  API trong Docker:

```powershell
Copy-Item .env.example .env
docker compose up --build
```

Biáº¿n cáº§n cĂ³ trong `.env`:

- `JwtSettings__SecretKey`
- `DefaultAdmin__Email`
- `DefaultAdmin__Password`
- `Cors__AllowedOrigins__0`
- `Cors__AllowedOrigins__1`

LÆ°u Ă½:

- `docker compose up mongo -d` khĂ´ng cáº§n `.env`.
- `docker compose up --build` cáº§n `.env` Ä‘á»ƒ API container khá»Ÿi Ä‘á»™ng thĂ nh cĂ´ng.
- API container cháº¡y `Production`, nĂªn khĂ´ng cĂ³ Swagger.

## Deploy tĂ¡ch backend/admin/public

### Backend API

- Deploy API riĂªng.
- Cáº¥u hĂ¬nh `JwtSettings__SecretKey` báº±ng secret tháº­t, tá»‘i thiá»ƒu 32 kĂ½ tá»±.
- KhĂ´ng Ä‘á»ƒ `DefaultAdmin__Password` rá»—ng trong production náº¿u váº«n báº­t seed admin.
- Chá»‰ báº­t CORS cho Ä‘Ăºng domain admin vĂ  public.
- KhĂ´ng commit `.env`, secret, hoáº·c file upload demo.

### Admin web

- Deploy `website-admin` riĂªng, domain riĂªng.
- Chá»‰ cáº¥u hĂ¬nh `VITE_API_BASE_URL` trá» tá»›i domain API.
- KhĂ´ng hardcode production API URL trong source.

### Public web

- Deploy `website-user` riĂªng, domain riĂªng.
- Cáº¥u hĂ¬nh:
  - `VITE_API_BASE_URL`
  - `VITE_MAPTILER_KEY`
  - `VITE_OSRM_BASE_URL`
  - `VITE_OSRM_PROFILE`
- Backend `PublicSiteSettings:BaseUrl` pháº£i trá» Ä‘Ăºng domain public Ä‘á»ƒ QR/deep link má»Ÿ Ä‘Ăºng trang khĂ¡ch.

### CORS backend

Backend pháº£i thĂªm Ä‘Ăºng domain cá»§a:

- Admin web
- Public web

VĂ­ dá»¥ production:

```env
Cors__AllowedOrigins__0=https://admin.example.com
Cors__AllowedOrigins__1=https://www.example.com
```

## Build kiá»ƒm tra

### Backend

```powershell
dotnet build backend/Quan4CulinaryTourism.Api/Quan4CulinaryTourism.Api.csproj
```

### Admin web

```powershell
cd website-admin
Remove-Item -Recurse -Force node_modules, dist -ErrorAction SilentlyContinue
npm ci
npm run build
```

### Public web

```powershell
cd website-user
Remove-Item -Recurse -Force node_modules, dist -ErrorAction SilentlyContinue
npm ci
npm run build
```

## Tạo zip sạch trước khi bàn giao

Script này tự loại trừ:

- `.git/`
- `.codegraph/`
- `.codex/`
- `.agents/`
- `node_modules/`
- `dist/`
- `bin/`
- `obj/`
- `tmp/`
- `backend/_build_verify*/`
- `backend/**/_build_verify*/`
- `backend/Quan4CulinaryTourism.Api/wwwroot/uploads/`
- `.env`
- `.env.*` trừ `.env.example`

Ngoài ra, nếu file zip đầu ra nằm trong một thư mục con của repo, script sẽ tự bỏ qua toàn bộ thư mục đầu ra đó để không cuốn zip cũ hoặc zip mới tạo vào chính archive mới.

Chạy nhanh:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\create-clean-zip.ps1
```

Khuyến nghị khi bàn giao:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\create-clean-zip.ps1 -OutputPath .\release\PhoAmThucVinhKhanh-clean.zip
```

## Tài liệu từng app

- [Backend API](backend/Quan4CulinaryTourism.Api/README.md)
- [Admin website](website-admin/README.md)
- [Public website](website-user/README.md)
- [Mobile app](mobile/Quan4CulinaryTourism.Mobile/README.md)

