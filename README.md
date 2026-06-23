# Quan4 Culinary Tourism System

Monorepo cho hệ thống du lịch ẩm thực Quận 4 gồm:

- `backend/Quan4CulinaryTourism.Api`: ASP.NET Core Web API
- `website-admin`: React + Vite admin client
- `mobile/Quan4CulinaryTourism.Mobile`: .NET MAUI mobile client
- `website-user`: hiện mới có file yêu cầu, chưa có source app runnable

## 1. Yêu cầu môi trường

### Bắt buộc

- Git
- MongoDB chạy tại `mongodb://localhost:27017`
- .NET SDK 10
- Node.js 20+ và npm

### Nên có thêm nếu chạy mobile

- Visual Studio 2022 / 2026 với workload `.NET MAUI`
- Android SDK + Android Emulator

## 2. Cấu trúc và cổng local

- Backend API: `http://localhost:5163`
- Swagger: `http://localhost:5163/swagger`
- Admin web dev server: `http://localhost:5173`
- Mobile Windows: gọi backend qua `http://localhost:5163`
- Mobile Android Emulator: gọi backend qua `http://10.0.2.2:5163`

## 3. Khởi động MongoDB

Nếu đã cài MongoDB local, chỉ cần bảo đảm service đang chạy.

Nếu muốn chạy bằng Docker:

```bash
docker run -d --name quan4-mongo -p 27017:27017 mongo:8
```

Kiểm tra nhanh:

```bash
docker ps
```

## 4. Chạy backend

Mở terminal tại:

```powershell
D:\Quan4 Culinary Tourism System — From Scratch\backend\Quan4CulinaryTourism.Api
```

Chạy:

```bash
dotnet restore
dotnet run
```

Backend dùng sẵn cấu hình trong `appsettings.json`:

- MongoDB: `mongodb://localhost:27017`
- Database: `quan4_culinary_tourism`
- CORS local:
  - `http://localhost:5173`
  - `http://localhost:3000`

Khi backend lên thành công, kiểm tra:

- Health check: `http://localhost:5163/api/health`
- Swagger: `http://localhost:5163/swagger`

Tài khoản admin seed sẵn:

- Email: `admin@quan4tourism.local`
- Password: `Admin@123456`

## 5. Chạy website admin

Mở terminal tại:

```powershell
D:\Quan4 Culinary Tourism System — From Scratch\website-admin
```

Tạo file `.env`:

```env
VITE_API_BASE_URL=http://localhost:5163
```

Nếu file này đã có rồi thì giữ nguyên.

Chạy:

```bash
npm install
npm run dev
```

Mở:

- `http://localhost:5173`

Build production:

```bash
npm run build
```

Lưu ý:

- Phải chạy backend trước để login và load dữ liệu.
- Admin web đang dùng chính tài khoản admin được seed từ backend.

## 6. Chạy mobile

Mở terminal tại:

```powershell
D:\Quan4 Culinary Tourism System — From Scratch\mobile\Quan4CulinaryTourism.Mobile
```

### Chạy bản Windows

```bash
dotnet build -f net10.0-windows10.0.19041.0
dotnet run -f net10.0-windows10.0.19041.0
```

Backend mặc định cho Windows là:

- `http://localhost:5163`

### Chạy bản Android Emulator

```bash
dotnet build -f net10.0-android
```

Sau đó chạy từ Visual Studio hoặc deploy lên emulator/device theo môi trường MAUI trên máy.

Backend mặc định cho Android Emulator là:

- `http://10.0.2.2:5163`

### Chạy trên máy thật

Trong app, vào `Settings` và nhập LAN IP của máy chạy backend, ví dụ:

```text
http://192.168.1.50:5163
```

### Google Maps trên Android

Muốn bật map thật, thay API key placeholder trong:

- `mobile/Quan4CulinaryTourism.Mobile/Platforms/Android/AndroidManifest.xml`

Hiện project vẫn chạy được flow fallback nếu chưa cấu hình Google Maps key.

### Lưu ý build Android

Project hiện nằm trong đường dẫn có ký tự Unicode/em dash:

```text
D:\Quan4 Culinary Tourism System — From Scratch
```

Trên một số máy, Android packaging có thể lỗi do đường dẫn này. Nếu gặp lỗi build Android, copy project sang một đường dẫn ASCII thuần như:

```text
D:\Quan4CulinaryTourism
```

rồi build lại.

## 7. Website user

Thư mục `website-user` hiện chưa có source app để chạy.

Hiện tại chỉ có:

- `website-user/yeucau.txt`

Nghĩa là repo này mới có:

- backend server
- admin web client
- mobile client
- tài liệu yêu cầu cho user web

Nếu cần chạy website user, phải implement thêm source cho client này trước.

## 8. Thứ tự chạy đầy đủ

Nếu muốn chạy toàn bộ phần đang có trong repo, dùng thứ tự này:

1. Khởi động MongoDB.
2. Chạy backend tại `backend/Quan4CulinaryTourism.Api`.
3. Mở Swagger hoặc `api/health` để xác nhận backend sống.
4. Chạy admin web tại `website-admin`.
5. Chạy mobile tại `mobile/Quan4CulinaryTourism.Mobile`.

## 9. Checklist kiểm tra nhanh

- MongoDB đang chạy ở cổng `27017`
- Backend mở được `http://localhost:5163/swagger`
- Admin web mở được `http://localhost:5173`
- Login admin thành công bằng `admin@quan4tourism.local`
- Mobile gọi đúng base URL theo nền tảng

## 10. Lệnh nhanh

### Backend

```bash
cd "D:\Quan4 Culinary Tourism System — From Scratch\backend\Quan4CulinaryTourism.Api"
dotnet restore
dotnet run
```

### Admin web

```bash
cd "D:\Quan4 Culinary Tourism System — From Scratch\website-admin"
npm install
npm run dev
```

### Mobile Windows

```bash
cd "D:\Quan4 Culinary Tourism System — From Scratch\mobile\Quan4CulinaryTourism.Mobile"
dotnet run -f net10.0-windows10.0.19041.0
```
