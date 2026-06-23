# API Test Flow

## Preconditions

- Backend dang chay tai `http://localhost:5163`.
- MongoDB dang hoat dong.
- Admin seed:
  - Email: `admin@quan4tourism.local`
  - Password: `Admin@123456`

## Test Order

1. `GET /api/health`
   - Muc tieu: xac nhan API va MongoDB healthy.

2. `POST /api/v1/admin/auth/login`
   - Muc tieu: lay `accessToken` admin dung cho cac buoc admin.
   - Body:

```json
{
  "email": "admin@quan4tourism.local",
  "password": "Admin@123456"
}
```

3. `GET /api/v1/categories`
   - Muc tieu: xac nhan public category list.

4. `POST /api/v1/categories`
   - Muc tieu: tao category mau neu he thong chua co category de test POI.

5. `POST /api/v1/admin/pois`
   - Muc tieu: tao POI bang token admin.
   - Luu lai `poiId`.

6. `GET /api/v1/poi/load-all`
   - Muc tieu: xac nhan POI vua tao xuat hien trong danh sach cong khai.

7. `GET /api/v1/poi/nearby`
   - Muc tieu: xac nhan truy van theo toa do.

8. `POST /api/v1/auth/register`
   - Muc tieu: tao user thuong de test flow owner.

9. `POST /api/v1/auth/login`
   - Muc tieu: lay token cua user thuong.

10. `POST /api/v1/auth/register-owner`
    - Muc tieu: user thuong gui yeu cau owner.
    - Luu lai `ownerRegistrationId`.

11. `GET /api/v1/admin/owner-registrations?status=pending`
    - Muc tieu: admin thay duoc yeu cau vua gui.

12. `PUT /api/v1/admin/owner-registrations/{id}/approve`
    - Muc tieu: approve owner.

13. `POST /api/v1/auth/login`
    - Muc tieu: login lai user da duoc approve de token moi chua role `Owner`.

14. `POST /api/v1/owner/submissions`
    - Muc tieu: owner tao submission.
    - Luu lai `submissionId`.

15. `GET /api/v1/admin/submissions?status=pending`
    - Muc tieu: admin thay submission cho phe duyet.

16. `PUT /api/v1/admin/submissions/{id}/approve`
    - Muc tieu: admin approve submission.

17. `POST /api/v1/admin/media/upload-image`
    - Muc tieu: upload anh admin.

18. `POST /api/v1/admin/media/upload-audio`
    - Muc tieu: upload audio admin.

19. `POST /api/v1/admin/pois/{poiId}/audio`
    - Muc tieu: gan audio cho POI.

20. `POST /api/v1/admin/pois/{poiId}/localizations`
    - Muc tieu: tao ban dich.

21. `GET /api/v1/admin/pois/{poiId}/localizations`
    - Muc tieu: xac nhan localization da luu.

22. `POST /api/v1/analytics/collect`
    - Muc tieu: ghi nhan event `poi_viewed`, `audio_played`, `search_executed`.

23. `GET /api/v1/admin/dashboard/stats`
    - Muc tieu: xem tong quan dashboard admin.

24. `GET /api/v1/admin/analytics/summary`
    - Muc tieu: doi chieu thong ke analytics.

25. `GET /api/v1/maps/pack-manifest`
    - Muc tieu: kiem tra map pack neu da co du lieu.

26. `GET /api/v1/audio/pack-manifest`
    - Muc tieu: kiem tra audio pack manifest.

## Suggested Test Data

- Category code: `street-food`
- Category name: `Street Food`
- Price range: `$$`
- Languages de test: `vi`, `en`
- Analytics events: `poi_viewed`, `audio_played`, `search_executed`, `nearby_requested`

## Important Notes

- Frontend admin nen dung `POST /api/v1/admin/auth/login` de dam bao chi tai khoan admin dang nhap duoc.
- Sau khi approve owner, phai login lai tai khoan do de lay JWT moi chua role `Owner`.
- `GET /api/v1/poi/load-all` hien tai khong tra metadata phan trang, frontend can tu xu ly.
- `GET /api/v1/categories` chi tra category active.
