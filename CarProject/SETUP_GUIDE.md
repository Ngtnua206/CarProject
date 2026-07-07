# Hướng Dẫn Setup & Chạy

## 1. Yêu Cầu

- Docker Desktop (WSL2 backend)
- .NET SDK 10.0+
- SQL Server Management Studio (tùy chọn)

## 2. Cấu Hình Database

### 2.1. Khởi động SQL Server

```powershell
cd CarProject
docker compose up -d
```

### 2.2. Tạo file appsettings.Development.json

Trong thư mục `CarProject/CarProject/`, tạo file `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=100.108.48.1,1433;Database=CarShopDb;User Id=Your_Id;Password=YourPassworD;TrustServerCertificate=True;"
  }
}
```

> File này đã được `.gitignore`, không lo lộ mật khẩu lên GitHub.
> Nếu SQL Server chạy trên máy khác qua Tailscale, đổi `localhost` thành IP Tailscale.

### 2.3. (Tùy chọn) Cấu hình SMTP gửi mail xác nhận

Thêm vào `appsettings.Development.json`:

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UserName": "your-email@gmail.com",
    "Password": "app-password",
    "From": "your-email@gmail.com",
    "EnableSsl": true
  }
}
```

Bỏ qua bước này nếu không cần gửi mail — đăng ký vẫn tạo tài khoản được.

## 3. Database Migration

```powershell
dotnet ef database update --project CarProject\CarProject
```

## 4. Chạy Ứng Dụng

```powershell
dotnet run --project CarProject\CarProject
```

App chạy tại: `http://localhost:5001`

Hoặc dùng `run.bat` (click đúp).

## 5. Tài Khoản Mẫu

| Vai trò | Tên đăng nhập | Mật khẩu |
|---------|---------------|----------|
| Admin   | `admin`       | `admin123` |
| User    | `user`        | `user123` |

## 6. Luồng Đăng Ký Mới

1. Vào `/Account/Register`
2. Nhập email → bấm "Đăng ký với Google"
3. Nếu email chưa tồn tại → đặt mật khẩu (tối thiểu 8 ký tự, gồm chữ hoa, chữ thường, số, ký tự đặc biệt)
4. Submit → nhận mail xác nhận (nếu cấu hình SMTP)
5. Click link xác nhận → đăng ký hoàn tất
6. Nếu email đã tồn tại → redirect sang đăng nhập kèm thông báo

## 7. Cấu Trúc Thư Mục

```
CarProject/
├── CarProject/
│   ├── Pages/            # Razor Pages
│   │   ├── Account/      # Login, Register, Logout
│   │   ├── Admin/        # CRUD các bảng
│   │   └── Orders/       # Đặt cọc
│   ├── Models/           # EF Core models
│   ├── Data/             # DbContext + DbInitializer
│   ├── Services/         # JwtService, EmailService, ActivityLogService
│   ├── wwwroot/          # CSS, JS, lib
│   └── appsettings.json  # Config (mật khẩu giữ)
├── docker-compose.yml    # SQL Server container
└── run.bat               # Click chạy
```
