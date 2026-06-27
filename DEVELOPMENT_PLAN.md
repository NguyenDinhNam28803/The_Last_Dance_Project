# 📋 Kế hoạch phát triển — The Last Dance Project

> Phân hệ **Khách hàng (Client)** thuộc Hệ thống giao dịch chứng khoán (Back Office)
> Tài liệu định hướng phát triển — cập nhật 2026-06-27

---

## 1. Bối cảnh & Mục tiêu

Hệ thống là một ứng dụng full-stack (ASP.NET Core 9 + Vue 3) hiện thực hóa **phân hệ Khách hàng** theo tài liệu đặc tả URD V1.0. Đặc trưng nghiệp vụ cốt lõi:

- **Maker–Checker** (kiểm soát 4 mắt): mọi thay đổi dữ liệu phải qua duyệt.
- **Audit trail**: lưu vết giá trị cũ/mới theo từng trường.
- **Quản lý Khách hàng chứng khoán**: hồ sơ định danh (KYC) chi tiết, nhiều liên hệ.

**Mục tiêu kế hoạch:** thu hẹp khoảng cách giữa code hiện tại (CRUD User/Customer tổng quát) và đặc tả URD (phân hệ Client chứng khoán đầy đủ).

---

## 2. Hiện trạng (đánh giá)

| Hạng mục | Trạng thái | Ghi chú |
|---|---|---|
| Build backend | ✅ Thành công (0 warning/error) | .NET 9 |
| Xác thực JWT + BCrypt + Token blacklist | ✅ Hoàn thiện | `AuthService`, `JWTService` |
| User Management (CRUD, role, toggle status) | ✅ Hoàn thiện | `CustomerService`, `UserController` |
| CustomerContact CRUD | ✅ Cơ bản | `CustomerContactService` |
| Maker–Checker | 🟡 Một phần | Chỉ tích hợp `CustomerContact`; status đơn giản hóa (N/A/R/C) |
| Audit logging | 🟡 Cơ bản | Ghi header + key, chưa có UI audit trail đầy đủ |
| SystemCode (danh mục động) | ✅ Cơ bản | `SystemCodeService` |
| Import/Export Excel | ✅ Cơ bản | EPPlus, `ImportExportService` |
| Frontend các màn hình | 🟡 Khung sườn | `ClientView.vue` còn rút gọn nhiều trường |
| i18n VN/EN | ❌ Chưa có | URD yêu cầu đổi ngôn ngữ không reload |

**Kết luận:** Phần nền tảng (auth, hạ tầng CRUD, khung UI) đã ổn. Phần **nghiệp vụ Client theo URD** và **vòng đời Maker–Checker đầy đủ** là khoảng trống lớn nhất.

---

## 3. Phân tích khoảng cách (URD ↔ Code)

### 3.1. Mô hình dữ liệu Client
URD đặc tả Client rất giàu trường; model `Customer` hiện tại đang gộp chung **User (CBNV đăng nhập)** và **Client (khách hàng chứng khoán)** — cần tách biệt.

Các trường URD còn thiếu/chưa chuẩn hóa:
- `ClientID` 6 ký tự **chỉ số**, hỗ trợ auto-sinh (`#` → giá trị nhỏ nhất chưa dùng).
- `RegistrationType` (Local/Foreign × Retail/Institutional) chi phối nhiều ràng buộc.
- Định danh: `IDType`, `IDNumber`, `IDIssueDate`, `IDExpiryDate`, `IDIssuePlace` + ràng buộc theo loại.
- `InstitutionType` (≈40 giá trị — đã có sẵn trong URD mục 3.2.4).
- `InvestorCode` (bắt buộc với KH nước ngoài).
- `FATCA` tự động tick khi có dấu hiệu Hoa Kỳ.
- `CustodyID` sinh theo quy tắc `003C/003F + ClientID`.
- `RelatedStaff`, `CreationMethod`, `OpenDate`, `CloseDate`.
- `Status` nghiệp vụ: Active / Closed / Suspend (kèm ràng buộc đóng tài khoản).
- Chữ ký: upload **nhiều ảnh** (jpg/png…).

### 3.2. Vòng đời Maker–Checker (quan trọng)
Code hiện dùng status `N/A/R/C`. URD yêu cầu **trạng thái bản ghi** đầy đủ:
`Pending Insert` · `Pending Update` · `Pending Delete` · `Active` · `Rejected` · `Deleted`.

Các luồng còn thiếu:
- Sửa bản ghi `Active` → `Pending Update`; sửa `Rejected` → về trạng thái trước khi bị từ chối.
- Hủy `Pending Insert` → xóa hẳn; hủy `Pending Update/Delete` → khôi phục bản đã duyệt gần nhất.
- Duyệt `Pending Delete` → `Deleted`; từ chối → khôi phục `Active`.
- Bắt buộc nhập **lý do từ chối**.
- Duyệt/Từ chối/Hủy **hàng loạt** (multi-select).

### 3.3. Giao diện chung (Toolbar + Grid) theo URD mục 2
- Toolbar phân biệt **Maker** vs **Checker** (ẩn/hiện nút theo vai trò + trạng thái bản ghi).
- Tìm kiếm wildcard: `MIN*`, `*MIN`, `*MIN*`, khớp chính xác.
- Grid: phân trang (10/20/50/100), cấu hình cột (hiện/ẩn, thứ tự, lưu theo user), export Excel.
- Audit trail popup, Copy record, Clear, Refresh.

### 3.4. Màn hình Login (URD mục 4)
- Ràng buộc mật khẩu: 8–20 ký tự, đủ hoa/thường/số/đặc biệt, không trùng username, không tuần tự.
- Nút Login disable đến khi nhập đủ; "Ghi nhớ đăng nhập".

---

## 4. Lộ trình theo giai đoạn (Roadmap)

### 🔹 Phase 0 — Dọn dẹp & nền tảng (1 tuần)
- [ ] Xóa `WeatherForecast*` (template thừa), dedup `using` trùng trong `Program.cs`.
- [ ] Tách cấu hình CORS production (không `AllowAnyOrigin`).
- [ ] Đưa `JwtSettings:SecretKey` ra User Secrets / biến môi trường (đang hard-code default).
- [ ] Bổ sung `.editorconfig`, chuẩn hóa response API (envelope `{ success, data, message }`).
- [ ] Tách `enum RecordStatus` rõ ràng thay cho chuỗi rời rạc.

### 🔹 Phase 1 — Chuẩn hóa mô hình Client & Maker–Checker (2–3 tuần) ⭐ ưu tiên cao
- [ ] Thiết kế lại entity `Client` theo URD (tách khỏi `User`/auth).
- [ ] Bảng staging cho Maker–Checker: lưu **bản nháp chờ duyệt** tách khỏi bản `Active`.
- [ ] Hiện thực hóa state machine đầy đủ (6 trạng thái) trong `MakerCheckerService`.
- [ ] Luồng Hủy/Từ chối có khôi phục snapshot bản đã duyệt.
- [ ] Migration EF Core + seed `InstitutionType` (40 giá trị từ URD 3.2.4).
- [ ] Unit test cho từng nhánh state machine.

### 🔹 Phase 2 — Nghiệp vụ KYC & Validation (2 tuần)
- [ ] Sinh `ClientID` (#) và `CustodyID` (003C/003F).
- [ ] Validation theo `RegistrationType`/`IDType` (FluentValidation).
- [ ] Kiểm tra tuổi ≥ 18, trùng CMT/CCCD, trùng SĐT/Email (cảnh báo + cho phép override).
- [ ] Auto-detect FATCA.
- [ ] Upload nhiều ảnh chữ ký (lưu file + metadata).
- [ ] Ràng buộc trạng thái Closed/Suspend (kiểm tra tài khoản chưa đóng).

### 🔹 Phase 3 — Giao diện chung & màn hình Client (2–3 tuần)
- [ ] Hoàn thiện `ClientView.vue`: đầy đủ trường 2 tab (Thông tin chung + Liên hệ).
- [ ] Toolbar động theo vai trò + trạng thái bản ghi.
- [ ] Tìm kiếm wildcard, grid phân trang + cấu hình cột (lưu localStorage/per-user).
- [ ] Popup Audit trail (cây thay đổi, lọc theo thời gian/UserID).
- [ ] Hiển thị diff: gạch chân đỏ + tooltip giá trị cũ khi sửa.
- [ ] Copy / Clear / Refresh.

### 🔹 Phase 4 — i18n, Login nâng cao, hoàn thiện (1–2 tuần)
- [ ] i18n VN/EN (vue-i18n) đổi ngôn ngữ không reload; droplist hiển thị theo ngôn ngữ.
- [ ] Ràng buộc mật khẩu URD + "Ghi nhớ đăng nhập".
- [ ] Import Client theo template URD (`Import.Client`) + validate BR1.
- [ ] Thông báo hệ thống chuẩn hóa (lỗi dưới trường, cảnh báo, success).

### 🔹 Phase 5 — Chất lượng & vận hành (xuyên suốt)
- [ ] Test integration (auth, maker-checker end-to-end).
- [ ] CI (GitHub Actions): build + test + lint cho cả backend & frontend.
- [ ] Tài liệu API (đã có Swagger) + hướng dẫn seed/migration.
- [ ] Logging/observability, rate-limit cho endpoint auth.

---

## 5. Nợ kỹ thuật & rủi ro cần xử lý

| Vấn đề | Mức độ | Hành động |
|---|---|---|
| `Customer` gộp User + Client | Cao | Tách entity ở Phase 1 |
| Status `N/A/R/C` không khớp URD | Cao | State machine 6 trạng thái |
| `SecretKey` JWT hard-code default | Cao (bảo mật) | Chuyển sang secret/env |
| CORS `AllowAnyOrigin` | Trung bình | Whitelist theo môi trường |
| Maker–Checker chỉ áp dụng CustomerContact | Trung bình | Tổng quát hóa cho mọi entity |
| Chưa có test tự động | Trung bình | Thêm test Phase 1 trở đi |
| Hash mật khẩu seed có vẻ không hợp lệ | Thấp | Kiểm tra lại seed BCrypt |

---

## 6. Thứ tự ưu tiên đề xuất

1. **Phase 1 (Maker–Checker + model Client)** — đây là xương sống nghiệp vụ, mọi thứ khác phụ thuộc vào nó.
2. **Phase 2 (Validation KYC)** — đảm bảo dữ liệu đúng đắn.
3. **Phase 3 (UI Client + toolbar/grid)** — đưa nghiệp vụ ra giao diện.
4. **Phase 0 (dọn dẹp)** — làm xen kẽ, ưu tiên ngay các mục bảo mật.
5. **Phase 4 & 5** — hoàn thiện và vận hành.

---

## 7. Đề xuất Sprint kế tiếp (2 tuần)

**Mục tiêu:** Hoàn tất nền tảng Maker–Checker đúng chuẩn URD cho 1 entity mẫu (Client).

- Tách entity `Client` + migration + seed `InstitutionType`.
- State machine 6 trạng thái + luồng Approve/Reject/Cancel có snapshot.
- Bổ sung các mục bảo mật Phase 0 (SecretKey, CORS).
- Unit test cho state machine.
- Cập nhật `ClientView.vue` hiển thị đúng trạng thái bản ghi mới.

---

*Tài liệu này là định hướng, có thể điều chỉnh theo ưu tiên nghiệp vụ thực tế.*
