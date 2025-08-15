# MultiTenant Infrastructure Documentation

## Overview

Bu dokümantasyon, eTrade Core projesi için kurulan MultiTenant (Çok Kiracılı) altyapısını açıklamaktadır. Bu altyapı, tek bir uygulama instance'ında birden fazla müşteri (tenant) için veri izolasyonu sağlar.

## Mimari Bileşenler

### 1. Core Interfaces

#### ITenantEntity
```csharp
public interface ITenantEntity
{
    string TenantId { get; set; }
}
```
- Tüm tenant-aware entity'ler bu interface'i implement eder
- TenantId property'si ile veri izolasyonu sağlanır

#### ITenantResolver
```csharp
public interface ITenantResolver
{
    Task<string?> ResolveTenantIdAsync();
    string? GetCurrentTenantId();
    void SetCurrentTenantId(string tenantId);
}
```
- Tenant bilgisini farklı kaynaklardan çözer
- Header, subdomain, claim gibi kaynakları destekler

#### ITenantContext
```csharp
public interface ITenantContext
{
    string? CurrentTenantId { get; }
    bool IsMultiTenant { get; }
    void SetCurrentTenant(string tenantId);
    void ClearCurrentTenant();
}
```
- Mevcut request için tenant context'ini yönetir
- Request scope'unda tenant bilgisini saklar

### 2. Tenant Resolution Stratejileri

#### Priority Order (Öncelik Sırası)
1. **Header**: `X-TenantId` header'ı
2. **Subdomain**: `company1.localhost:5000` → `company1`
3. **Claim**: JWT token'daki `TenantId` claim'i
4. **Default**: Varsayılan tenant ID

#### Header-based Resolution
```http
GET /api/products
X-TenantId: company1
```

#### Subdomain-based Resolution
```
company1.localhost:5000 → TenantId: company1
company2.localhost:5000 → TenantId: company2
```

#### Claim-based Resolution
```json
{
  "sub": "user123",
  "TenantId": "company1",
  "exp": 1234567890
}
```

### 3. Entity Framework Core Integration

#### Global Query Filters
```csharp
// Tüm entity'lerde otomatik tenant filtering
entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
```

#### Automatic TenantId Assignment
```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    // SaveChanges sırasında otomatik TenantId ataması
    var entities = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Added && e.Entity is ITenantEntity)
        .Select(e => e.Entity as ITenantEntity);

    foreach (var entity in entities)
    {
        if (entity != null && string.IsNullOrEmpty(entity.TenantId))
        {
            entity.TenantId = GetCurrentTenantId();
        }
    }
}
```

### 4. Dapper Integration

#### Automatic SQL Modification
```csharp
// Orijinal SQL
"SELECT * FROM Products WHERE CategoryId = @CategoryId"

// Tenant filter ile otomatik modifiye edilmiş SQL
"SELECT * FROM Products WHERE CategoryId = @CategoryId AND TenantId = @TenantId"
```

#### Parameter Injection
```csharp
// Orijinal parametreler
new { CategoryId = 1 }

// TenantId ile genişletilmiş parametreler
new { CategoryId = 1, TenantId = "company1" }
```

## Kurulum ve Konfigürasyon

### 1. Program.cs'de Service Registration
```csharp
// Tenant services'leri ekle
builder.Services.AddTenantServices("default");

// Middleware'i ekle
app.UseTenantMiddleware();
```

### 2. DbContext Konfigürasyonu
```csharp
// DomainDbContext'e tenant resolver inject et
services.AddDbContext<DomainDbContext>((provider, options) =>
{
    var tenantResolver = provider.GetService<ITenantResolver>();
    options.UseNpgsql(connectionString);
    // Tenant resolver'ı constructor'a geç
});
```

### 3. Repository Konfigürasyonu
```csharp
// Repository'lere tenant resolver inject et
services.AddScoped<IProductRepository>(provider =>
{
    var connectionFactory = provider.GetRequiredService<IDbConnectionFactory>();
    var tenantResolver = provider.GetRequiredService<ITenantResolver>();
    return new ProductRepository(connectionFactory, tenantResolver);
});
```

## Kullanım Örnekleri

### 1. Controller'da Tenant Bilgisi
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ITenantContext _tenantContext;
    
    public ProductController(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var currentTenant = _tenantContext.CurrentTenantId;
        // Tenant bilgisi otomatik olarak tüm sorgularda kullanılır
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
}
```

### 2. Service'de Tenant Validation
```csharp
public class ProductService : IProductService
{
    private readonly ITenantContext _tenantContext;
    
    public async Task<Product> CreateProductAsync(Product product)
    {
        // TenantId otomatik olarak atanır
        // product.TenantId = _tenantContext.CurrentTenantId;
        
        var createdProduct = await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return createdProduct;
    }
}
```

### 3. Custom Tenant Resolution
```csharp
public class CustomTenantResolver : ITenantResolver
{
    public async Task<string?> ResolveTenantIdAsync()
    {
        // Custom logic for tenant resolution
        // Örnek: Database'den tenant bilgisi çekme
        return await GetTenantFromDatabaseAsync();
    }
}

// Program.cs'de custom resolver'ı kullan
builder.Services.AddTenantServices<CustomTenantResolver>();
```

## Row-Level Security (RLS) Stratejisi

### 1. PostgreSQL RLS (Opsiyonel)
```sql
-- Tenant tablosunda RLS'yi etkinleştir
ALTER TABLE "Products" ENABLE ROW LEVEL SECURITY;

-- Tenant bazlı policy oluştur
CREATE POLICY tenant_isolation_policy ON "Products"
    FOR ALL
    USING (TenantId = current_setting('app.current_tenant_id')::text);

-- Current tenant'ı set et
SELECT set_config('app.current_tenant_id', 'company1', false);
```

### 2. Application-Level RLS
```csharp
// Global query filter ile application seviyesinde RLS
entity.HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
```

### 3. Hybrid Approach
```csharp
// Database RLS + Application filter kombinasyonu
// Database: Güvenlik katmanı
// Application: Performance ve flexibility
```

## Performance Optimizasyonları

### 1. Index Stratejileri
```sql
-- TenantId + diğer sık kullanılan kolonlar için composite index
CREATE INDEX idx_products_tenant_category ON "Products" (TenantId, CategoryId);
CREATE INDEX idx_orders_tenant_status ON "Orders" (TenantId, Status);
```

### 2. Query Optimization
```csharp
// Tenant filter'ı her zaman WHERE clause'un başında
// SQL Server için hint kullanımı
"SELECT * FROM Products WITH (INDEX(IX_Products_TenantId)) WHERE TenantId = @TenantId"
```

### 3. Connection Pooling
```csharp
// Tenant bazlı connection string'ler için
// Her tenant için ayrı connection pool
```

## Güvenlik Considerations

### 1. Tenant Isolation
- Tüm sorgularda TenantId filter'ı zorunlu
- Cross-tenant data access engellenir
- Soft delete ile veri kaybı önlenir

### 2. Authentication & Authorization
```csharp
// JWT token'da TenantId claim'i zorunlu
[Authorize]
public class SecureController : ControllerBase
{
    // Tenant bilgisi otomatik olarak validate edilir
}
```

### 3. Audit Logging
```csharp
// Tenant bazlı audit log
public class AuditLog
{
    public string TenantId { get; set; }
    public string UserId { get; set; }
    public string Action { get; set; }
    public DateTime Timestamp { get; set; }
}
```

## Monitoring ve Debugging

### 1. Tenant Context Logging
```csharp
public class TenantMiddleware
{
    private readonly ILogger<TenantMiddleware> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        var tenantId = await _tenantResolver.ResolveTenantIdAsync();
        _logger.LogInformation("Request processed for tenant: {TenantId}", tenantId);
        
        // ... middleware logic
    }
}
```

### 2. Performance Metrics
```csharp
// Tenant bazlı performance monitoring
public class TenantMetrics
{
    public string TenantId { get; set; }
    public int RequestCount { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public int DatabaseQueryCount { get; set; }
}
```

## Migration Stratejisi

### 1. Existing Data Migration
```sql
-- Mevcut verileri default tenant'a ata
UPDATE "Products" SET "TenantId" = 'default' WHERE "TenantId" IS NULL;
UPDATE "Orders" SET "TenantId" = 'default' WHERE "TenantId" IS NULL;
-- ... diğer tablolar için
```

### 2. Schema Updates
```sql
-- TenantId kolonu ekle
ALTER TABLE "Products" ADD COLUMN "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';
ALTER TABLE "Orders" ADD COLUMN "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';
-- ... diğer tablolar için

-- Unique constraint ekle
CREATE UNIQUE INDEX "IX_Tenants_TenantId" ON "Tenants" ("TenantId");
```

## Best Practices

### 1. Tenant ID Naming Convention
- Kısa ve anlamlı: `company1`, `store_abc`, `client_xyz`
- Lowercase kullan
- Özel karakterlerden kaçın
- Maksimum 50 karakter

### 2. Error Handling
```csharp
public class TenantNotFoundException : Exception
{
    public TenantNotFoundException(string tenantId) 
        : base($"Tenant '{tenantId}' not found or inactive") { }
}

// Service'de kullan
if (!await _tenantService.IsTenantActiveAsync(tenantId))
{
    throw new TenantNotFoundException(tenantId);
}
```

### 3. Testing
```csharp
[Test]
public async Task CreateProduct_ShouldSetTenantId()
{
    // Arrange
    var tenantId = "test_tenant";
    _tenantContext.SetCurrentTenant(tenantId);
    
    // Act
    var product = await _productService.CreateAsync(new Product { Name = "Test" });
    
    // Assert
    Assert.AreEqual(tenantId, product.TenantId);
}
```

## Troubleshooting

### 1. Common Issues

#### TenantId Not Set
```csharp
// Middleware'in doğru sırada eklendiğinden emin ol
app.UseTenantMiddleware(); // Authentication'dan sonra
app.UseAuthorization();
```

#### Cross-Tenant Data Access
```csharp
// Global query filter'ın çalıştığından emin ol
// DbContext'te tenant resolver inject edildiğinden emin ol
```

#### Performance Issues
```csharp
// TenantId index'lerinin oluşturulduğundan emin ol
// Query plan'ları kontrol et
```

### 2. Debug Commands
```sql
-- Current tenant'ı kontrol et
SELECT current_setting('app.current_tenant_id');

-- Tenant bazlı veri sayısını kontrol et
SELECT TenantId, COUNT(*) FROM Products GROUP BY TenantId;
```

## Sonuç

Bu MultiTenant altyapısı ile:

✅ **Veri İzolasyonu**: Her tenant'ın verisi tamamen izole edilir
✅ **Otomatik Filtering**: Tüm sorgularda TenantId otomatik eklenir
✅ **Esnek Resolution**: Header, subdomain, claim gibi farklı kaynaklardan tenant çözümleme
✅ **Performance**: Optimized queries ve index stratejileri
✅ **Güvenlik**: Row-level security ve application-level filtering
✅ **Maintainability**: Clean architecture ve dependency injection
✅ **Scalability**: Tenant bazlı connection pooling ve caching

Bu altyapı, eTrade Core projesinin çok kiracılı bir SaaS uygulaması olarak geliştirilmesine olanak sağlar. 