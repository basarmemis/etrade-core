@echo off
setlocal enabledelayedexpansion

REM E-Trade Core Database Migration and Update Script (Windows)
REM Bu script hem DomainDbContext hem de ApplicationDbContext için migration ve update işlemlerini yapar

echo 🚀 E-Trade Core Database Migration and Update Script
echo ==================================================

REM Proje dizinini al
set "SCRIPT_DIR=%~dp0"
set "PROJECT_ROOT=%SCRIPT_DIR%..\..\.."

echo 📁 Proje dizini: %PROJECT_ROOT%
echo.

REM Proje dizinine git
cd /d "%PROJECT_ROOT%"
if errorlevel 1 (
    echo ❌ Hata: Proje dizinine geçilemedi
    exit /b 1
)

REM Proje build kontrolü
echo 🔨 Proje build kontrolü yapılıyor...
dotnet build --no-restore
if errorlevel 1 (
    echo ❌ Hata: Proje build edilemedi
    exit /b 1
)
echo ✅ Proje başarıyla build edildi

echo.
echo 📦 Migration işlemleri başlatılıyor...
echo =====================================

REM Migration adı için timestamp kullan
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "TIMESTAMP=%dt:~0,8%_%dt:~8,6%"
set "MIGRATION_NAME=Migration_%TIMESTAMP%"

REM DomainDbContext için migration oluştur
echo.
echo ℹ️  DomainDbContext için migration oluşturuluyor...

dotnet ef migrations add "%MIGRATION_NAME%" ^
    --project etrade-core.persistence ^
    --startup-project etrade-core.api ^
    --context DomainDbContext ^
    --output-dir Migrations/Domain

if errorlevel 1 (
    echo ⚠️  DomainDbContext için yeni migration gerekmiyor veya hata oluştu
) else (
    echo ✅ DomainDbContext migration oluşturuldu: %MIGRATION_NAME%
)

REM ApplicationDbContext için migration oluştur
echo.
echo ℹ️  ApplicationDbContext için migration oluşturuluyor...

dotnet ef migrations add "%MIGRATION_NAME%" ^
    --project etrade-core.persistence ^
    --startup-project etrade-core.api ^
    --context ApplicationDbContext ^
    --output-dir Migrations/Identity

if errorlevel 1 (
    echo ⚠️  ApplicationDbContext için yeni migration gerekmiyor veya hata oluştu
) else (
    echo ✅ ApplicationDbContext migration oluşturuldu: %MIGRATION_NAME%
)

echo.
echo 🔄 Veritabanı güncelleme işlemleri başlatılıyor...
echo ================================================

REM DomainDbContext için database update
echo.
echo ℹ️  DomainDbContext veritabanı güncelleniyor...

dotnet ef database update ^
    --project etrade-core.persistence ^
    --startup-project etrade-core.api ^
    --context DomainDbContext

if errorlevel 1 (
    echo ❌ DomainDbContext veritabanı güncellenirken hata oluştu
    exit /b 1
)
echo ✅ DomainDbContext veritabanı başarıyla güncellendi

REM ApplicationDbContext için database update
echo.
echo ℹ️  ApplicationDbContext veritabanı güncelleniyor...

dotnet ef database update ^
    --project etrade-core.persistence ^
    --startup-project etrade-core.api ^
    --context ApplicationDbContext

if errorlevel 1 (
    echo ❌ ApplicationDbContext veritabanı güncellenirken hata oluştu
    exit /b 1
)
echo ✅ ApplicationDbContext veritabanı başarıyla güncellendi

echo.
echo 🎉 Tüm işlemler başarıyla tamamlandı!
echo =====================================
echo.
echo 📋 Özet:
echo   ✅ Proje build edildi
echo   ✅ Migration'lar oluşturuldu (gerekirse)
echo   ✅ DomainDbContext veritabanı güncellendi
echo   ✅ ApplicationDbContext veritabanı güncellendi
echo.
echo 🚀 API projenizi çalıştırabilirsiniz:
echo   dotnet run --project etrade-core.api
echo.

pause 