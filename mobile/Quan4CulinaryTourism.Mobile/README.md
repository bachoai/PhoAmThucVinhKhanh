# Quan4 Culinary Tourism Mobile

Ứng dụng .NET MAUI cho demo khám phá ẩm thực Quận 4.

## Stack

- .NET MAUI
- C#
- CommunityToolkit.Mvvm
- SQLite (`sqlite-net-pcl`)
- MAUI Maps
- Plugin.Maui.Audio
- `Preferences` cho theme, language, session, base URL

## Base URL

App không còn hard-code một LAN IP duy nhất.

Preset hiện có:

- Windows: `http://localhost:5163`
- Android Emulator: `http://10.0.2.2:5163`
- Máy thật: nhập LAN IP, ví dụ `http://192.168.1.50:5163`

Có thể đổi trực tiếp trong `SettingsPage`:

- chọn preset
- hoặc nhập tay `Backend base URL`
- app vẫn giữ `AppConfig.NormalizeUrl(...)` để nối ảnh/audio relative path

## Google Maps

`Platforms/Android/AndroidManifest.xml` hiện vẫn để placeholder:

```xml
<meta-data android:name="com.google.android.geo.API_KEY" android:value="debug-placeholder-google-maps-key" />
```

Muốn bật map Android thật:

1. tạo Google Maps API key cho Android
2. thay giá trị placeholder trong `AndroidManifest.xml`
3. build lại app

Nếu chưa có key hoặc map không sẵn sàng:

- app fallback sang danh sách POI
- vẫn có nút mở Google Maps ngoài app
- không phụ thuộc backend thay đổi

## Tính năng demo đã bổ sung

- Theme light/dark đầy đủ cho background, surface, text, border
- Hero UI cho HomePage và PoiDetailPage
- Helper media + placeholder ảnh POI
- Audio offline tải về `FileSystem.AppDataDirectory/audio`
- TTS fallback khi POI chưa có audio
- Foreground geofence cơ bản với cooldown 5 phút mỗi POI
- Event analytics mới:
  - `tts_played`
  - `offline_audio_downloaded`
  - `geofence_triggered`
  - `qr_scanned`
- Màn `QR / Mã POI` để demo flow scan fallback

## Run

```bash
dotnet build -f net10.0-windows10.0.19041.0
dotnet build -f net10.0-android
```

## Test Flow

1. Mở backend tại `http://localhost:5163` hoặc cấu hình endpoint phù hợp trong `SettingsPage`.
2. Mở app, kiểm tra Home load danh mục và POI.
3. Vào detail, xác nhận ảnh render đúng với URL `/uploads/...`.
4. Phát audio online.
5. Bấm `Tải audio offline`.
6. Tắt mạng, mở lại detail và phát audio local.
7. Test `Nghe bằng TTS` với POI chưa có audio.
8. Test Nearby và cấp quyền GPS.
9. Test Map fallback khi chưa có Google Maps key.
10. Bật `Auto Narration` trong `SettingsPage` và di chuyển trong foreground để demo geofence cơ bản.
11. Test màn `QR / Mã POI`.

## Build Verification

- `dotnet build -f net10.0-android` hiện đã qua compile/XAML và dừng ở bước Android packaging với lỗi môi trường `APT2265`.
- Nguyên nhân thực tế: workspace đang nằm trong đường dẫn có ký tự Unicode/em dash: `D:\Quan4 Culinary Tourism System — From Scratch\...`
- Đây là lỗi tooling Android/AAPT trên máy hiện tại, không phải lỗi C#/XAML ứng dụng.
- Cách xử lý thực tế: copy project sang đường dẫn ASCII thuần, ví dụ `D:\Quan4CulinaryTourism\mobile\Quan4CulinaryTourism.Mobile`, rồi build Android lại.

## Notes

- Mobile vẫn map `CategoryName` phía client khi backend chưa trả trường này.
- Offline cache hiện lưu categories, POI list, POI detail và audio metadata/local path.
- `sqlite-net-pcl` đang kéo theo cảnh báo `NU1903` từ `SQLitePCLRaw.lib.e_sqlite3`; build vẫn chạy nhưng nên nâng package khi có thời gian.
