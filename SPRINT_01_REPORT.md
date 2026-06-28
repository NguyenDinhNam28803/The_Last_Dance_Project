# 📊 Báo cáo Sprint 01 — Nền tảng Maker–Checker & Khách hàng (Client)

> Thực hiện: 2026-06-27 · Branch: `claude/project-analysis-a06h5o`
> Mục tiêu sprint: chuẩn hóa vòng đời Maker–Checker theo URD, bổ sung nghiệp vụ Client, vá lỗi build, siết bảo mật.

---

## 1. Tóm tắt nhanh

| Hạng mục | Kết quả |
|---|---|
| Lỗi build frontend (có sẵn) | ✅ Đã sửa — **build xanh** (đã kiểm chứng bằng Vite) |
| Maker–Checker state machine | ✅ Refactor theo URD (hằng số trạng thái, bắt buộc lý do từ chối, mở rộng hủy) |
| Client domain helpers | ✅ ClientID/CustodyID/FATCA/tuổi (service mới + DI) |
| Bảo mật & dọn dẹp | ✅ JWT fail-fast, CORS theo config, xóa WeatherForecast, dedupe usings |
| Seed InstitutionType | ✅ SQL script (40 giá trị) + hằng số frontend |

> ⚠️ **Lưu ý môi trường:** container này **không có .NET SDK**, nên backend (C#) **chưa được biên dịch tại đây**. Code viết theo đúng pattern hiện có; bạn cần `dotnet build` trên máy Windows để xác nhận. Frontend (Node) **đã build kiểm chứng**.

---

## 2. Chi tiết thay đổi

### 2.1. Sửa lỗi build frontend (đã kiểm chứng ✅)
Trước sprint, `npm run build` **thất bại** với 2 lỗi missing export:
- `ImportExportService` không được export trong `services/api.js` → đã thêm (template/export/import customers).
- `addTypeLabels` không được export trong `data/mockData.js` → đã thêm (A/S/E/F → nhãn).
- `ClientView.vue` thiếu import `useCustomerContactStore`, `useNotify` → đã bổ sung.

Kết quả: **build thành công** (`✓ built in ~340ms`).

### 2.2. Maker–Checker theo URD (`MakerCheckerService.cs`, `CustomerContactService.cs`)
- Thêm `Constants/RecordStatus.cs`: `TransactionStatus` (N/A/R/C), `TransactionType` (INSERT/UPDATE/DELETE + `Normalize`), `RecordStatus` (PI/PU/PD/A/R/D theo URD).
- Thay toàn bộ chuỗi "magic string" bằng hằng số.
- **Reject bắt buộc lý do** (`reason` rỗng → từ chối thất bại) — đúng URD.
- **Cancel** mở rộng: cho phép hủy bản ghi `Chờ duyệt (N)` **hoặc** `Từ chối (R)` — đúng URD.
- Chuẩn hóa `MtlType` đầu vào → đồng bộ `ApplyMakerCheckerAsync` (INSERT/UPDATE/DELETE).
- Giữ nguyên giá trị wire `MtlStatus` (N/A/R/C) → **không phá vỡ** frontend hiện có.

### 2.3. Client domain helpers (`IClientCodeService` + `ClientCodeService`)
Các quy tắc nghiệp vụ URD mục 3.2, đăng ký DI trong `Program.cs`:
- `GenerateClientIdAsync()` — sinh ClientID 6 số nhỏ nhất chưa dùng (icon `#`).
- `GenerateCustodyId()` — `003C`/`003F` + ClientID theo trong/ngoài nước.
- `DetectFatca()` — tự phát hiện dấu hiệu Hoa Kỳ (quốc tịch/địa chỉ).
- `IsAtLeast18()` — kiểm tra KH cá nhân đủ 18 tuổi.

> Các helper này sẵn sàng để `UserController`/`CustomerService` gọi khi hoàn thiện luồng tạo Client (Phase 2).

### 2.4. Bảo mật & dọn dẹp (`Program.cs`)
- **JWT SecretKey fail-fast**: bỏ giá trị mặc định yếu hard-code; thiếu config → ném lỗi khi khởi động.
- **CORS theo cấu hình**: đọc `Cors:AllowedOrigins`; có khai báo → whitelist + `AllowCredentials`; không → fallback `AllowAnyOrigin` (chỉ dev).
- Dedupe `using` trùng lặp.
- Xóa `WeatherForecast.cs` + `WeatherForecastController.cs` (template thừa).

### 2.5. Seed InstitutionType
- `scripts/seed_institution_types.sql` — idempotent, chèn nhóm `INSTITUTION_TYPE` + 40 giá trị vào `SYSTEMCODE`/`SYSTEMCODE_VALUE`.
- `frontend/src/constants/institutionType.js` — 40 giá trị (VN/EN + scope) cho droplist.
- `frontend/src/constants/recordStatus.js` — nhãn + badge trạng thái, dùng ngay trong grid `ClientView`.

---

## 3. Việc bạn cần làm để chạy (hành động phía bạn)

1. **Build & chạy backend** (máy có .NET SDK):
   ```bash
   dotnet build
   ```
   Nếu chưa cấu hình secret, app sẽ báo lỗi rõ ràng (đúng thiết kế mới). Đặt secret:
   ```bash
   dotnet user-secrets set "JwtSettings:SecretKey" "<chuỗi-bí-mật-đủ-dài>"
   ```
2. **Seed InstitutionType** vào DB:
   ```bash
   sqlcmd -S <server> -d Last_Dance_API -i scripts/seed_institution_types.sql
   ```
3. *(Tùy chọn, production)* thêm vào `appsettings.json`:
   ```json
   "Cors": { "AllowedOrigins": [ "https://your-frontend-domain" ] }
   ```

---

## 4. Chưa làm (đưa sang sprint sau)

- Tách hẳn entity `User` (auth) khỏi `Client` (nghiệp vụ) — cần migration, **rủi ro cao**, làm khi có .NET SDK + DB.
- Bổ sung cột giấy tờ định danh (IdType/IdNumber/IdIssueDate…) vào schema — cần migration.
- Unit test state machine (cần tạo test project — hoãn vì không build/test C# được tại đây).
- Toolbar động theo vai trò + tìm kiếm wildcard + cấu hình cột (Phase 3).

---

## 6. Bổ sung Phase 2 — Validation KYC & luồng tạo Client

> Frontend đã build kiểm chứng ✅. Backend chưa compile tại đây (không có SDK).

- **`Dtos/ClientDto.cs`** — `ClientCreateDto` với các trường KYC hiện có cột trong `Customer`.
- **`Validators/ClientValidators.cs`** — FluentValidation theo URD 3.2.3:
  - KH cá nhân (Retail): bắt buộc Giới tính + Ngày sinh, **đủ 18 tuổi**.
  - KH tổ chức (Institution): bắt buộc Loại hình tổ chức.
  - KH nước ngoài (Foreign): bắt buộc Investor code.
  - ClientID nếu nhập tay phải **6 chữ số**.
- **`CustomerService.CreateClientAsync`** (inject `IClientCodeService`):
  - `#`/rỗng → **tự sinh ClientID**; chặn trùng mã.
  - Tự sinh **CustodyID** (003C/003F) + **auto-detect FATCA**.
  - Đặt `RecordStatus = PendingInsert` (PI), Role = USER.
- **Endpoint mới** (`UserController`): `GET /User/Client/next-id`, `POST /User/Client`.
- **Frontend**: `ClientService.getNextId` + nút **#** tự sinh mã trên màn hình Client.

### Việc bạn cần làm thêm
- `dotnet build` để xác nhận biên dịch backend Phase 2.
- (Tùy chọn) bổ sung cột giấy tờ định danh nếu muốn validate đầy đủ IDType/IDNumber.

---

## 7. Phase 3 — Giao diện chung: tìm kiếm wildcard, phân trang, cấu hình cột, toolbar động

> Toàn bộ là frontend — **đã build kiểm chứng** ✅ (Vite). Không có thay đổi backend.

- **`utils/wildcard.js`** — tìm kiếm theo URD: `MIN*`, `*MIN`, `*MIN*`, khớp chính xác (không phân biệt hoa/thường).
- **`composables/useDataGrid.js`** — lưới dùng chung, tái sử dụng được cho mọi màn hình:
  - Tìm kiếm wildcard nhiều trường.
  - **Phân trang 10/20/50/100** + điều hướng trang.
  - **Cấu hình cột** (hiện/ẩn + đổi thứ tự ↑↓), **lưu localStorage theo từng user**, nút "Mặc định".
- **`ClientView.vue`** tích hợp:
  - Ô tìm kiếm + Clear + chọn số dòng/trang + nút ⚙ Cột (popup cấu hình).
  - Lưới render cột động theo cấu hình; badge trạng thái; trạng thái rỗng.
  - Thanh phân trang.
  - **Toolbar động theo vai trò** (URD Maker vs Checker): Maker/Admin có Add/Edit/Copy/Delete/Import; Checker/Admin có Approve/Reject; chung Search/Refresh/Audit/Export/Template.
  - Handler bổ sung: Refresh (làm mới), Cancel, Edit; các chức năng chưa hoàn thiện (Approve/Reject/Delete/Copy/Audit) hiển thị thông báo "đang phát triển".

### Còn lại cho Phase 3 (sprint sau)
- Tìm kiếm nhiều tiêu chí riêng từng trường + chọn nhiều bản ghi (bulk).
- Kéo-thả đổi thứ tự cột (hiện dùng nút ↑↓).

---

## 8. Phase 4 — Hoàn thiện Maker–Checker cho bản ghi Client

> Frontend **đã build kiểm chứng** ✅. Backend viết theo pattern hiện có, **cần `dotnet build`** xác nhận.

**Backend (`CustomerService` + `ICustomerService` + `UserController`):**
- `ApproveClientAsync` — Chờ duyệt thêm/sửa → **Active** (đặt OpenDate khi duyệt thêm); Chờ duyệt xóa → **Deleted** (Closed + CloseDate). Quy tắc **4 mắt** (không tự duyệt bản ghi mình tạo/sửa).
- `RejectClientAsync` — **bắt buộc lý do**, → Rejected (lưu RejectDes).
- `RequestDeleteClientAsync` — Active → **PendingDelete**.
- `GetClientAuditAsync` — đọc lịch sử từ MTTRAN theo từng Client.
- **Audit append-only**: mỗi sự kiện (tạo/duyệt/từ chối/yêu cầu xóa) ghi 1 dòng MTTRAN.
- **Endpoints**: `POST /User/Client/{id}/approve`, `/reject`, `/delete-request`; `GET /User/Client/{id}/audit`.

**Sửa lỗi tiềm ẩn:** `MtlType` (cột `nvarchar(3)`) trước đây bị gán chuỗi dài ("CREATE"…) → lỗi truncation. Đã đổi `TransactionType` sang mã ngắn **I/U/D**.

**Frontend (`ClientView`):**
- Handler thực cho **Approve / Reject / Delete** (gọi API, làm mới danh sách, xác nhận + nhập lý do từ chối).
- **Popup Audit trail** hiển thị lịch sử thay đổi (thời gian, hành động, trạng thái, maker, checker, mô tả) với nhãn tiếng Việt.

### Còn lại
- Chức năng Copy record.
- Bulk trong 1 transaction phía backend (hiện điều phối phía frontend).

---

## 9. Phase 5 — Sửa Client đầy đủ (Edit → PendingUpdate) + Thao tác hàng loạt

> Frontend **đã build kiểm chứng** ✅ (ClientView ~28.7kB). Backend cần `dotnet build`.

### A. Màn sửa Client đầy đủ (Edit → PendingUpdate)
**Backend:**
- `UserResponseDto` mở rộng: RegistrationType, InstitutionType, InvestorCode, PlaceOfBirth, IsStaff, CreationMethod, CustodyCd, FATCA (+ cập nhật `MapToResponseDto`) → form round-trip đủ dữ liệu.
- `ClientUpdateDto` + `UpdateClientAsync`: sửa bản **Active/Rejected/PendingUpdate → PendingUpdate**; bản **PendingInsert giữ nguyên**; recompute CustodyID/FATCA; chặn tuổi <18 (cá nhân); log MTTRAN. 
- Endpoint **`PUT /User/Client/{id}`**.

**Frontend (`ClientView` tab Thông tin chung):**
- Dựng form đầy đủ: Mã KH, Tên (+ngôn ngữ khác, viết tắt), Loại hình KH, Quốc tịch, **Loại tổ chức** (hiện theo KH tổ chức, lọc theo trong/ngoài nước), **Investor code** (KH nước ngoài), **Giới tính + Ngày sinh** (KH cá nhân), Nơi sinh, Quốc gia cư trú, Email, SĐT, Kênh mở TK, NV công ty; khối Trạng thái (badge + CustodyID + FATCA, chỉ đọc).
- Validate client-side theo loại hình (đủ 18 tuổi, investor code, loại tổ chức…).
- Edit nạp bản ghi → sửa → Save gọi `PUT` → PendingUpdate; báo lỗi từ server.

### B. Thao tác hàng loạt (bulk) trên grid
- Cột checkbox + chọn-tất-c-trang; thanh hành động hiện khi có lựa chọn.
- **Checker/Admin**: Duyệt / Từ chối hàng loạt (một lý do dùng chung). **Maker/Admin**: Yêu cầu xóa hàng loạt.
- Điều phối bằng `Promise.allSettled`, báo cáo số thành công/thất bại, tự làm mới danh sách.

### Còn lại
- Bulk gói trong 1 transaction backend (hiện gọi tuần tự nhiều request).

---

## 10. Phase 6 — Tab Liên hệ (CRUD) + Copy record

> Frontend **đã build kiểm chứng** ✅ (ClientView ~34.2kB). Backend: chỉ chỉnh phân quyền controller.

### A. Phân quyền liên hệ (`CustomerContactController`)
- Trước đây chỉ **Administrator** (hoặc chính chủ) được quản lý liên hệ → **Maker bị Forbid** khi thao tác liên hệ của Client.
- Thêm helper `CanRead` (Admin/Maker/Checker/chính chủ) và `CanWrite` (Admin/Maker/chính chủ); thay toàn bộ kiểm tra cứng. Ghi vẫn đi qua maker-checker (chờ duyệt).

### B. Tab Liên hệ trong `ClientView`
- Danh sách liên hệ của Client đang chọn: Loại LH, Loại thông tin, Giá trị, Quốc gia, Mặc định.
- **Thêm/Sửa/Xóa/Đặt mặc định** inline (Maker/Admin).
- `InfoType` thay đổi động theo `AddType` (Địa chỉ/SĐT/Email/Fax), trường Fax attention hiện khi loại = Fax.
- Thêm/sửa/xóa **gửi qua maker-checker** (hiển thị thông báo chờ duyệt); Đặt mặc định áp dụng ngay.

### C. Copy record
- Nút **Copy** (Maker/Admin): sao chép dữ liệu bản ghi đang chọn sang chế độ thêm mới, xóa Mã KH + các trường tự sinh để tạo bản ghi mới.

### Còn lại cho phân hệ Client
- Bulk gói trong 1 transaction backend.
- Tải/hiển thị nhiều ảnh chữ ký; cột giấy tờ định danh (cần migration).

---

## 11. Phase 7 — Đa ngôn ngữ VN/EN (i18n)

> 100% frontend — **đã build kiểm chứng** ✅. Không thêm thư viện ngoài (i18n tự xây, nhẹ).

**Hạ tầng:**
- `i18n/messages.js` — từ điển VN/EN (nav, field, grid, contact, common…).
- `stores/locale.js` — Pinia store: `locale` (lưu localStorage), `setLocale`, `t(key)` (dịch khóa), `tc({vi,en})` (dịch object hằng số).
- `composables/useI18n.js` — `{ t, tc, locale, setLocale }`.

**Áp dụng:**
- `TopNav`: **nút chuyển VN/EN** + dịch toàn bộ menu điều hướng + nút Đăng xuất + placeholder tìm kiếm.
- `ClientView`: dịch tab, tiêu đề section, **toàn bộ nhãn trường**, header lưới + cấu hình cột, phân trang, tab Liên hệ; **các droplist đổi ngôn ngữ tức thì** (loại hình KH, loại tổ chức, giới tính, kênh mở TK, loại liên hệ/loại thông tin) qua `tc()`; trạng thái bản ghi theo ngôn ngữ.
- Đổi ngôn ngữ **không reload trang**, ghi nhớ lựa chọn (localStorage) — đúng yêu cầu URD.

### Còn lại
- Áp i18n cho các màn còn lại (Login, Dashboard, User, SystemCode, Audit, MakerChecker) theo cùng pattern.
- Toolbar buttons (Search/Add/…) hiện vẫn nhãn cố định — có thể map qua `t()` sau.

---

## 5. Tự đánh giá rủi ro

| Thay đổi | Rủi ro | Ghi chú |
|---|---|---|
| Frontend fixes | Thấp | Đã build kiểm chứng |
| MakerChecker/Constants | Trung bình | Không compile tại đây; pattern đơn giản, giữ tương thích wire |
| ClientCodeService | Thấp–TB | Self-contained, không động DB schema |
| Program.cs (JWT/CORS) | Thấp | Thay đổi nhỏ, có `JWTService` fail-fast sẵn làm tiền lệ |
| Không đụng EF model/seed | — | Cố ý, tránh lệch ModelSnapshot khi chưa sinh được migration |
