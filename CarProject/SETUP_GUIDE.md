# Hướng Dẫn Setup & Chạy Webshop Bán Hàng Ô Tô

## 1. CHUẨN BỊ DATABASE (Docker SQL Server)

### 1.1. Cài đặt Docker
- Tải Docker Desktop từ: https://www.docker.com/get-started
- Cài đặt trên Windows (chọn WSL2 backend)

### 1.2. Khởi động SQL Server Container
Mở PowerShell tại folder root (D:\Code\Code\WebMVC\CarProject\):
```powershell
docker compose up -d
```

Kiểm tra container đã chạy:
```powershell
docker ps
```

### 1.3. Kết nối từ SSMS (SQL Server Management Studio)
- Server: `localhost,1433`
- User: `sa`
- Password: `Your_strong@Passw0rd` (hoặc mật khẩu bạn đặt trong docker-compose.yml)
- Authentication: SQL Server Authentication

---

## 2. SETUP PROJECT & DATABASE MIGRATIONS

### 2.1. Cài đặt dotnet-ef tool (nếu chưa có)
```powershell
dotnet tool install --global dotnet-ef
```

### 2.2. Tạo Migration
Từ folder D:\Code\Code\WebMVC\CarProject\ chạy:
```powershell
dotnet ef migrations add InitialCreate --project CarProject
```

Lệnh này sẽ tạo folder Migrations/ với các file migration.

### 2.3. Cập nhật Database
```powershell
dotnet ef database update --project CarProject
```

Lệnh này sẽ:
- Tạo database `CarShopDb` trên SQL Server
- Chạy migration để tạo các bảng (HangXe, DongXe, PhienBanXe_SanPham, v.v.)
- Seed dữ liệu mẫu từ DbInitializer.cs

---

## 3. CHẠY WEB APPLICATION

### 3.1. Chạy từ Visual Studio
- Mở CarProject.slnx
- Nhấn **Ctrl+F5** (Run without Debugging) hoặc **F5** (Debug)
- Visual Studio sẽ khởi động app tại https://localhost:5001

### 3.2. Hoặc chạy từ Terminal
```powershell
dotnet run --project CarProject
```

Truy cập: https://localhost:5001 (hoặc http://localhost:5000)

---

## 4. SỬ DỤNG WEB APPLICATION

### Trang Chính
- URL: https://localhost:5001/
- Hiển thị danh sách Dòng Xe

### Trang Chi Tiết
- URL: https://localhost:5001/Details/{MaDong}
- Hiển thị danh sách Phiên Bản Xe của một dòng
- Nút "Đặt Cọc" để đi tới form đặt cọc

### Đặt Cọc Xe
- URL: https://localhost:5001/Orders/DepositForm/{MaPhienBan}
- Form điền thông tin khách hàng và tiền cọc
- Gọi API để lưu đơn cọc

### Đăng Nhập
- URL: https://localhost:5001/Account/Login
- **Demo accounts:**
  - Username: `admin` / Password: `admin123`
  - Username: `quanly1` / Password: `pass123`

---

## 5. ADMIN PAGES (QUẢN LÝ)

Sau khi đăng nhập, truy cập các trang quản lý:

### Quản lý Hãng Xe (CRUD)
- https://localhost:5001/Admin/HangXe/ - Danh sách
- https://localhost:5001/Admin/HangXe/Create - Thêm mới
- https://localhost:5001/Admin/HangXe/Edit/{id} - Sửa
- https://localhost:5001/Admin/HangXe/Delete/{id} - Xóa

### Quản lý Dòng Xe (CRUD)
- https://localhost:5001/Admin/DongXe/Index - Danh sách
- https://localhost:5001/Admin/DongXe/Create - Thêm mới
- https://localhost:5001/Admin/DongXe/Edit/{id} - Sửa
- https://localhost:5001/Admin/DongXe/Delete/{id} - Xóa

### Quản lý Phiên Bản Xe (CRUD)
- https://localhost:5001/Admin/PhienBan/Index - Danh sách
- https://localhost:5001/Admin/PhienBan/Create - Thêm mới

### Quản lý Quảng Cáo Banner (CRUD)
- https://localhost:5001/Admin/Banner/Index - Danh sách
- https://localhost:5001/Admin/Banner/Create - Thêm mới

---

## 6. CẤU TRÚC PROJECT

```
CarProject/
├── Pages/
│   ├── Index.cshtml - Trang chính (danh sách dòng xe)
│   ├── Details.cshtml - Chi tiết dòng xe
│   ├── Admin/
│   │   ├── HangXe/ - Quản lý hãng xe (Index, Create, Edit, Delete)
│   │   ├── DongXe/ - Quản lý dòng xe
│   │   ├── PhienBan/ - Quản lý phiên bản
│   │   └── Banner/ - Quản lý banner quảng cáo
│   ├── Account/
│   │   └── Login.cshtml - Trang đăng nhập
│   └── Orders/
│       └── DepositForm.cshtml - Form đặt cọc xe
├── Models/
│   ├── Entities.cs - Models cơ bản (HangXe, DongXe, PhienBanXe, TaiKhoan...)
│   └── MoreEntities.cs - Models bổ sung (DonDatCoc, HoaDonMuaXe, ...)
├── Data/
│   ├── AppDbContext.cs - EF Core DbContext
│   └── DbInitializer.cs - Seed data mẫu
├── Program.cs - Cấu hình ứng dụng
├── docker-compose.yml - SQL Server container config
└── appsettings.json - Connection string

```

---

## 7. TROUBLESHOOTING

### Lỗi: Could not connect to SQL Server
**Giải pháp:**
- Kiểm tra Docker đã chạy: `docker ps`
- Kiểm tra container name: `sqlserver-carproject`
- Xem logs: `docker logs sqlserver-carproject`

### Lỗi: Migration add failed
**Giải pháp:**
- Đảm bảo kết nối database OK
- Xóa folder Migrations (nếu có) và thử lại

### Lỗi: Port 1433 đã bị trùng
**Giải pháp:**
- Giết process cũ hoặc thay port khác trong docker-compose.yml
- VD: "1434:1433" (client port 1434 -> container port 1433)

---

## 8. GHI CHÚ QUAN TRỌNG

⚠️ **Security Notes:**
- Mật khẩu SA trong docker-compose là mẫu; không dùng cho production
- Session login không mã hóa password; chỉ dùng để demo
- Cần implement ASP.NET Identity hoặc OAuth2 cho production

📝 **To-Do (Mở rộng sau):**
- Thêm xác thực email
- Thêm thanh toán PayPal/Stripe integration
- Thêm admin dashboard với biểu đồ bán hàng
- Thêm role-based authorization
- Thêm API REST endpoints

---

**Hỗ trợ:** Nếu gặp vấn đề, kiểm tra logs hoặc liên hệ team phát triển.
