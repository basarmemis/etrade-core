#!/bin/bash

# scripti çalışır hale getirmek için 
# chmod +x etrade-core.api/scripts/db/migrateandupdate.sh 
# komutunu çalıştırın.

# E-Trade Core Database Migration and Update Script
# Bu script hem DomainDbContext hem de ApplicationDbContext için migration ve update işlemlerini yapar

echo "🚀 E-Trade Core Database Migration and Update Script"
echo "=================================================="

# Proje dizinini al
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$(dirname "$(dirname "$PROJECT_DIR")")")"

echo "📁 Proje dizini: $PROJECT_ROOT"
echo ""

# Renk kodları
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Hata kontrolü fonksiyonu
check_error() {
    if [ $? -ne 0 ]; then
        echo -e "${RED}❌ Hata: $1${NC}"
        exit 1
    fi
}

# Başarı mesajı fonksiyonu
success_message() {
    echo -e "${GREEN}✅ $1${NC}"
}

# Bilgi mesajı fonksiyonu
info_message() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

# Uyarı mesajı fonksiyonu
warning_message() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

# Proje dizinine git
cd "$PROJECT_ROOT"
check_error "Proje dizinine geçilemedi"

# Proje build kontrolü
echo "🔨 Proje build kontrolü yapılıyor..."
dotnet build --no-restore
check_error "Proje build edilemedi"
success_message "Proje başarıyla build edildi"

echo ""
echo "📦 Migration işlemleri başlatılıyor..."
echo "====================================="

# DomainDbContext için migration oluştur
echo ""
info_message "DomainDbContext için migration oluşturuluyor..."

# Migration adı için timestamp kullan
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
MIGRATION_NAME="Migration_${TIMESTAMP}"

dotnet ef migrations add "$MIGRATION_NAME" \
    --project etrade-core.persistence \
    --startup-project etrade-core.api \
    --context DomainDbContext \
    --output-dir Migrations/Domain

if [ $? -eq 0 ]; then
    success_message "DomainDbContext migration oluşturuldu: $MIGRATION_NAME"
else
    warning_message "DomainDbContext için yeni migration gerekmiyor veya hata oluştu"
fi

# ApplicationDbContext için migration oluştur
echo ""
info_message "ApplicationDbContext için migration oluşturuluyor..."

dotnet ef migrations add "$MIGRATION_NAME" \
    --project etrade-core.persistence \
    --startup-project etrade-core.api \
    --context ApplicationDbContext \
    --output-dir Migrations/Identity

if [ $? -eq 0 ]; then
    success_message "ApplicationDbContext migration oluşturuldu: $MIGRATION_NAME"
else
    warning_message "ApplicationDbContext için yeni migration gerekmiyor veya hata oluştu"
fi

echo ""
echo "🔄 Veritabanı güncelleme işlemleri başlatılıyor..."
echo "================================================"

# DomainDbContext için database update
echo ""
info_message "DomainDbContext veritabanı güncelleniyor..."

dotnet ef database update \
    --project etrade-core.persistence \
    --startup-project etrade-core.api \
    --context DomainDbContext

if [ $? -eq 0 ]; then
    success_message "DomainDbContext veritabanı başarıyla güncellendi"
else
    echo -e "${RED}❌ DomainDbContext veritabanı güncellenirken hata oluştu${NC}"
    exit 1
fi

# ApplicationDbContext için database update
echo ""
info_message "ApplicationDbContext veritabanı güncelleniyor..."

dotnet ef database update \
    --project etrade-core.persistence \
    --startup-project etrade-core.api \
    --context ApplicationDbContext

if [ $? -eq 0 ]; then
    success_message "ApplicationDbContext veritabanı başarıyla güncellendi"
else
    echo -e "${RED}❌ ApplicationDbContext veritabanı güncellenirken hata oluştu${NC}"
    exit 1
fi

echo ""
echo "🎉 Tüm işlemler başarıyla tamamlandı!"
echo "====================================="
echo ""
echo "📋 Özet:"
echo "  ✅ Proje build edildi"
echo "  ✅ Migration'lar oluşturuldu (gerekirse)"
echo "  ✅ DomainDbContext veritabanı güncellendi"
echo "  ✅ ApplicationDbContext veritabanı güncellendi"
echo ""
echo "🚀 API projenizi çalıştırabilirsiniz:"
echo "  dotnet run --project etrade-core.api"
echo "" 