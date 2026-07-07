@echo off
cd /d "%~dp0"
dotnet run --project CarProject\CarProject\CarProject.csproj --launch-profile http
