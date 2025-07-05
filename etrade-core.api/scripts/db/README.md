# Database Migration Scripts

Bu klasör, E-Trade Core projesinin veritabanı migration ve update işlemlerini otomatikleştiren script'leri içerir.

## 📁 Dosyalar

- `migrateandupdate.sh` - Linux/macOS için shell script
- `migrateandupdate.bat` - Windows için batch script
- `README.md` - Bu dosya

## 🚀 Kullanım

### Linux/macOS

```bash
# Script'i çalıştırılabilir hale getir (sadece ilk kez)
chmod +x etrade-core.api/scripts/db/migrateandupdate.sh

# Script'i çalıştır
./etrade-core.api/scripts/db/migrateandupdate.sh
```

### Windows

```cmd
# Script'i çalıştır
etrade-core.api\scripts\db\migrateandupdate.bat
```

## 🔧 Script'in Yaptığı İşlemler

1. **Proje Build Kontrolü**
   - Projenin başarıyla build edilip edilmediğini kontrol eder
   - Build hatası varsa işlemi durdurur

2. **Migration Oluşturma**
   - DomainDbContext için migration oluşturur (gerekirse)
   - ApplicationDbContext için migration oluşturur (gerekirse)
   - Migration adı otomatik olarak timestamp ile oluşturulur

3. **Veritabanı Güncelleme**
   - DomainDbContext veritabanını günceller
   - ApplicationDbContext veritabanını günceller
   - Hata durumunda işlemi durdurur

## 📋 Çıktı

Script çalıştığında aşağıdaki bilgileri gösterir:

- ✅ Başarılı işlemler (yeşil)
- ⚠️ Uyarılar (sarı)
- ❌ Hatalar (kırmızı)
- ℹ️ Bilgi mesajları (mavi)

## 🎯 Özellikler

- **Otomatik Timestamp**: Migration adları otomatik olarak timestamp ile oluşturulur
- **Hata Kontrolü**: Her adımda hata kontrolü yapılır
- **Renkli Çıktı**: Farklı mesaj türleri için renkli çıktı
- **Çift Context Desteği**: Hem DomainDbContext hem de ApplicationDbContext için çalışır
- **Platform Desteği**: Linux/macOS ve Windows için ayrı script'ler

## ⚠️ Önemli Notlar

- Script'i çalıştırmadan önce PostgreSQL veritabanının çalıştığından emin olun
- Connection string'lerin doğru olduğundan emin olun
- Gerekli NuGet paketlerinin yüklü olduğundan emin olun

## 🔄 Manuel Migration İşlemleri

Eğer script kullanmak istemiyorsanız, aşağıdaki komutları manuel olarak çalıştırabilirsiniz:

### DomainDbContext için:

```bash
# Migration oluştur
dotnet ef migrations add MigrationName --project etrade-core.persistence --startup-project etrade-core.api --context DomainDbContext --output-dir Migrations/Domain

# Veritabanını güncelle
dotnet ef database update --project etrade-core.persistence --startup-project etrade-core.api --context DomainDbContext
```

### ApplicationDbContext için:

```bash
# Migration oluştur
dotnet ef migrations add MigrationName --project etrade-core.persistence --startup-project etrade-core.api --context ApplicationDbContext --output-dir Migrations/Identity

# Veritabanını güncelle
dotnet ef database update --project etrade-core.persistence --startup-project etrade-core.api --context ApplicationDbContext
``` 