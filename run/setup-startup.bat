@echo off
chcp 65001 >nul
title Setup Auto-Start - CarProject
echo ============================================
echo    Cai dat tu dong chay cung Windows
echo ============================================
echo.

:: Them vao Startup folder
set STARTUP_DIR=%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup
copy /Y "%~dp0carproject-startup.vbs" "%STARTUP_DIR%\CarProject.vbs" >nul
echo OK - Da them vao Startup folder
echo.

:: Xoa service cu (neu co)
sc stop CarProject >nul 2>&1
sc delete CarProject >nul 2>&1
sc stop cloudflared >nul 2>&1
sc delete cloudflared >nul 2>&1
echo OK - Da xoa service cu
echo.

:: Kiem tra Docker auto-start
echo Nho bat Docker Desktop: Settings ^> General ^> Start when you sign in
echo.
echo ============================================
echo Xong! Tu lan sau mo may:
echo - Docker tu chay
echo - CarProject + Cloudflare tu dong chay ngam
echo - https://mylxcar.online hoat dong luon
echo.
echo Chu y: Lan nay chay reset.bat de khoi dong luon
echo ============================================
pause
