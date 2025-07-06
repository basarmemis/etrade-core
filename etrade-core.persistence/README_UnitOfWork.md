# Unit of Work Pattern Implementation

Bu projede Unit of Work pattern'i implement edilmiştir. Bu pattern, transaction yönetimi ve repository'lerin koordinasyonu için kullanılır.

## Özellikler

- **Transaction Yönetimi**: Otomatik ve manuel transaction yönetimi
- **Repository Koordinasyonu**: Tüm repository'lerin tek bir noktadan yönetimi
- **Dispose Pattern**: Kaynakların düzgün şekilde temizlenmesi
- **Error Handling**: Hata durumlarında otomatik rollback

## Kullanım Örnekleri

### 1. Basit Kullanım (Otomatik Transaction)

```csharp
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Order> CreateOrderAsync(Order order, List<OrderItem> orderItems)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // Siparişi kaydet
            await _unitOfWork.Orders.AddAsync(order);
            
            // Sipariş kalemlerini kaydet
            foreach (var item in orderItems)
            {
                item.OrderId = order.Id;
                await _unitOfWork.OrderItems.AddAsync(item);
            }

            return order;
        });
    }
}
```

### 2. Manuel Transaction Yönetimi

```csharp
public async Task<Order> CreateOrderWithManualTransactionAsync(Order order, List<OrderItem> orderItems)
{
    try
    {
        // Transaction başlat
        await _unitOfWork.BeginTransactionAsync();

        // İşlemleri gerçekleştir
        await _unitOfWork.Orders.AddAsync(order);
        
        foreach (var item in orderItems)
        {
            item.OrderId = order.Id;
            await _unitOfWork.OrderItems.AddAsync(item);
        }

        // Değişiklikleri kaydet
        await _unitOfWork.SaveChangesAsync();

        // Transaction'ı commit et
        await _unitOfWork.CommitTransactionAsync();

        return order;
    }
    catch
    {
        // Hata durumunda rollback
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

### 3. Sadece Okuma İşlemleri

```csharp
public async Task<IEnumerable<Order>> GetUserOrdersAsync(long userProfileId)
{
    // Okuma işlemleri için transaction gerekmez
    return await _unitOfWork.Orders.GetOrdersByUserProfileIdAsync(userProfileId);
}
```

## Dependency Injection

Program.cs veya Startup.cs dosyasında servisleri kaydetmek için:

```csharp
// Persistence katmanı servislerini kaydet
services.AddPersistenceServices(Configuration.GetConnectionString("DefaultConnection"));

// Application katmanı servislerini kaydet
services.AddScoped<OrderService>();
```

## Repository'ler

Unit of Work içerisinde şu repository'ler bulunur:

- `IUserProfileRepository` - Kullanıcı profilleri
- `IProductRepository` - Ürünler
- `IOrderRepository` - Siparişler
- `IOrderItemRepository` - Sipariş kalemleri

## Transaction Durumu Kontrolü

```csharp
// Aktif transaction var mı kontrol et
if (_unitOfWork.HasActiveTransaction)
{
    // Transaction zaten aktif
}

// Dispose edilmiş mi kontrol et
if (_unitOfWork.IsDisposed)
{
    // Unit of Work dispose edilmiş
}
```

## Best Practices

1. **Her HTTP request için bir Unit of Work instance'ı kullanın**
2. **Uzun süren işlemler için transaction timeout'larını ayarlayın**
3. **Hata durumlarında mutlaka rollback yapın**
4. **Dispose pattern'ini doğru kullanın**
5. **Repository'leri Unit of Work üzerinden erişin**

## Hata Yönetimi

Unit of Work pattern'i otomatik olarak hata durumlarında rollback yapar:

```csharp
try
{
    await _unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        // İşlemler...
        throw new Exception("Bir hata oluştu");
    });
}
catch (Exception ex)
{
    // Transaction otomatik olarak rollback edildi
    // Hata loglanabilir
}
```

## Performans İpuçları

1. **Lazy Loading**: Repository'ler lazy loading ile yüklenir
2. **Transaction Scope**: Gereksiz uzun transaction'lardan kaçının
3. **Batch Operations**: Toplu işlemler için AddRangeAsync kullanın
4. **Connection Pooling**: DbContext connection pooling'i kullanır 