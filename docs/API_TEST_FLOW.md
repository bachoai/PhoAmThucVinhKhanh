# API Test Flow

## Preconditions

- Backend dang chay tai `http://localhost:5163`.
- MongoDB dang hoat dong.
- Admin seed:
  - Email: `admin@quan4tourism.local`
  - Password: `Admin@123456`

## Smoke Flow

1. `GET /api/health`
   - Muc tieu: xac nhan API va MongoDB healthy.

2. `POST /api/v1/admin/auth/login`
   - Muc tieu: lay `accessToken` admin dung cho cac buoc approve/reject.

3. `POST /api/v1/auth/register`
   - Muc tieu: tao user thuong de test flow owner/public.

4. Kiem tra response `POST /api/v1/auth/register`
   - Muc tieu: xac nhan backend tra JWT ngay sau dang ky de frontend public co the tu dang nhap.

5. `POST /api/v1/auth/register-owner`
   - Muc tieu: user thuong gui yeu cau owner.
   - Luu lai `ownerRegistrationId`.

6. Goi lai `POST /api/v1/auth/register-owner` voi cung user.
   - Muc tieu: xac nhan backend chan trung owner request dang `pending` va tra loi ro rang.

7. `GET /api/v1/admin/owner-registrations?status=pending`
   - Muc tieu: admin thay duoc owner request vua gui.

8. `PUT /api/v1/admin/owner-registrations/{id}/approve`
   - Muc tieu: admin approve owner.

9. `POST /api/v1/auth/login`
   - Muc tieu: login lai user da duoc approve de lay JWT moi chua role `Owner`.

10. `GET /api/v1/owner/dashboard` va `GET /api/v1/owner/pois`
    - Muc tieu: xac nhan user da vao duoc khong gian owner; `owner/pois` ban dau co the rong.

11. `POST /api/v1/owner/submissions`
    - Muc tieu: tao submission `create` voi `poiId = null` hoac bo qua `poiId`.
    - Luu lai `createSubmissionId`.

12. `PUT /api/v1/admin/submissions/{createSubmissionId}/approve`
    - Muc tieu: admin approve submission `create`.

13. `GET /api/v1/poi/load-all` va `GET /api/v1/owner/pois`
    - Muc tieu: POI moi xuat hien o public web va trong danh sach POI cua owner.

14. `POST /api/v1/owner/submissions`
    - Muc tieu: tao submission `update` bang cach lay `poiId` tu `GET /api/v1/owner/pois`.
    - Luu lai `updateSubmissionId`.

15. `PUT /api/v1/admin/submissions/{updateSubmissionId}/approve`
    - Muc tieu: admin approve submission `update`.

16. Kiem tra lai `GET /api/v1/poi/{id}`
    - Muc tieu: du lieu public da phan anh thay doi sau khi approve update.

17. Security checks
    - Goi `POST /api/v1/owner/submissions` voi `submissionType = update` va `poiId` cua owner khac.
    - Muc tieu: backend phai tra `403`.

18. Admin re-approve / re-reject checks
    - Goi lai `PUT /api/v1/admin/submissions/{id}/approve` hoac `reject` cho submission da xu ly.
    - Muc tieu: backend phai tu choi vi submission khong con `pending`.

19. Clean zip check
    - Chay `powershell -ExecutionPolicy Bypass -File .\scripts\create-clean-zip.ps1 -OutputPath .\release\PhoAmThucVinhKhanh-clean.zip`.
    - Muc tieu: zip khong chua `.git`, `.codegraph`, `.codex`, `.agents`, `node_modules`, `dist`, `bin`, `obj`, `tmp`, `_build_verify*`, `wwwroot/uploads`, real `.env*`, hoac zip cu trong `release`.

## Important Notes

- Frontend public phai dung `POST /api/v1/auth/register-owner`; khong con flow `POST /api/v1/owner/register`.
- Submission `update` phai lay `poiId` tu `GET /api/v1/owner/pois`, khong cho nhap tay ID tren public web.
- Sau khi approve owner, phai login lai tai khoan do de lay JWT moi chua role `Owner`.
- `GET /api/v1/poi/load-all` hien tai khong tra metadata phan trang, frontend can tu xu ly.
