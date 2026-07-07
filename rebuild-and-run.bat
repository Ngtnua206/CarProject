@echo off
cd /d "%~dp0"
echo [build] Building CarProject...
dotnet build CarProject\CarProject\CarProject.csproj -c Debug --no-restore 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Build failed!
    pause
    exit /b %ERRORLEVEL%
)
echo [run] Starting CarProject...
dotnet run --project CarProject\CarProject\CarProject.csproj --launch-profile http
