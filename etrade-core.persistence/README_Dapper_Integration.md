# Dapper Entegrasyonu

Bu dokümanda, projeye eklenen Dapper entegrasyonu ve kullanım şekilleri açıklanmaktadır.

## Genel Bakış

Dapper entegrasyonu, performans odaklı okuma işlemleri için EF Core ile birlikte kullanılmak üzere tasarlanmıştır. Bu yaklaşım:

- **EF Core**: Yazma işlemleri ve karmaşık entity işlemleri için
- **Dapper**: Performans odaklı okuma işlemleri, projection-only sorgular ve sayfalama için

## Mimari Yapı

### 1. Connection Factory
```csharp
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    string GetConnectionString();
}
```

### 2. Repository Interface'leri
- `IDapperReadRepository<TEntity, TKey>`: Sadece okuma işlemleri
- `IDapperWriteRepository<TEntity, TKey>`: Sadece yazma işlemleri  
- `IDapperRepository<TEntity, TKey>`: Hem okuma hem yazma işlemleri

### 3. Base Repository'ler
- `BaseDapperReadRepository<TEntity, TKey>`: Okuma işlemleri için base class
- `BaseDapperWriteRepository<TEntity, TKey>`: Yazma işlemleri için base class
- `BaseDapperRepository<TEntity, TKey>`: Composition pattern ile birleştirilmiş

### 4. Unit of Work
- `IDapperUnitOfWork`: Dapper için unit of work interface'i
- `DapperUnitOfWork`: Implementasyon

## Kullanım Senaryoları

### 1. Performans Odaklı Okuma
```csharp
// Product listesi için projection-only sorgu
var products = await dapperUow.Products.QueryAsync<ProductListDto>(sql);
```

### 2. Sayfalama
```csharp
var (items, totalCount) = await dapperUow.Products
    .QueryWithPaginationAsync<ProductListDto>(sql, pageNumber, pageSize);
```

### 3. Complex Join Sorguları
```csharp
var orderDetails = await dapperUow.Orders
    .QueryAsync<OrderDetailDto>(complexJoinSql, new { OrderId = orderId });
```

### 4. Stored Procedure
```csharp
var stats = await dapperUow.Products
    .QueryStoredProcedureAsync<ProductStatsDto>("GetProductStatistics");
```

### 5. Multiple Result Set
```csharp
var results = await dapperUow.Products.QueryMultipleAsync(sql);
var products = results.ElementAt(0).Cast<Product>();
var categories = results.ElementAt(1).Cast<Category>();
```

### 6. Transaction Yönetimi
```csharp
await dapperUow.ExecuteInTransactionAsync(async transaction =>
{
    // Transaction içinde işlemler
    var orderId = await dapperUow.Orders.QueryScalarAsync<long>(orderSql, order, transaction);
    // ... diğer işlemler
});
```

## EF Core ile Birlikte Kullanım

### Servis Kayıtları
```csharp
// EF Core servisleri
services.AddScoped<IUnitOfWork, UnitOfWork>();

// Dapper servisleri
services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(connectionString));
services.AddScoped<IDapperUnitOfWork, DapperUnitOfWork>();
```

### Kullanım Stratejisi
```csharp
public class ProductService
{
    private readonly IUnitOfWork _efUow;        // Yazma işlemleri için
    private readonly IDapperUnitOfWork _dapperUow; // Okuma işlemleri için

    public ProductService(IUnitOfWork efUow, IDapperUnitOfWork dapperUow)
    {
        _efUow = efUow;
        _dapperUow = dapperUow;
    }

    // Yazma işlemi - EF Core
    public async Task<Product> CreateProductAsync(Product product)
    {
        var result = await _efUow.Products.AddAsync(product);
        await _efUow.SaveChangesAsync();
        return result;
    }

    // Okuma işlemi - Dapper (performans için)
    public async Task<IEnumerable<ProductListDto>> GetProductListAsync()
    {
        const string sql = "SELECT Id, Name, Price FROM Products WHERE IsDeleted = false";
        return await _dapperUow.Products.QueryAsync<ProductListDto>(sql);
    }
}
```

## Avantajlar

### Dapper Avantajları
- **Performans**: Micro-ORM olarak çok hızlı
- **Esneklik**: Raw SQL ile tam kontrol
- **Hafiflik**: Minimal overhead
- **Sayfalama**: Built-in sayfalama desteği
- **Bulk Operations**: Toplu işlemler için optimize

### EF Core Avantajları
- **Change Tracking**: Otomatik değişiklik takibi
- **Lazy Loading**: İhtiyaç duyulduğunda yükleme
- **LINQ**: Type-safe sorgular
- **Migrations**: Database schema yönetimi
- **Transaction Management**: Gelişmiş transaction yönetimi

## Best Practices

### 1. Sorgu Seçimi
- **EF Core**: Entity CRUD, karmaşık business logic
- **Dapper**: Report queries, projection-only, complex joins

### 2. Performance Optimization
- Dapper'da sadece gerekli kolonları seç
- Index'leri optimize et
- Sayfalama kullan

### 3. Transaction Management
- EF Core: Entity operations
- Dapper: Read operations (transaction gerekmez)
- Hybrid: Her ikisini de kullan

### 4. Error Handling
```csharp
try
{
    var result = await dapperUow.ExecuteInTransactionAsync(async transaction =>
    {
        // Transaction işlemleri
    });
}
catch (Exception ex)
{
    // Error handling
    logger.LogError(ex, "Transaction failed");
    throw;
}
```

## Örnek SQL Sorguları

### Product List with Category
```sql
SELECT 
    p.Id,
    p.Name,
    p.Description,
    p.Price,
    c.Name as CategoryName,
    COUNT(pi.Id) as ImageCount
FROM Products p
LEFT JOIN Categories c ON p.CategoryId = c.Id
LEFT JOIN ProductImages pi ON p.Id = pi.ProductId
WHERE p.IsDeleted = false
GROUP BY p.Id, p.Name, p.Description, p.Price, c.Name
ORDER BY p.CreatedDate DESC
```

### Order Details with User and Products
```sql
SELECT 
    o.Id,
    o.OrderNumber,
    o.Status,
    o.TotalAmount,
    up.FirstName,
    up.LastName,
    p.Name as ProductName,
    oi.Quantity,
    oi.UnitPrice
FROM Orders o
INNER JOIN UserProfiles up ON o.UserProfileId = up.Id
INNER JOIN OrderItems oi ON o.Id = oi.OrderId
INNER JOIN Products p ON oi.ProductId = p.Id
WHERE o.Id = @OrderId AND o.IsDeleted = false
```

## Sonuç

Dapper entegrasyonu ile:
- Performans kritik okuma işlemleri optimize edildi
- EF Core'un güçlü yanları korundu
- Hybrid yaklaşım ile en iyi performans elde edildi
- Esnek ve ölçeklenebilir mimari oluşturuldu

Bu yapı sayesinde, her iki ORM'in de güçlü yanlarından faydalanarak optimal performans elde edilebilir. 