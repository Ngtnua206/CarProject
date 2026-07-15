# Chạy bằng PowerShell Admin (Right-click → Run as Administrator)

# Kiểm tra admin
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "Vui lòng chạy với quyền Administrator!" -ForegroundColor Red
    exit 1
}

# 1. Cài CarProject thành Windows Service
Write-Host "=== Tạo service CarProject ===" -ForegroundColor Cyan
$projectPath = "D:\Code\Code\WebMVC\CarProject\CarProject\CarProject.csproj"
$binPath = "dotnet.exe run --project `"$projectPath`" --launch-profile http"

sc.exe create CarProject binPath=$binPath start=auto displayName="CarProject Web App"
sc.exe description CarProject "CarProject ASP.NET Core web application"
Write-Host "OK" -ForegroundColor Green

# 2. Set cloudflared service start = auto
Write-Host "=== Cấu hình cloudflared service ===" -ForegroundColor Cyan
$cfdPath = "C:\Program Files (x86)\cloudflared\cloudflared.exe"
if (Test-Path $cfdPath) {
    sc.exe create cloudflared binPath="$cfdPath tunnel run car-tunnel" start=auto displayName="Cloudflare Tunnel"
    Write-Host "OK - Đã tạo service cloudflared" -ForegroundColor Green
    Write-Host "NHƯNG bạn cần chạy cloudflared tunnel login + setup tunnel trước:" -ForegroundColor Yellow
    Write-Host "  1. & `"$cfdPath`" tunnel login" -ForegroundColor Yellow
    Write-Host "  2. & `"$cfdPath`" tunnel create car-tunnel" -ForegroundColor Yellow
    Write-Host "  3. & `"$cfdPath`" tunnel route dns car-tunnel <domain-của-ban>" -ForegroundColor Yellow
    Write-Host "Sau đó mới start service cloudflared được." -ForegroundColor Yellow
} else {
    Write-Host "cloudflared chưa cài. Cài bằng: winget install cloudflare.cloudflared" -ForegroundColor Red
}

# 3. Hướng dẫn Docker Desktop
Write-Host "=== Docker Desktop ===" -ForegroundColor Cyan
Write-Host "Vào Docker Desktop → Settings → General → bật 'Start Docker Desktop when you log in'" -ForegroundColor Yellow
Write-Host "Container đã có restart: unless-stopped → tự chạy khi Docker mở" -ForegroundColor Green

# 4. Start service
Write-Host "=== Khởi động service CarProject ===" -ForegroundColor Cyan
sc.exe start CarProject

Write-Host ""
Write-Host "=== Hoàn tất! ===" -ForegroundColor Green
Write-Host "Sau khi restart máy: Docker chạy → SQL Server chạy → CarProject chạy → cloudflared (nếu setup) chạy" -ForegroundColor Green
Write-Host "Truy cập: https://<domain-của-ban> (qua cloudflare) hoặc http://100.108.48.1:5001 (qua tailscale)" -ForegroundColor Green
