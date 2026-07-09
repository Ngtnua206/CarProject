# Agents Summary

## Goal
- Build luxury car showroom website with full UI/UX + admin CRUD using ASP.NET Core / Razor Pages / SQL Server Docker, integrating friend's Mercedes-Benz React design; added Quản Lý role with showroom revenue dashboard + admin revenue chart (line + bar via Chart.js)

## Constraints & Preferences
- Primary theme: Mercedes-Benz dark luxury (black/silver, glassmorphism, premium buttons)
- Bootstrap 5 grid + custom CSS; no Tailwind
- Session-based auth (no ASP.NET Identity); roles: Admin / Quản Lý / User
- SQL Server via Docker (localhost:1433, SA password `Iumaioanhh@2024`)
- App runs on `http://0.0.0.0:5001` HTTP; accessed via LAN IP `192.168.1.5:5001`
- Run from: `dotnet run --project CarProject\CarProject\CarProject.csproj` in `D:\Code\Code\WebMVC`
- Registration is Google-only (no manual form); password field has eye toggle
- User wants to use **Aspire** hosting (VS default), not just cmd
- Avatar upload requires client-side crop modal (zoom + drag)

## Progress
### Done
- Added `Models/NhatKyHeThong` entity for activity logging with fields: MaTaiKhoan, TenDangNhap, VaiTro, HanhDong, ChiTiet, DiaChiIP, TrinhDuyet, DuongDan, ThoiGian
- Added `NhatKyHeThong` DbSet and table mapping in AppDbContext
- Created migration `AddNhatKyHeThong` for new log table
- Created `Services/ActivityLogService.cs` (IActivityLogService + ActivityLogService) with IHttpContextAccessor
- Registered ActivityLogService + HttpContextAccessor in Program.cs
- Added `await _log.LogAsync(...)` to all page handlers
- Created `Pages/Admin/Logs/Index.cshtml + .cs` with search/filter, paginated table
- Updated `Pages/Admin/Index.cshtml + .cs` with TotalLogs count card + link to Logs viewer
- Added request logging middleware in Program.cs
- Rewrote `wwwroot/css/site.css` with luxury automotive theme, premium components, animations
- Rewrote `Views/Shared/_Layout.cshtml` with premium navbar, floating contact buttons, footer
- Rewrote `Pages/Index.cshtml` luxury homepage: hero-fullscreen, brand grid, car grid, testimonials
- Created `Pages/Cars.cshtml + .cs`: car listing with filter sidebar, search, sort
- Created `Pages/Details.cshtml + .cs`: detail page with gallery, specs, CTAs
- Rewrote `Pages/Account/Login.cshtml` with premium card layout, demo credentials
- Rewrote `Pages/Orders/DepositForm.cshtml` with 2-column layout
- Updated `Pages/Index.cshtml.cs` to remove BannerList (simplification)
- Removed stale `Controllers/AccountController.cs` (moved to Razor Pages)
- Admin avatar upload fixed via `/api/upload-avatar-admin` endpoint (session-independent)
- User account delete with transaction-based clean up (parents: Nhật ký, Đơn cọc, Lịch hẹn, Banner)
- TempData["Success"] + TempData["Error"] displayed on all admin CRUD pages
- **Quản Lý role + Showroom doanh thu**:
  - New `Pages/QuanLy/Dashboard.cshtml + .cs`: manager dashboard with assigned showroom info, daily revenue stats, 30-day line chart (Chart.js)
  - New `Pages/Admin/ThongKe/DoanhThu.cshtml + .cs`: admin revenue page with date picker + showroom filter, daily line chart + showroom bar chart
  - Auth middleware updated: `/QuanLy/*` requires login; `Quản Lý` role redirects to `/QuanLy/Dashboard` after login
  - Admin sidebar + main layout dropdown updated for Quản Lý navigation

### In Progress
- (none)

### Blocked
- (none)

## Key Decisions
- Use emoji + Bootstrap Icons for car placeholders (no real car images available)
- Separate Cars listing page from Index homepage (instead of one page for both)
- Filter sidebar uses radio buttons + form submit for simplicity (no JavaScript filtering)
- Floating contact buttons fixed to bottom-right across all pages
- Dark theme hero, light theme content sections to match luxury automotive sites
- Use Razor Pages instead of MVC Controllers for page-based routes
- Remove stale Controller files in favor of Razor Pages
- Admin area uses Bootstrap (not luxury theme) for functional CRUD UX
- **JWT authentication** added alongside existing session auth:
  - Session (cookie) → Razor Pages (existing, unchanged)
  - JWT (Bearer `/api/login`) → API clients / React frontend
  - JWT key: `appsettings.json → Jwt:Key`, expiry: 30 phút
  - Token lưu trong session (`JwtToken`) khi login qua form, có sẵn cho API calls sau đó

## Next Steps
- Try running Aspire AppHost with the signed DLL — if signing doesn't resolve WDAC block, disable Memory Integrity (Windows Security → Device Security → Core Isolation → Off → reboot)
- If Aspire still fails, use `run.bat` or VS Code launch config "CarProject (http)" for daily work
- Commit + push clean code to GitHub

## Critical Context
- Build: 0 errors, CS8618 nullable warnings (same pattern, no functional impact)
- DB: `CarShopDb`; server `localhost,1433`; SA password `Iumaioanhh@2024`
- **WDAC/Smart App Control fix**: Self-signed code signing cert created, CarProject.dll signed automatically after each build via post-build event in `.csproj`. Cert installed in LocalMachine\Root + LocalMachine\TrustedPublisher, CurrentUser\My, CurrentUser\TrustedPublisher. Post-build script: `build/sign-after-build.ps1`.
- **Smart App Control** is in "Enforce" mode (Windows Insider build). If signing doesn't help, turn off: Windows Settings → Privacy & security → Windows Security → App & browser control → Smart App Control → Off
- **Memory Integrity** (Virtualization-based security) is enabled. If needed, disable: Windows Security → Device Security → Core Isolation → Memory Integrity → Off → reboot
- Port 5001 often held by stale `dcp.exe` (Aspire DCP) after previous run; kill with `taskkill /F /IM dcp.exe`
- `.vscode/launch.json` has two configurations: "Aspire AppHost" (with `postDebugTask: kill-dcp`) and "CarProject (http)"
- `builder.Build().Run();` was added to `AppHost.cs` (was missing, causing immediate exit)
- `setup-cert.bat` at workspace root — for other developers who need signing (run as admin)
- `run.bat` at workspace root — quick launch: `dotnet run --project CarProject\CarProject\CarProject.csproj --launch-profile http`
- `rebuild-and-run.bat` at workspace root — build + run in one click

## Relevant Files
- `CarProject/wwwroot/css/site.css`: complete luxury theme (all CSS)
- `CarProject/Views/Shared/_Layout.cshtml`: premium layout (navbar + footer + floating buttons)
- `CarProject/Pages/_ViewStart.cshtml`: layout directive for Razor Pages
- `CarProject/Pages/_ViewImports.cshtml`: tag helpers and usings
- `CarProject/Pages/Index.cshtml + .cs`: luxury homepage (hero, brands, featured cars, testimonials)
- `CarProject/Pages/Cars.cshtml + .cs`: car listing with filter sidebar
- `CarProject/Pages/Details.cshtml + .cs`: car detail with specs and CTAs
- `CarProject/Pages/Account/Login.cshtml`: premium login form
- `CarProject/Pages/Orders/DepositForm.cshtml`: deposit form with 2-column layout
- `CarProject/Models/*`: EF Core models (HangXe, DongXe, PhienBanXe, etc.)
- `CarProject/Data/AppDbContext.cs`: database context
- `CarProject/Data/DbInitializer.cs`: seed data
- `CarProject/build/sign-after-build.ps1`: post-build signing script (auto-run)
- `run.bat`: double-click to run
- `rebuild-and-run.bat`: build + run
- `setup-cert.bat`: for other developers to install signing cert
