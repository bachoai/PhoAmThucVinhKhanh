# Quan4CulinaryTourism.Api

Backend ASP.NET Core Web API cho dự án "Hệ Thống Du Lịch Ẩm Thực Quận 4, TP.HCM" dùng MongoDB, JWT Bearer và upload file local.

## Yêu cầu môi trường

- .NET SDK 10
- MongoDB local hoặc Docker
- Windows / PowerShell hoặc shell bất kỳ hỗ trợ `dotnet`

## Cài MongoDB nhanh

### Chạy local

- Cài MongoDB Community Server
- Bảo đảm MongoDB lắng nghe tại `mongodb://localhost:27017`

### Chạy bằng Docker

```bash
docker run -d --name quan4-mongo -p 27017:27017 mongo:8
```

## Cấu hình

File cấu hình chính: `appsettings.json`

- `MongoDbSettings`: chuỗi kết nối và tên database
- `JwtSettings`: khóa bí mật, issuer, audience, thời hạn token
- `UploadSettings`: thư mục upload và giới hạn kích thước file
- `DefaultAdmin`: tài khoản admin seed lúc startup
- `Cors.AllowedOrigins`: domain frontend local

## Chạy backend

```bash
dotnet restore
dotnet run
```

Mặc định Swagger mở tại:

- `http://localhost:5000/swagger`
- hoặc cổng random theo `launchSettings.json`

## Tài khoản admin mặc định

- Email: `admin@quan4tourism.local`
- Password: `Admin@123456`

## API chính

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `GET /api/v1/auth/me`
- `GET /api/v1/categories`
- `GET /api/v1/poi/load-all`
- `GET /api/v1/poi/nearby`
- `POST /api/v1/owner/submissions`
- `GET /api/v1/admin/dashboard/stats`
- `POST /api/v1/admin/pois`
- `POST /api/v1/admin/media/upload-image`
- `POST /api/v1/analytics/collect`
- `GET /api/health`

## Thứ tự test API

1. `GET /api/health`
2. `POST /api/v1/auth/login` bằng tài khoản admin
3. `GET /api/v1/categories`
4. `POST /api/v1/admin/pois`
5. `GET /api/v1/poi/load-all`
6. `GET /api/v1/poi/nearby`
7. `POST /api/v1/auth/register`
8. `POST /api/v1/auth/register-owner`
9. `POST /api/v1/owner/submissions`
10. `PUT /api/v1/admin/submissions/{id}/approve`

## Tương đương Spring Boot

- `Program.cs` tương tự `Application.java` + cấu hình bean/security
- `Controllers` tương tự `@RestController`
- `Services` tương tự `@Service`
- `Repositories` là lớp truy cập MongoDB trực tiếp, không dùng interface/JPA
- `appsettings.json` tương tự `application.yml` / `application.properties`
