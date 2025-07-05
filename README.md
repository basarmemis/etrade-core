# E-Trade Core Project

Bu proje, Unit of Work ve Generic Repository tasarım desenlerini kullanan bir .NET Core e-ticaret uygulamasıdır.

## Proje Yapısı

```
etrade-core/
├── etrade-core.api/          # Web API katmanı
├── etrade-core.domain/       # Domain/Entity katmanı
│   └── Entities/
│       ├── Base/             # Base entity sınıfları
│       └── Core/             # Domain entity'leri
└── etrade-core.persistence/  # Veri erişim katmanı
    └── Repositories/
        ├── Base/             # Base repository sınıfları
        └── Interfaces/       # Repository interface'leri
```

## Esnek Generic Repository Tasarımı

### Interface Tabanlı Yapı

Proje, farklı entity tipleri için esnek bir yapı sunar:

```csharp
// Temel interface'ler
public interface IEntity<TKey>
{
    TKey Id { get; set; }
}

public interface IAuditableEntity
{
    DateTime CreatedDate { get; set; }
    DateTime? UpdatedDate { get; set; }
}

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; set; }
}
```

### Base Entity Sınıfları

#### 1. Tek ID'li Entity'ler (Audit Bilgileri Var)
```csharp
// Generic BaseEntity - farklı ID tipleri için
public abstract class BaseEntity<TKey> : IEntity<TKey>, IAuditableEntity, ISoftDeletableEntity
{
    public TKey Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
}

// Default BaseEntity - int ID için
public abstract class BaseEntity : BaseEntity<int>
{
}
```

#### 2. Tek ID'li Entity'ler (Audit Bilgileri Yok)
```csharp
// BaseEntityWithoutAudit - audit bilgileri yok
public abstract class BaseEntityWithoutAudit<TKey> : IEntity<TKey>, ISoftDeletableEntity
{
    public TKey Id { get; set; }
    public bool IsDeleted { get; set; }
}

// Default BaseEntityWithoutAudit - int ID için
public abstract class BaseEntityWithoutAudit : BaseEntityWithoutAudit<int>
{
}
```

#### 3. Composite Key'li Entity'ler (Audit Bilgileri Var)
```csharp
// BaseEntityWithCompositeKey - audit bilgileri var
public abstract class BaseEntityWithCompositeKey : IAuditableEntity, ISoftDeletableEntity
{
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
}
```

#### 4. Composite Key'li Entity'ler (Audit Bilgileri Yok)
```csharp
// BaseEntityWithCompositeKeyWithoutAudit - audit bilgileri yok
public abstract class BaseEntityWithCompositeKeyWithoutAudit : ISoftDeletableEntity
{
    public bool IsDeleted { get; set; }
}
```

### Repository Interface'leri

#### Tek ID'li Entity'ler İçin
- **IReadRepository<TEntity, TKey>**: Sadece okuma işlemleri
- **IWriteRepository<TEntity, TKey>**: Sadece yazma işlemleri  
- **IRepository<TEntity, TKey>**: Hem okuma hem yazma işlemleri

#### Composite Key'li Entity'ler İçin
- **ICompositeKeyReadRepository<TEntity>**: Sadece okuma işlemleri
- **ICompositeKeyWriteRepository<TEntity>**: Sadece yazma işlemleri
- **ICompositeKeyRepository<TEntity>**: Hem okuma hem yazma işlemleri

### Base Repository Implementasyonları

#### Tek ID'li Entity'ler İçin
```csharp
// Read Repository
public abstract class BaseReadRepository<TEntity, TKey, TContext> : IReadRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>, ISoftDeletableEntity
    where TKey : IEquatable<TKey>
    where TContext : DbContext

// Write Repository
public abstract class BaseWriteRepository<TEntity, TKey, TContext> : IWriteRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>, ISoftDeletableEntity
    where TKey : IEquatable<TKey>
    where TContext : DbContext

// Combined Repository
public abstract class BaseRepository<TEntity, TKey, TContext> : IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>, ISoftDeletableEntity
    where TKey : IEquatable<TKey>
    where TContext : DbContext
{
    // Audit bilgileri otomatik olarak kontrol edilir
    // IAuditableEntity implement eden entity'ler için audit bilgileri güncellenir
}
```

#### Composite Key'li Entity'ler İçin
```csharp
// Read Repository
public abstract class BaseCompositeKeyReadRepository<TEntity, TContext> : ICompositeKeyReadRepository<TEntity>
    where TEntity : class, ISoftDeletableEntity
    where TContext : DbContext

// Write Repository
public abstract class BaseCompositeKeyWriteRepository<TEntity, TContext> : ICompositeKeyWriteRepository<TEntity>
    where TEntity : class, ISoftDeletableEntity
    where TContext : DbContext

// Combined Repository
public abstract class BaseCompositeKeyRepository<TEntity, TContext> : ICompositeKeyRepository<TEntity>
    where TEntity : class, ISoftDeletableEntity
    where TContext : DbContext
{
    // GetByCompositeKeyAsync metodu derived class'larda implement edilmelidir
    // Audit bilgileri otomatik olarak kontrol edilir
}
```

## Kullanım Örnekleri

### 1. Audit Bilgileri Olan Entity (Tek ID)

```csharp
public class Product : BaseEntity<int>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; }
    public bool IsActive { get; set; }
}

public class ProductRepository : BaseRepository<Product, int, DbContext>, IProductRepository
{
    public ProductRepository(DbContext context) : base(context) { }
    
    // Özel metodlar
    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await GetAsync(p => p.IsActive);
    }
}
```

### 2. Audit Bilgileri Olmayan Entity (Tek ID)

```csharp
public class UserProfile : BaseEntityWithoutAudit<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    
    // Foreign key for one-to-one relationship
    public int UserId { get; set; }
    public User User { get; set; }
}

public class UserProfileRepository : BaseRepository<UserProfile, int, DbContext>, IUserProfileRepository
{
    public UserProfileRepository(DbContext context) : base(context) { }
    
    // Özel metodlar
    public async Task<UserProfile?> GetByUserIdAsync(int userId)
    {
        return await GetFirstAsync(p => p.UserId == userId);
    }
}
```

### 3. Composite Key'li Entity (Audit Bilgileri Var)

```csharp
public class OrderItem : BaseEntityWithCompositeKey
{
    [Key]
    public int OrderId { get; set; }
    
    [Key]
    public int ProductId { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    
    // Navigation properties
    public Order Order { get; set; }
    public Product Product { get; set; }
}

public class OrderItemRepository : BaseCompositeKeyRepository<OrderItem, DbContext>, IOrderItemRepository
{
    public OrderItemRepository(DbContext context) : base(context) { }
    
    // Composite key lookup implementasyonu
    public override async Task<OrderItem?> GetByCompositeKeyAsync(object[] keys, bool includeDeleted = false)
    {
        if (keys.Length != 2 || keys[0] is not int orderId || keys[1] is not int productId)
        {
            throw new ArgumentException("Composite key must contain OrderId and ProductId as integers");
        }

        return await GetQueryable(includeDeleted)
            .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ProductId == productId);
    }
    
    // Özel metodlar
    public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId)
    {
        return await GetAsync(oi => oi.OrderId == orderId);
    }
    
    public async Task<decimal> GetTotalAmountByOrderIdAsync(int orderId)
    {
        var orderItems = await GetByOrderIdAsync(orderId);
        return orderItems.Sum(oi => oi.TotalPrice);
    }
}
```

### Interface Tanımlama

```csharp
// Tek ID'li entity'ler için
public interface IProductRepository : IRepository<Product, int>
{
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<Product?> GetBySKUAsync(string sku);
}

public interface IUserProfileRepository : IRepository<UserProfile, int>
{
    Task<UserProfile?> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserProfile>> GetByPhoneNumberAsync(string phoneNumber);
}

// Composite key'li entity'ler için
public interface IOrderItemRepository : ICompositeKeyRepository<OrderItem>
{
    Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
    Task<IEnumerable<OrderItem>> GetByProductIdAsync(int productId);
    Task<decimal> GetTotalAmountByOrderIdAsync(int orderId);
}
```

## Özellikler

### ✅ Tam Entity Desteği
- **BaseEntity**: Audit bilgileri + Soft delete + Tek ID
- **BaseEntityWithoutAudit**: Sadece Soft delete + Tek ID (One-to-One ilişkiler için)
- **BaseEntityWithCompositeKey**: Audit bilgileri + Soft delete + Composite key
- **BaseEntityWithCompositeKeyWithoutAudit**: Sadece Soft delete + Composite key

### ✅ Otomatik Audit Yönetimi
- `IAuditableEntity` implement eden entity'ler için otomatik audit
- Audit bilgileri olmayan entity'ler için audit yok
- UTC timestamp kullanımı

### ✅ Generic ID Tipleri
- `int`, `long`, `Guid`, `string` vb. ID tipleri desteklenir
- Type-safe constraint'ler

### ✅ Composite Key Desteği
- Birden fazla sütunun birlikte primary key oluşturması
- `GetByCompositeKeyAsync(object[] keys)` metodu
- Derived class'larda implement edilmesi gereken özel key handling

### ✅ Soft Delete
- Varsayılan olarak soft delete aktif
- `IsDeleted` flag'i ile mantıksal silme
- `includeDeleted` parametresi ile silinmiş kayıtları da getirebilme

### ✅ Esnek Repository Yapısı
- Read/Write operasyonları ayrı interface'ler ve base sınıflar
- Tek ID ve Composite key için ayrı repository'ler
- Özel metodlar için extension noktaları
- Unit of Work pattern'e hazır yapı

### ✅ Performance Optimizasyonları
- Async/await pattern
- IQueryable kullanımı
- Lazy loading desteği

## Gelecek Geliştirmeler

- [ ] Unit of Work pattern implementasyonu
- [ ] Specification pattern
- [ ] Caching layer
- [ ] Pagination desteği
- [ ] Bulk operations
- [ ] Audit logging

## Kurulum

```bash
# Projeyi clone et
git clone <repository-url>
cd etrade-core

# Bağımlılıkları restore et
dotnet restore

# Projeyi build et
dotnet build

# API'yi çalıştır
cd etrade-core.api
dotnet run
```

## Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit yapın (`git commit -m 'Add amazing feature'`)
4. Push yapın (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun 