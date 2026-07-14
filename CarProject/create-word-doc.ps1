$word = New-Object -ComObject Word.Application
$word.Visible = $false
$doc = $word.Documents.Add()
$selection = $word.Selection

function Add-Heading($text, $level) {
    $selection.Style = $doc.Styles("Heading $level")
    $selection.TypeText($text)
    $selection.TypeParagraph()
}
function Add-Para($text) {
    $selection.Style = $doc.Styles("Normal")
    $selection.TypeText($text)
    $selection.TypeParagraph()
}
function Add-Bullet($text) {
    $selection.Style = $doc.Styles("List Bullet")
    $selection.TypeText($text)
    $selection.TypeParagraph()
}
function Add-Table($headers, $rows) {
    $table = $selection.Tables.Add($selection.Range, $rows.Count + 1, $headers.Count)
    $table.Borders.InsideLineStyle = 1
    $table.Borders.OutsideLineStyle = 1
    $table.Range.Font.Size = 10
    for ($i = 0; $i -lt $headers.Count; $i++) {
        $cell = $table.Cell(1, $i+1)
        $cell.Shading.BackgroundPatternColor = $word.GetColorCode("DarkBlue")
        $cell.Range.Font.Color = $word.GetColorCode("White")
        $cell.Range.Font.Bold = 1
        $cell.Range.Text = $headers[$i]
    }
    for ($r = 0; $r -lt $rows.Count; $r++) {
        for ($c = 0; $c -lt $rows[$r].Count; $c++) {
            $cell = $table.Cell($r+2, $c+1)
            $cell.Range.Text = $rows[$r][$c]
            if ($r % 2 -eq 1) { $cell.Shading.BackgroundPatternColor = $word.GetColorCode("LightGray") }
        }
    }
    $selection.EndKey(6)
    $selection.TypeParagraph()
}

# Cover page
$selection.ParagraphFormat.Alignment = 1
$selection.TypeParagraph(); $selection.TypeParagraph(); $selection.TypeParagraph()
$selection.Font.Bold = 1; $selection.Font.Size = 36; $selection.Font.Color = $word.GetColorCode("DarkBlue")
$selection.TypeText("MyCar Showroom"); $selection.TypeParagraph()
$selection.Font.Size = 18; $selection.Font.Bold = 0; $selection.Font.Color = $word.GetColorCode("DarkGray")
$selection.TypeText("HE THONG QUAN LY SHOWROOM XE HOI CAO CAP"); $selection.TypeParagraph(); $selection.TypeParagraph()
$selection.Font.Size = 14; $selection.Font.Color = $word.GetColorCode("Black"); $selection.Font.Bold = 1
$selection.TypeText("BAO CAO TONG QUAN KIEN TRUC HE THONG"); $selection.TypeParagraph(); $selection.TypeParagraph()
$selection.Font.Size = 12; $selection.Font.Bold = 0; $selection.Font.Italic = 1
$selection.TypeText("Nen tang: ASP.NET Core 10.0 . Razor Pages . SQL Server . Docker")
$selection.TypeParagraph()
$selection.TypeText("Phien ban: 1.0 . Ngay: 14/07/2026")
$selection.TypeParagraph()

$doc.PageSetup.SectionStart = 3

# Section 1
Add-Heading "1. Tong Quan He Thong" 1
Add-Para "MyCar Showroom la he thong quan ly showroom xe hoi cao cap, duoc xay dung tren nen tang ASP.NET Core 10.0 su dung Razor Pages. He thong cung cap giao dien nguoi dung sang trong phong cach Mercedes-Benz, ket hop voi quan tri noi dung CRUD day du cho Admin, dashboard doanh thu cho Quan Ly showroom, va trai nghiem mua sam xe hoi truc tuyen cho nguoi dung cuoi."

Add-Para "Thong so ky thuat:"
Add-Table @("Thanh phan", "Mo ta") @(
    @("Ngon ngu & Framework", "C# 13, ASP.NET Core 10.0, .NET 10.0"),
    @("Kien truc", "Razor Pages (chinh) + Minimal APIs + MVC (phu)"),
    @("Co so du lieu", "SQL Server 2022 (Docker) - 14 bang"),
    @("ORM", "Entity Framework Core 8.0"),
    @("Xac thuc", "Session Cookie + JWT Bearer (song song)"),
    @("Phan quyen", "Middleware tuy chinh - 3 vai tro: Admin / Quan Ly / User"),
    @("Port", "HTTP 5001 (chay tren localhost + LAN)"),
    @("Container", "Docker container sqlserver-carproject (SQL Server)"),
    @("Hosting", ".NET Aspire AppHost + OpenTelemetry"),
    @("Giao dien", "Bootstrap 5 + CSS Luxury (glassmorphism, dark/light theme)"),
    @("Logging", "Serilog (file + console) + Database (NhatKyHeThong)")
)

# Section 2
Add-Heading "2. Kien Truc He Thong" 1
Add-Para "He thong duoc to chuc theo mo hinh middleware pipeline cua ASP.NET Core. Luong xu ly yeu cau di qua chuoi middleware duoc dang ky theo thu tu trong Program.cs."

Add-Para "Luong Middleware:"
Add-Table @("Buoc", "Middleware", "Chuc nang") @(
    @("1", "Serilog Request Logging", "Ghi log tat ca HTTP request"),
    @("2", "UTF-8 Enforcer", "Dam bao charset UTF-8 cho response"),
    @("3", "Aspire Health Checks", "Endpoint /health, /alive"),
    @("4", "Exception Handler", "Xu ly loi (Detail -> Dev, generic -> Prod)"),
    @("5", "Routing", "Xac dinh route cho request"),
    @("6", "Request Logging", "Ghi thoi gian xu ly request"),
    @("7", "Session", "Server-side session (30 phut timeout)"),
    @("8", "Authentication", "Cookie + JWT"),
    @("9", "Authorization", "Authorize filter"),
    @("10", "Custom Path Auth", "Kiem tra quyen theo path (/Admin/*, /QuanLy/*)"),
    @("11", "Static Files", "wwwroot assets"),
    @("12", "Minimal APIs + RP + MVC", "Xu ly request den endpoint")
)

# Section 3
Add-Heading "3. Xac Thuc va Phan Quyen" 1
Add-Heading "3.1. Co che dang nhap" 2
Add-Para "He thong ho tro hai co che xac thuc song song:"

Add-Para "1. Session Cookie (Razor Pages): Sau khi dang nhap thanh cong, thong tin nguoi dung (UserName, UserRole, AvatarUrl, Email, MaTaiKhoan) duoc luu vao session server-side. Session cookie HttpOnly co thoi gian song 30 phut."
Add-Para "2. JWT Bearer (API Clients): Endpoint POST /api/login nhan {tenDangNhap, matKhau} va tra ve JWT token (HMAC-SHA256, 30 phut). Token chua claims: NameIdentifier, Name, Role, AvatarUrl, Email, MaTaiKhoan."
Add-Para "3. Google OAuth 2.0: Dang nhap qua Google (callback /Account/GoogleCallback). Neu email chua ton tai trong DB, tu dong tao tai khoan moi voi vai tro User. Sau dang nhap, chuyen huong theo vai tro."

Add-Heading "3.2. Phan quyen" 2
Add-Table @("Vai tro", "Quyen han") @(
    @("Admin", "Truy cap toan bo /Admin/* va tat ca CRUD (13 chuc nang)"),
    @("Quan Ly", "Truy cap /QuanLy/Dashboard - xem doanh thu showroom duoc phan cong"),
    @("User", "Trang public: dat coc, dat lai thu, so sanh xe, xem ho so")
)

Add-Para "Co che bao ve path-based:"
Add-Bullet "/Admin/* - chi Admin moi truy cap duoc"
Add-Bullet "/Orders/*, /Profile, /TestDrive/*, /QuanLy/* - yeu cau dang nhap"
Add-Bullet "Khong co quyen: tra ve 403 (Admin) hoac redirect den /Account/Login"

# Section 4
Add-Heading "4. Co So Du Lieu" 1
Add-Para "He thong su dung SQL Server 2022 chay tren Docker container. Database CarShopDb gom 14 bang du lieu."

Add-Table @("Bang", "Mo ta") @(
    @("HangXe", "Hang xe (vd: Mercedes-Benz)"),
    @("DongXe", "Dong xe (FK -> HangXe)"),
    @("PhienBanXe_SanPham", "Phien ban xe (FK -> DongXe)"),
    @("TaiKhoan", "Tai khoan nguoi dung"),
    @("ChiTietKhachHang", "Chi tiet khach hang"),
    @("DonDatCoc", "Don dat coc (FK -> TaiKhoan, PhienBanXe)"),
    @("HoaDonMuaXe", "Hoa don mua xe (FK -> DonDatCoc)"),
    @("LichHenLaiThu", "Lich hen lai thu (FK -> TaiKhoan, DongXe, ChiNhanh)"),
    @("ChiNhanhShowroom", "Chi nhanh showroom (FK -> TaiKhoan Quan Ly)"),
    @("ChuongTrinhKhuyenMai", "Chuong trinh khuyen mai"),
    @("QuangCaoBanner", "Banner quang cao (FK -> TaiKhoan)"),
    @("KenhTuVan", "Kenh tu van (Messenger, Zalo, SMS)"),
    @("NhatKyHeThong", "Nhat ky hoat dong (FK -> TaiKhoan)"),
    @("ThongKeTongHop_Boss", "Thong ke tong hop (FK -> ChiNhanh, DongXe)")
)

# Section 5
Add-Heading "5. Chuc Nang Chinh" 1
Add-Heading "5.1. Trang cong khai (User)" 2
Add-Table @("Trang", "Chuc nang") @(
    @("Trang chu (/Index)", "Hero fullscreen, thuong hieu, xe tieu bieu, danh gia khach hang"),
    @("Danh sach xe (/Cars)", "Bo loc (hang, kieu than xe), tim kiem, sap xep"),
    @("Chi tiet xe (/Details)", "Gallery, thong so ky thuat, phien ban lien quan, CTA"),
    @("So sanh (/Compare)", "So sanh toi da 3 xe side-by-side"),
    @("Lai thu (/TestDrive)", "Dat lich lai thu (ghi DB LichHenLaiThu)"),
    @("Dat coc (/Orders/DepositForm)", "Dat coc xe (yeu cau dang nhap)"),
    @("Ho so (/Profile)", "Xem/sua thong tin, doi mat khau, upload avatar"),
    @("Lien he (/Contact)", "Form lien he (ghi log)")
)

Add-Heading "5.2. Quan tri (Admin)" 2
Add-Table @("Chuc nang", "Mo ta") @(
    @("Dashboard", "13 the thong ke: tong xe, don hang, doanh thu, nguoi dung..."),
    @("Hang xe / Dong xe / Phien ban", "CRUD quan ly danh muc xe"),
    @("Nguoi dung", "Danh sach + phan quyen (User <-> Quan Ly), gan showroom"),
    @("Chi nhanh Showroom", "CRUD chi nhanh + phan cong Quan Ly"),
    @("Don coc / Hoa don / Lich hen", "Quan ly giao dich"),
    @("Khuyen mai / Banner", "CRUD noi dung quang cao"),
    @("Thong ke doanh thu", "Chart.js: bieu do duong (theo ngay) + cot (theo showroom)"),
    @("Nhat ky he thong", "Tra cuu log hoat dong (phan trang, tim kiem)")
)

Add-Heading "5.3. Dashboard Quan Ly Showroom" 2
Add-Para "Quan Ly la vai tro trung gian giua Admin va User. Moi Quan Ly duoc gan mot chi nhanh showroom cu the. Dashboard hien thi: doanh thu hom nay, doanh thu thang nay, tong so don da xu ly, bieu do duong 30 ngay, danh sach doanh thu chi tiet theo ngay."

# Section 6
Add-Heading "6. Bao Mat He Thong" 1
Add-Table @("Bien phap", "Mo ta") @(
    @("Session Cookie HttpOnly", "Chong truy cap tu JavaScript (XSS)"),
    @("Anti-Forgery Token", "Mac dinh tren tat ca POST handler Razor Pages"),
    @("Phan quyen middleware", "Chan path-based: /Admin/*, /Orders/*, /QuanLy/*"),
    @("EF Core Parameterized", "Chong SQL injection"),
    @("403 Forbidden", "Tra ve loi 403 cho truy cap Admin trai phep"),
    @("Exception Handler", "An chi tiet loi o production"),
    @("Logging day du", "Tat ca hanh dong deu ghi log + Serilog"),
    @("JWT HMAC-SHA256", "Token ky so voi secret key 32 ky tu"),
    @("Read-only SQL user", "Tai khoan AppReader bi deny SELECT tren bang nhay cam")
)

Add-Para "Luu y bao mat can cai thien:"
Add-Bullet "Mat khau dang luu dang plain text, chua co hashing (bcrypt/PBKDF2)"
Add-Bullet "Chua su dung ASP.NET Identity - tu quan ly session thu cong"

# Section 7
Add-Heading "7. Trieu Khai" 1
Add-Table @("Thanh phan", "Mo ta") @(
    @("Web Server", "Kestrel (self-host)"),
    @("Port", "HTTP 5001 (0.0.0.0 -> truy cap LAN)"),
    @("Database", "Docker container sqlserver-carproject (localhost:1433)"),
    @(".NET Version", "10.0 (net10.0)"),
    @("Container Runtime", "mcr.microsoft.com/dotnet/aspnet:10.0"),
    @("Orchestration", ".NET Aspire AppHost + ServiceDefaults"),
    @("Monitoring", "OpenTelemetry (metrics, tracing), health checks"),
    @("Ky so DLL", "Post-build PowerShell script (bypass WDAC)")
)

# Section 8
Add-Heading "8. Giao Dien Nguoi Dung" 1
Add-Bullet "Chu de: Dark/Light Theme (Mercedes-Benz luxury, glassmorphism)"
Add-Bullet "Hieu ung chuyen theme: rem hai ben (curtain animation)"
Add-Bullet "CSS: Bootstrap 5 grid + custom CSS (3000+ dong)"
Add-Bullet "Font: Inter (body), Montserrat (nav), Playfair Display / Cormorant Garamond (brand)"
Add-Bullet "Responsive: Desktop, Tablet, Mobile"
Add-Bullet "Icon: Bootstrap Icons + Font Awesome 6"
Add-Bullet "Bieu do: Chart.js 4.4.1 (CDN)"

# Save
$outputPath = "D:\Code\Code\WebMVC\CarProject\BaoCao_HeThong_MyCar.docx"
$doc.SaveAs2([ref]$outputPath, [ref]16)
$word.Quit()
Write-Output "OK"
