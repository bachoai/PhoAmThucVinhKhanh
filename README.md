# Quan4 Culinary Tourism System

Monorepo gồm:

- API ASP.NET Core + MongoDB
- website admin React/Vite
- website public React/Vite
- mobile app .NET MAUI

## Thư mục chính

- `backend/Quan4CulinaryTourism.Api`: backend API
- `website-admin`: trang quản trị
- `website-user`: website cho du khách
- `mobile/Quan4CulinaryTourism.Mobile`: ứng dụng mobile

## Cổng và URL mặc định

| Thành phần | URL / kết nối |
| --- | --- |
| MongoDB | `mongodb://localhost:27017` |
| API | `http://localhost:5163` |
| Swagger (khi `dotnet run`) | `http://localhost:5163/swagger` |
| Admin web | `http://localhost:5173` |
| Public web | `http://localhost:5174` |

## Yêu cầu

- Docker Desktop hoặc Docker Engine + Docker Compose
- .NET SDK 10
- Node.js 20+
- .NET MAUI workload nếu cần chạy mobile

## Cách chạy dễ nhất cho local

Tất cả lệnh bên dưới được viết theo PowerShell và nên chạy từ repo root:

```powershell
cd D:\bac\PhoAmThucVinhKhanh
```

### 1. MongoDB

```powershell
docker compose up mongo -d
```

Lệnh này chỉ dùng để bật MongoDB local. Không cần set biến môi trường trước nữa.

### 2. API backend

```powershell
cd backend\Quan4CulinaryTourism.Api
dotnet restore
dotnet run
```

Ghi chú:

- `dotnet run` dùng profile `Development`, nên Swagger mở tại `http://localhost:5163/swagger`
- backend đã có dev secret và tài khoản admin seed sẵn trong `appsettings.Development.json`
- nếu database rỗng, backend sẽ seed role, admin, categories và dữ liệu demo

Tài khoản admin local:

- Email: `admin@quan4tourism.local`
- Password: `Admin@123456`

### 3. Website admin

Chạy một lần để tạo file env:

```powershell
Copy-Item website-admin\.env.example website-admin\.env
```

Sau đó:

```powershell
cd website-admin
npm ci
npm run dev
```

Admin web chạy tại `http://localhost:5173`.

### 4. Website public

Chạy một lần để tạo file env:

```powershell
Copy-Item website-user\.env.example website-user\.env
```

Sau đó:

```powershell
cd website-user
npm ci
npm run dev
```

Website public chạy tại `http://localhost:5174`.

## Chạy API bằng Docker

Nếu muốn chạy cả MongoDB và API bằng Docker:

```powershell
Copy-Item .env.example .env
docker compose up --build
```

File `.env.example` ở repo root cung cấp các biến cần thiết cho container API:

- `JwtSettings__SecretKey`
- `DefaultAdmin__Email`
- `DefaultAdmin__Password`
- `Cors__AllowedOrigins__0`
- `Cors__AllowedOrigins__1`

Lưu ý:

- `docker compose up mongo -d` không cần file `.env`
- `docker compose up --build` cần `.env` nếu bạn muốn API container khởi động thành công
- API trong Docker đang chạy với `ASPNETCORE_ENVIRONMENT=Production`, nên không có Swagger

## Mobile

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

Preset API trong mobile:

- Windows: `http://localhost:5163`
- Android Emulator: `http://10.0.2.2:5163`
- Máy thật: nhập LAN IP của máy đang chạy backend, ví dụ `http://192.168.1.50:5163`

## Lệnh kiểm tra

- API: `dotnet build backend/Quan4CulinaryTourism.Api/Quan4CulinaryTourism.Api.csproj`
- Admin: `cd website-admin; npm run build`
- Public web: `cd website-user; npm run build`
- Mobile Windows: `cd mobile/Quan4CulinaryTourism.Mobile; dotnet build -f net10.0-windows10.0.19041.0`

## Tài liệu từng app

- [Backend API](backend/Quan4CulinaryTourism.Api/README.md)
- [Admin website](website-admin/README.md)
- [Public website](website-user/README.md)
- [Mobile app](mobile/Quan4CulinaryTourism.Mobile/README.md)
