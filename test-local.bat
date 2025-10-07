@echo off
echo Testing CouponHub API locally...
echo.

echo Building the application...
dotnet build CouponHub.Api/CouponHub.Api.csproj
if %ERRORLEVEL% neq 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo Build successful! Starting the application...
echo.

cd CouponHub.Api
dotnet run --urls "http://localhost:5000"
