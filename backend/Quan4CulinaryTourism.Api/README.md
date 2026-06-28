# Quan4CulinaryTourism.Api

Backend ASP.NET Core Web API cho dự án du lịch ẩm thực Quận 4, dùng MongoDB, JWT Bearer và upload file local.

## Yêu cầu

- .NET SDK 10
- MongoDB tại `mongodb://localhost:27017`

Nếu chưa có MongoDB local, từ repo root chạy:

```powershell
docker compose up mongo -d
```

## Chạy backend local

```powershell
cd backend\Quan4CulinaryTourism.Api
dotnet restore
python -m pip install -r tools\requirements-tts.txt
dotnet run
```

Khi chạy bằng `dotnet run`:

- profile mặc định là `Development`
- API ở `http://localhost:5163`
- Swagger ở `http://localhost:5163/swagger`

## Tài khoản admin local

Trong `Development`, backend tự seed admin từ `appsettings.Development.json`:

- Email: `admin@quan4tourism.local`
- Password: `Admin@123456`

## Cấu hình

Nguồn cấu hình chính:

- `appsettings.json`: giá trị cơ sở
- `appsettings.Development.json`: giá trị để chạy local
- biến môi trường: dùng để override khi cần

Tóm tắt:

- `dotnet run` không tự đọc file `.env.example`
- `backend/Quan4CulinaryTourism.Api/.env.example` chỉ là template để import vào shell, IDE, hoặc container
- nếu chạy API trong Docker, copy `.env.example` ở repo root thành `.env`

Section quan trọng:

- `MongoDbSettings`
- `JwtSettings`
- `UploadSettings`
- `TextToSpeechSettings`
- `DefaultAdmin`
- `Cors.AllowedOrigins`

## Audio tiếng Việt từ Python

Backend có script Python `tools/tts_generate.py` dùng `gTTS` để tạo MP3 tiếng Việt từ `ttsScript` hoặc `description` của POI.

Luồng hiện tại:

- FE gọi `GET /api/v1/poi/{id}/audio?lang=vi`
- nếu Mongo chưa có `PoiAudio` tiếng Việt, backend sẽ tự sinh file MP3 vào `wwwroot/uploads/audio`
- file tạo xong sẽ được lưu lại trong `poi_audios` để lần sau phát trực tiếp

Thiết lập chính:

- `TextToSpeechSettings:Enabled`
- `TextToSpeechSettings:PythonCommand`
- `TextToSpeechSettings:DefaultVoice`

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

## Kiểm tra nhanh

1. `GET /api/health`
2. `POST /api/v1/auth/login` bằng tài khoản admin local
3. `GET /api/v1/categories`
4. `GET /api/v1/poi/load-all`
5. `GET /api/v1/poi/nearby`

## Build

```powershell
dotnet build
```
