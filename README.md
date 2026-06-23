# Quan4 Culinary Tourism System

Monorepo gồm API ASP.NET Core, website quản trị, public website cho du khách và ứng dụng .NET MAUI.

## Chạy local

Yêu cầu: .NET SDK 10, Node.js 20+, MongoDB 8 và (nếu chạy mobile) workload .NET MAUI.

1. Đặt các biến trong `backend/Quan4CulinaryTourism.Api/.env.example` (PowerShell: `$env:JwtSettings__SecretKey=...`, `$env:DefaultAdmin__Password=...`). Không có secret hay mật khẩu mặc định trong source.
2. Chạy MongoDB: `docker compose up mongo -d`.
3. API: `cd backend/Quan4CulinaryTourism.Api; dotnet run` — Swagger chỉ có ở Development, API là `http://localhost:5163`.
4. Admin: `cd website-admin; npm ci; npm run dev` — `http://localhost:5173`.
5. Public web: `cd website-user; copy .env.example .env; npm ci; npm run dev` — `http://localhost:5174`.

API tự seed categories và 5 POI demo nếu database chưa có POI. Có thể quản lý dữ liệu qua admin sau đó.

## Production

`docker compose up --build` chạy MongoDB và API. Trước khi chạy, đặt `JWT_SECRET` và `DEFAULT_ADMIN_PASSWORD`; đồng thời cấu hình `ADMIN_ORIGIN` và `PUBLIC_ORIGIN` theo domain thật. Không dùng các giá trị development trong production.

## Kiểm tra

- API: `dotnet build backend/Quan4CulinaryTourism.Api/Quan4CulinaryTourism.Api.csproj`
- Admin: `npm run build` trong `website-admin`
- Public web: `npm run build` trong `website-user`
- Mobile Windows: `dotnet build -f net10.0-windows10.0.19041.0` trong `mobile/Quan4CulinaryTourism.Mobile`

CI build API và hai website trên mỗi push/pull request. Mobile cần runner Windows có MAUI workload, nên chưa chạy trong workflow Linux.
