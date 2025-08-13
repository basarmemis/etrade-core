# Generic Product System - Kullanım Kılavuzu

## Genel Bakış
Bu sistem, farklı ürün tiplerini (villa kiralama, gözlük satışı, tekne kiralama vb.) tek bir yapı altında yönetmeyi sağlar.

## ProductAttribute Sistemi

### Temel Yapı
```csharp
public class ProductAttribute
{
    public string AttributeKey { get; set; }    // Özellik adı
    public string AttributeValue { get; set; }  // Özellik değeri
    public string AttributeType { get; set; }   // Veri tipi
    public string? Unit { get; set; }           // Birim
    public bool IsRequired { get; set; }        // Zorunlu mu?
    public int DisplayOrder { get; set; }       // Görüntüleme sırası
}
```

## Ürün Tipi Örnekleri

### 1. Villa Kiralama
```csharp
var villa = new Product
{
    Name = "Lüks Villa",
    Description = "Deniz manzaralı villa",
    Price = 0, // Kiralama ürünü olduğu için 0
    OfferingType = ProductOfferingTypes.Rental,
    Brand = "Premium Villas",
    Condition = "New"
};

// Villa özellikleri
var villaAttributes = new List<ProductAttribute>
{
    new() { AttributeKey = "DailyRentalPrice", AttributeValue = "500", AttributeType = "Decimal", Unit = "TL", IsRequired = true, DisplayOrder = 1 },
    new() { AttributeKey = "WeeklyRentalPrice", AttributeValue = "3000", AttributeType = "Decimal", Unit = "TL", IsRequired = false, DisplayOrder = 2 },
    new() { AttributeKey = "MonthlyRentalPrice", AttributeValue = "12000", AttributeType = "Decimal", Unit = "TL", IsRequired = false, DisplayOrder = 3 },
    new() { AttributeKey = "DepositRequired", AttributeValue = "1000", AttributeType = "Decimal", Unit = "TL", IsRequired = true, DisplayOrder = 4 },
    new() { AttributeKey = "MinimumRentalDays", AttributeValue = "2", AttributeType = "Integer", Unit = "days", IsRequired = true, DisplayOrder = 5 },
    new() { AttributeKey = "MaximumRentalDays", AttributeValue = "30", AttributeType = "Integer", Unit = "days", IsRequired = true, DisplayOrder = 6 },
    new() { AttributeKey = "MaxGuests", AttributeValue = "6", AttributeType = "Integer", Unit = "persons", IsRequired = true, DisplayOrder = 7 },
    new() { AttributeKey = "Bedrooms", AttributeValue = "3", AttributeType = "Integer", Unit = "rooms", IsRequired = true, DisplayOrder = 8 },
    new() { AttributeKey = "Bathrooms", AttributeValue = "2", AttributeType = "Integer", Unit = "rooms", IsRequired = true, DisplayOrder = 9 },
    new() { AttributeKey = "HasPool", AttributeValue = "true", AttributeType = "Boolean", IsRequired = false, DisplayOrder = 10 },
    new() { AttributeKey = "HasWifi", AttributeValue = "true", AttributeType = "Boolean", IsRequired = false, DisplayOrder = 11 },
    new() { AttributeKey = "Location", AttributeValue = "Kalkan", AttributeType = "Text", IsRequired = true, DisplayOrder = 12 }
};
```

### 2. Gözlük Satışı
```csharp
var glasses = new Product
{
    Name = "Ray-Ban Aviator",
    Description = "Klasik aviator gözlük",
    Price = 1500, // Satış fiyatı
    OfferingType = ProductOfferingTypes.Purchsase,
    Brand = "Ray-Ban",
    Model = "RB3025",
    Condition = "New"
};

// Gözlük özellikleri
var glassesAttributes = new List<ProductAttribute>
{
    new() { AttributeKey = "FrameSize", AttributeValue = "58", AttributeType = "Integer", Unit = "mm", IsRequired = true, DisplayOrder = 1 },
    new() { AttributeKey = "LensWidth", AttributeValue = "62", AttributeType = "Integer", Unit = "mm", IsRequired = true, DisplayOrder = 2 },
    new() { AttributeKey = "BridgeWidth", AttributeValue = "14", AttributeType = "Integer", Unit = "mm", IsRequired = true, DisplayOrder = 3 },
    new() { AttributeKey = "TempleLength", AttributeValue = "135", AttributeType = "Integer", Unit = "mm", IsRequired = true, DisplayOrder = 4 },
    new() { AttributeKey = "FrameColor", AttributeValue = "Gold", AttributeType = "Text", IsRequired = true, DisplayOrder = 5 },
    new() { AttributeKey = "LensColor", AttributeValue = "Green", AttributeType = "Text", IsRequired = true, DisplayOrder = 6 },
    new() { AttributeKey = "LensMaterial", AttributeValue = "Glass", AttributeType = "Text", IsRequired = true, DisplayOrder = 7 },
    new() { AttributeKey = "FrameMaterial", AttributeValue = "Metal", AttributeType = "Text", IsRequired = true, DisplayOrder = 8 },
    new() { AttributeKey = "UVProtection", AttributeValue = "100", AttributeType = "Integer", Unit = "%", IsRequired = true, DisplayOrder = 9 },
    new() { AttributeKey = "Polarized", AttributeValue = "true", AttributeType = "Boolean", IsRequired = false, DisplayOrder = 10 }
};
```

### 3. Tekne Kiralama
```csharp
var boat = new Product
{
    Name = "Lüks Yat",
    Description = "12 metre lüks yat",
    Price = 0,
    OfferingType = ProductOfferingTypes.Rental,
    Brand = "Azimut",
    Model = "Azimut 40",
    Year = 2020,
    Condition = "Used"
};

// Tekne özellikleri
var boatAttributes = new List<ProductAttribute>
{
    new() { AttributeKey = "DailyRentalPrice", AttributeValue = "800", AttributeType = "Decimal", Unit = "TL", IsRequired = true, DisplayOrder = 1 },
    new() { AttributeKey = "DepositRequired", AttributeValue = "2000", AttributeType = "Decimal", Unit = "TL", IsRequired = true, DisplayOrder = 2 },
    new() { AttributeKey = "MinimumRentalDays", AttributeValue = "1", AttributeType = "Integer", Unit = "days", IsRequired = true, DisplayOrder = 3 },
    new() { AttributeKey = "MaximumRentalDays", AttributeValue = "14", AttributeType = "Integer", Unit = "days", IsRequired = true, DisplayOrder = 4 },
    new() { AttributeKey = "Length", AttributeValue = "12", AttributeType = "Decimal", Unit = "meters", IsRequired = true, DisplayOrder = 5 },
    new() { AttributeKey = "MaxPassengers", AttributeValue = "8", AttributeType = "Integer", Unit = "persons", IsRequired = true, DisplayOrder = 6 },
    new() { AttributeKey = "EnginePower", AttributeValue = "300", AttributeType = "Integer", Unit = "hp", IsRequired = true, DisplayOrder = 7 },
    new() { AttributeKey = "FuelCapacity", AttributeValue = "400", AttributeType = "Integer", Unit = "liters", IsRequired = true, DisplayOrder = 8 },
    new() { AttributeKey = "HasCabin", AttributeValue = "true", AttributeType = "Boolean", IsRequired = false, DisplayOrder = 9 },
    new() { AttributeKey = "HasBathroom", AttributeValue = "true", AttributeType = "Boolean", IsRequired = false, DisplayOrder = 10 },
    new() { AttributeKey = "Location", AttributeValue = "Bodrum Marina", AttributeType = "Text", IsRequired = true, DisplayOrder = 11 }
};
```

## Order ve OrderItem OfferingType Sistemi

### Order OfferingTypes (Flags Enum)
```csharp
// Karışık sipariş - hem kiralama hem satış
var mixedOrder = new Order
{
    OrderNumber = "ORD-001",
    OrderDate = DateTime.Now,
    TotalAmount = 2500,
    Status = OrderStatus.Pending,
    CustomerEmail = "customer@example.com",
    OfferingTypes = ProductOfferingTypes.Rental | ProductOfferingTypes.Purchsase // Birden fazla tür
};

// Sadece kiralama siparişi
var rentalOrder = new Order
{
    OrderNumber = "ORD-002",
    OrderDate = DateTime.Now,
    TotalAmount = 1500,
    Status = OrderStatus.Pending,
    CustomerEmail = "customer@example.com",
    OfferingTypes = ProductOfferingTypes.Rental // Sadece kiralama
};

// Sadece satış siparişi
var purchaseOrder = new Order
{
    OrderNumber = "ORD-003",
    OrderDate = DateTime.Now,
    TotalAmount = 800,
    Status = OrderStatus.Pending,
    CustomerEmail = "customer@example.com",
    OfferingTypes = ProductOfferingTypes.Purchsase // Sadece satış
};
```

### OrderItem OfferingType
```csharp
// Villa kiralama siparişi
var villaOrderItem = new OrderItem
{
    ProductId = villaId,
    Quantity = 1,
    UnitPrice = 500, // Günlük fiyat
    OfferingType = ProductOfferingTypes.Rental, // Bu öğe kiralama
    RentalStartDate = DateTime.Now.AddDays(1),
    RentalEndDate = DateTime.Now.AddDays(7),
    RentalDays = 6,
    Attributes = new List<OrderItemAttribute>
    {
        new() { AttributeKey = "DepositAmount", AttributeValue = "1000", AttributeType = "Decimal", Unit = "TL", IsRequired = true, DisplayOrder = 1 },
        new() { AttributeKey = "RentalStatus", AttributeValue = "Pending", AttributeType = "Enum", IsRequired = true, DisplayOrder = 2 },
        new() { AttributeKey = "SpecialRequests", AttributeValue = "Erken check-in", AttributeType = "Text", IsRequired = false, DisplayOrder = 3 },
        new() { AttributeKey = "InsuranceAmount", AttributeValue = "500", AttributeType = "Decimal", Unit = "TL", IsRequired = false, DisplayOrder = 4 }
    }
};

// Gözlük satın alma siparişi
var glassesOrderItem = new OrderItem
{
    ProductId = glassesId,
    Quantity = 1,
    UnitPrice = 1500,
    OfferingType = ProductOfferingTypes.Purchsase, // Bu öğe satış
    Attributes = new List<OrderItemAttribute>
    {
        new() { AttributeKey = "PrescriptionDetails", AttributeValue = "-2.5", AttributeType = "Text", IsRequired = true, DisplayOrder = 1 },
        new() { AttributeKey = "LensCoating", AttributeValue = "Anti-reflective", AttributeType = "Text", IsRequired = false, DisplayOrder = 2 },
        new() { AttributeKey = "DeliveryInstructions", AttributeValue = "Ofis adresine teslimat", AttributeType = "Text", IsRequired = false, DisplayOrder = 3 }
    }
};
```

## OrderItem Attribute Sistemi

### OrderItem için Dinamik Özellikler
```csharp
// Villa kiralama siparişi
var villaOrderItem = new OrderItem
{
    ProductId = villaId,
    Quantity = 1,
    UnitPrice = 500, // Günlük fiyat
    RentalStartDate = DateTime.Now.AddDays(1),
    RentalEndDate = DateTime.Now.AddDays(7),
    RentalDays = 6,
    Attributes = new List<OrderItemAttribute>
    {
        new() { AttributeKey = "DepositAmount", AttributeValue = "1000", AttributeType = "Decimal", Unit = "TL", IsRequired = true, DisplayOrder = 1 },
        new() { AttributeKey = "RentalStatus", AttributeValue = "Pending", AttributeType = "Enum", IsRequired = true, DisplayOrder = 2 },
        new() { AttributeKey = "SpecialRequests", AttributeValue = "Erken check-in", AttributeType = "Text", IsRequired = false, DisplayOrder = 3 },
        new() { AttributeKey = "InsuranceAmount", AttributeValue = "500", AttributeType = "Decimal", Unit = "TL", IsRequired = false, DisplayOrder = 4 }
    }
};

// Gözlük satın alma siparişi
var glassesOrderItem = new OrderItem
{
    ProductId = glassesId,
    Quantity = 1,
    UnitPrice = 1500,
    Attributes = new List<OrderItemAttribute>
    {
        new() { AttributeKey = "PrescriptionDetails", AttributeValue = "-2.5", AttributeType = "Text", IsRequired = true, DisplayOrder = 1 },
        new() { AttributeKey = "LensCoating", AttributeValue = "Anti-reflective", AttributeType = "Text", IsRequired = false, DisplayOrder = 2 },
        new() { AttributeKey = "DeliveryInstructions", AttributeValue = "Ofis adresine teslimat", AttributeType = "Text", IsRequired = false, DisplayOrder = 3 }
    }
};
```

## Attribute Tipleri

### Desteklenen Veri Tipleri:
- **Text**: Metin değerleri
- **Integer**: Tam sayılar
- **Decimal**: Ondalıklı sayılar
- **Boolean**: True/False değerleri
- **Date**: Tarih değerleri
- **Enum**: Önceden tanımlı değerler

### Standart Attribute Key'leri:

#### Kiralama İçin:
- `DailyRentalPrice` - Günlük kiralama fiyatı
- `WeeklyRentalPrice` - Haftalık kiralama fiyatı
- `MonthlyRentalPrice` - Aylık kiralama fiyatı
- `DepositRequired` - Gerekli depozito
- `MinimumRentalDays` - Minimum kiralama günü
- `MaximumRentalDays` - Maksimum kiralama günü

#### OrderItem için:
- `DepositAmount` - Ödenen depozito
- `RentalStatus` - Kiralama durumu
- `SpecialRequests` - Özel istekler
- `InsuranceAmount` - Sigorta tutarı
- `PrescriptionDetails` - Reçete detayları (gözlük)
- `DeliveryInstructions` - Teslimat talimatları

#### Genel Özellikler:
- `Brand` - Marka
- `Model` - Model
- `Year` - Yıl
- `Condition` - Durum
- `Color` - Renk
- `Size` - Boyut
- `Weight` - Ağırlık
- `Material` - Malzeme

## Kullanım Örnekleri

### Attribute Değerini Alma:
```csharp
// Villa'nın günlük kiralama fiyatını alma
var dailyPrice = villa.Attributes
    .FirstOrDefault(a => a.AttributeKey == "DailyRentalPrice")
    ?.AttributeValue;

// Gözlüğün frame boyutunu alma
var frameSize = glasses.Attributes
    .FirstOrDefault(a => a.AttributeKey == "FrameSize")
    ?.AttributeValue;

// Sipariş öğesinin depozito tutarını alma
var depositAmount = villaOrderItem.Attributes
    .FirstOrDefault(a => a.AttributeKey == "DepositAmount")
    ?.AttributeValue;
```

### Filtreleme:
```csharp
// Kiralanabilir ürünleri bulma
var rentableProducts = products.Where(p => 
    p.OfferingType.HasFlag(ProductOfferingTypes.Rental) &&
    p.Attributes.Any(a => a.AttributeKey == "DailyRentalPrice"));

// Belirli bir fiyat aralığındaki villaları bulma
var affordableVillas = products.Where(p => 
    p.OfferingType.HasFlag(ProductOfferingTypes.Rental) &&
    p.Attributes.Any(a => 
        a.AttributeKey == "DailyRentalPrice" && 
        decimal.Parse(a.AttributeValue) <= 500));

// Aktif kiralama siparişlerini bulma
var activeRentals = orderItems.Where(oi => 
    oi.RentalStartDate.HasValue &&
    oi.Attributes.Any(a => 
        a.AttributeKey == "RentalStatus" && 
        a.AttributeValue == "Active"));

// Sadece kiralama siparişlerini bulma
var rentalOrders = orders.Where(o => 
    o.OfferingTypes.HasFlag(ProductOfferingTypes.Rental));

// Karışık siparişleri bulma (hem kiralama hem satış)
var mixedOrders = orders.Where(o => 
    o.OfferingTypes.HasFlag(ProductOfferingTypes.Rental) && 
    o.OfferingTypes.HasFlag(ProductOfferingTypes.Purchsase));

// Kiralama öğelerini bulma
var rentalItems = orderItems.Where(oi => 
    oi.OfferingType == ProductOfferingTypes.Rental);
```

## Avantajlar

1. **Esneklik**: Yeni ürün tipleri kolayca eklenebilir
2. **Genişletilebilirlik**: Yeni özellikler mevcut yapıyı bozmadan eklenebilir
3. **Performans**: Sadece gerekli özellikler saklanır
4. **Bakım**: Tek bir yapı tüm ürün tiplerini yönetir
5. **Arama**: Attribute'lar üzerinden gelişmiş arama yapılabilir
6. **Sipariş Esnekliği**: Her sipariş öğesi için özel bilgiler saklanabilir
7. **İşlem Türü Takibi**: Order ve OrderItem seviyesinde işlem türü takibi
8. **Raporlama**: İşlem türlerine göre raporlama yapılabilir 