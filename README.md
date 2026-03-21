# 🚀 The Last Dance Project - System Documentation

## 📝 Giới thiệu
**The Last Dance Project** là một hệ thống Web API hiện đại được xây dựng trên nền tảng .NET 9. Hệ thống tập trung vào việc quản lý định danh (Identity), quản trị người dùng (User Management) và đặc biệt là cơ chế **Giám sát thay đổi (Audit Logging)** chuyên sâu.

---

## 🏗 Kiến trúc hệ thống
Hệ thống được thiết kế theo mô hình phân lớp (Layered Architecture):
*   **Presentation Layer (Controllers):** Tiếp nhận yêu cầu HTTP, xử lý phân quyền và trả về dữ liệu.
*   **Business Logic Layer (Services):** Xử lý các quy tắc nghiệp vụ, mã hóa mật khẩu, kiểm tra điều kiện dữ liệu.
*   **Data Access Layer (Data/Models):** Tương tác với SQL Server thông qua Entity Framework Core 9.
*   **DTO Layer (Data Transfer Objects):** Chuẩn hóa dữ liệu đầu vào/đầu ra, bảo mật thông tin nhạy cảm.

---

## 🔐 Luồng hệ thống chính (Core System Flows)

### 1. Luồng Xác thực & Bảo mật (Authentication Flow)
1.  **Đăng nhập:** Người dùng gửi `Username` và `Password`.
2.  **Xác thực:** `AuthService` kiểm tra sự tồn tại của người dùng và dùng **BCrypt** để so khớp mật khẩu băm.
3.  **Cấp Token:** Nếu hợp lệ, `JWTService` tạo một mã **JWT (Access Token)** chứa các Claims (UserId, UserName, Role).
4.  **Sử dụng:** Người dùng đính kèm Token vào Header `Authorization: Bearer {token}` cho các yêu cầu tiếp theo.
5.  **Đăng xuất:** Token sẽ được đưa vào `TokenBlacklist` trong DB để ngăn chặn việc tái sử dụng (Revocation).

### 2. Luồng Quản lý người dùng (User Management Flow)
*   **Khởi tạo:** Chỉ tài khoản có Role `ADMIN` mới có quyền tạo người dùng mới qua `UserController`.
*   **Phân quyền (RBAC):** Người dùng được gán các vai trò như `ADMIN`, `MANAGER`, `USER`. Hệ thống kiểm tra quyền thông qua Middleware `[Authorize(Roles = "...")]`.
*   **Trạng thái:** Admin có thể "Toggle Status" để khóa (Inactive) hoặc mở khóa (Active) tài khoản ngay lập tức.

### 3. Luồng Giám sát thay đổi (Audit Logging Flow)
Đây là điểm đặc biệt của hệ thống:
1.  **Ghi nhận giao dịch:** Mỗi khi có một thay đổi dữ liệu quan trọng, một bản ghi được tạo trong `Auditentity` (Bảng `MTTRAN`).
2.  **Chi tiết thay đổi:** Các giá trị Cũ (Old) và Mới (New) của từng trường dữ liệu được lưu vết trong `AuditEntityKey`.
3.  **Truy vết:** Cho phép quản trị viên biết chính xác: *Ai đã sửa? Sửa cái gì? Sửa lúc nào?*

### 4. Luồng Cấu hình hệ thống (System Configuration)
*   Sử dụng cặp bảng `SystemCode` và `SystemCodeValue`.
*   **Mục đích:** Quản lý các danh mục động (Dropdowns, Status codes, Role names) mà không cần sửa mã nguồn hoặc cấu trúc database.

---

## 🛠 Công nghệ sử dụng
*   **Framework:** .NET 9.0 Web API
*   **Database:** Microsoft SQL Server
*   **ORM:** Entity Framework Core 9.0
*   **Security:** JWT, BCrypt.Net-Next
*   **Documentation:** Swagger UI (OpenAPI 3.0)

---

## 🚀 Hướng dẫn cài đặt & Chạy
1.  **Cấu hình Database:** Cập nhật chuỗi kết nối trong `appsettings.json`:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER;Database=TheLastDanceDB;Trusted_Connection=True;TrustServerCertificate=True;"
    }
    ```
2.  **Migration:** Chạy lệnh sau trong Package Manager Console:
    ```powershell
    Update-Database
    ```
3.  **Chạy dự án:** Nhấn `F5` hoặc dùng lệnh:
    ```bash
    dotnet run
    ```
4.  **Truy cập API:** Mở trình duyệt tại `https://localhost:{port}/swagger` để xem tài liệu API.

---

## 📊 Sơ đồ bảng dữ liệu chính
| Nhóm | Các bảng chính |
| :--- | :--- |
| **Người dùng** | `Customers`, `CustomerContacts`, `Roles` |
| **Bảo mật** | `TokenBlacklists` |
| **Audit** | `MTTRAN` (Auditentity), `AUDITENTITY_KEY` |
| **Cấu hình** | `SYSTEMCODE`, `SYSTEMCODE_VALUE` |

---
*Copyright © 2026 - The Last Dance Project Team*
