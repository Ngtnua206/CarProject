@echo off
chcp 65001 >nul
title Setup Services - CarProject
echo ============================================
echo    Thiet lap Service tu dong cho CarProject
echo    Chay voi quyen Administrator
echo ============================================
echo.

:: Kiểm tra admin
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Vui long chay file nay voi quyen Administrator!
    echo Click phai chuot - Run as administrator
    pause
    exit /b 1
)

set SCRIPT_DIR=%~dp0

:: 1. Tao Service cho CarProject
echo [1/3] Dang tao service CarProject...
sc stop CarProject >nul 2>&1
sc delete CarProject >nul 2>&1
sc create CarProject binPath="%SCRIPT_DIR%run-carproject.cmd" start=auto displayName="CarProject Web App"
sc description CarProject "CarProject ASP.NET Core web application"
echo OK - Service CarProject da duoc tao
echo.

:: 2. Tao Service cho Cloudflare Tunnel
echo [2/3] Dang tao service cloudflared...
sc stop cloudflared >nul 2>&1
sc delete cloudflared >nul 2>&1

set CF_BIN="C:\Program Files (x86)\cloudflared\cloudflared.exe"
if exist %CF_BIN% (
    sc create cloudflared binPath="%CF_BIN% tunnel run car-tunnel" start=auto displayName="Cloudflare Tunnel"
    echo OK - Service cloudflared da duoc tao
) else (
    echo KHONG tim thay cloudflared. Cai bang: winget install cloudflare.cloudflared
)
echo.

:: 3. Bat Docker Desktop tu dong chay cung Windows
echo [3/3] Docker Desktop - vao Settings bat auto-start...
echo Vao Docker Desktop ^> Settings ^> General ^> 'Start Docker Desktop when you sign in'
echo Container da co restart: unless-stopped
echo.

:: 4. Khoi dong services
echo Dang khoi dong services...
sc start CarProject >nul 2>&1
echo OK - CarProject da chay ngam
echo.
echo ============================================
echo Hoan tat! Sau khi reboot may:
echo - Tat ca deu tu dong chay
echo - https://mylxcar.online
echo - http://100.108.48.1:5001 (Tailscale)
echo.
echo De reset nhanh: reset.bat
echo ============================================
pause
