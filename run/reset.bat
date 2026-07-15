@echo off
chcp 65001 >nul
title Reset - CarProject
echo ============================================
echo    Reset toan bo CarProject
echo ============================================
echo.

:: Kill CarProject dang chay (neu co)
echo [1/4] Tat CarProject cu...
taskkill /f /fi "WINDOWTITLE eq CarProject*" 2>nul
taskkill /f /im dotnet.exe 2>nul
timeout /t 2 /nobreak >nul
echo OK

:: Kill cloudflared cu
echo [2/4] Tat cloudflared cu...
taskkill /f /im cloudflared.exe 2>nul
timeout /t 1 /nobreak >nul
echo OK

:: Khoi dong lai Docker container
echo [3/4] Khoi dong lai Docker SQL Server...
docker restart sqlserver-carproject 2>nul
if %errorLevel% neq 0 (
    echo Docker chua chay. Dang mo Docker Desktop...
    start "" "C:\Program Files\Docker\Docker\Docker Desktop.exe"
    timeout /t 15 /nobreak >nul
    docker start sqlserver-carproject 2>nul
)
echo OK

:: Khoi dong CarProject app + Cloudflare tunnel
echo [4/4] Khoi dong CarProject + Cloudflare tunnel...

:: Chay CarProject ngam (an cua so)
start /B /MIN "" "%~dp0run-carproject.cmd"

:: Doi app chay xong moi mo tunnel
timeout /t 10 /nobreak >nul

:: Chay cloudflared ngam (an cua so)
start /B /MIN "" "C:\Program Files (x86)\cloudflared\cloudflared.exe" tunnel --url http://localhost:5001 run car-tunnel

echo OK
echo.
echo ============================================
echo Chay het roi! Kiem tra:
echo - http://localhost:5001
echo - http://100.108.48.1:5001 (Tailscale)
echo - https://mylxcar.online   (Cloudflare)
echo ============================================
echo.
echo Nhan phim bat ky de dong...
pause >nul
