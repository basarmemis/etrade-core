# Generic Product Architecture

Bu yapı, aynı Product entity'sini farklı sektörlerde kullanmanızı sağlar.

## Kullanım Örnekleri

### 1. Villa Kiralama Sitesi

```csharp
// Villa Template
var villaTemplate = new ProductTemplate
{
    Name = "Villa Template",
    CategoryId = villaCategoryId,
    TemplateAttributes = new List<ProductTemplateAttribute>
    {
        new() { AttributeKey = "MaxGuests", AttributeName = "Maximum Guests", AttributeType = "Number", Unit = "persons", IsRequired = true, IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "Bedrooms", AttributeName = "Number of Bedrooms", AttributeType = "Number", IsRequired = true, IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "Bathrooms", AttributeName = "Number of Bathrooms", AttributeType = "Number", IsRequired = true, IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "Pool", AttributeName = "Has Pool", AttributeType = "Boolean", IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "Location", AttributeName = "Location", AttributeType = "Text", IsRequired = true, IsSearchable = true },
        new() { AttributeKey = "CheckInTime", AttributeName = "Check-in Time", AttributeType = "Text", DefaultValue = "14:00" },
        new() { AttributeKey = "CheckOutTime", AttributeName = "Check-out Time", AttributeType = "Text", DefaultValue = "11:00" }
    }
};

// Villa Ürünü
var villa = new Product
{
    Name = "Lüks Villa - Antalya",
    Description = "Deniz manzaralı lüks villa",
    Price = 2500.00m,
    StockQuantity = 1,
    SKU = "VIL-001",
    OfferingType = ProductOfferingTypes.Rental,
    Brand = "Premium Villas",
    Model = "Luxury Villa",
    Year = 2020,
    Condition = "New",
    CategoryId = villaCategoryId,
    ProductTemplateId = villaTemplate.Id,
    Attributes = new List<ProductAttribute>
    {
        new() { AttributeKey = "MaxGuests", AttributeValue = "8", AttributeType = "Number", Unit = "persons" },
        new() { AttributeKey = "Bedrooms", AttributeValue = "4", AttributeType = "Number" },
        new() { AttributeKey = "Bathrooms", AttributeValue = "3", AttributeType = "Number" },
        new() { AttributeKey = "Pool", AttributeValue = "true", AttributeType = "Boolean" },
        new() { AttributeKey = "Location", AttributeValue = "Antalya, Kemer", AttributeType = "Text" },
        new() { AttributeKey = "CheckInTime", AttributeValue = "15:00", AttributeType = "Text" },
        new() { AttributeKey = "CheckOutTime", AttributeValue = "10:00", AttributeType = "Text" }
    }
};
```

### 2. Tekne Kiralama Sitesi

```csharp
// Tekne Template
var boatTemplate = new ProductTemplate
{
    Name = "Boat Template",
    CategoryId = boatCategoryId,
    TemplateAttributes = new List<ProductTemplateAttribute>
    {
        new() { AttributeKey = "MaxPassengers", AttributeName = "Maximum Passengers", AttributeType = "Number", Unit = "persons", IsRequired = true, IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "BoatLength", AttributeName = "Boat Length", AttributeType = "Number", Unit = "meters", IsRequired = true, IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "EnginePower", AttributeName = "Engine Power", AttributeType = "Number", Unit = "hp", IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "BoatType", AttributeName = "Boat Type", AttributeType = "Enum", AllowedValues = "[\"Yacht\",\"Speedboat\",\"Sailboat\",\"Catamaran\"]", IsRequired = true, IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "CaptainIncluded", AttributeName = "Captain Included", AttributeType = "Boolean", IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "Marina", AttributeName = "Marina", AttributeType = "Text", IsRequired = true, IsSearchable = true }
    }
};

// Tekne Ürünü
var boat = new Product
{
    Name = "Lüks Yacht - Bodrum",
    Description = "Denizde unutulmaz anlar için",
    Price = 5000.00m,
    StockQuantity = 1,
    SKU = "BOAT-001",
    OfferingType = ProductOfferingTypes.Rental,
    Brand = "Bodrum Yachts",
    Model = "Luxury Yacht",
    Year = 2019,
    Condition = "Excellent",
    CategoryId = boatCategoryId,
    ProductTemplateId = boatTemplate.Id,
    Attributes = new List<ProductAttribute>
    {
        new() { AttributeKey = "MaxPassengers", AttributeValue = "12", AttributeType = "Number", Unit = "persons" },
        new() { AttributeKey = "BoatLength", AttributeValue = "15", AttributeType = "Number", Unit = "meters" },
        new() { AttributeKey = "EnginePower", AttributeValue = "500", AttributeType = "Number", Unit = "hp" },
        new() { AttributeKey = "BoatType", AttributeValue = "Yacht", AttributeType = "Enum" },
        new() { AttributeKey = "CaptainIncluded", AttributeValue = "true", AttributeType = "Boolean" },
        new() { AttributeKey = "Marina", AttributeValue = "Bodrum Marina", AttributeType = "Text" }
    }
};
```

### 3. Gözlük Satış Sitesi

```csharp
// Gözlük Template
var glassesTemplate = new ProductTemplate
{
    Name = "Glasses Template",
    CategoryId = glassesCategoryId,
    TemplateAttributes = new List<ProductTemplateAttribute>
    {
        new() { AttributeKey = "FrameSize", AttributeName = "Frame Size", AttributeType = "Enum", AllowedValues = "[\"Small\",\"Medium\",\"Large\",\"Extra Large\"]", IsRequired = true, IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "FrameColor", AttributeName = "Frame Color", AttributeType = "Text", IsRequired = true, IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "LensColor", AttributeName = "Lens Color", AttributeType = "Text", IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "LensType", AttributeName = "Lens Type", AttributeType = "Enum", AllowedValues = "[\"Single Vision\",\"Bifocal\",\"Progressive\",\"Computer\"]", IsRequired = true, IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "FrameMaterial", AttributeName = "Frame Material", AttributeType = "Text", IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "UVProtection", AttributeName = "UV Protection", AttributeType = "Boolean", IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "Prescription", AttributeName = "Prescription Required", AttributeType = "Boolean", IsSearchable = true, IsFilterable = true }
    }
};

// Gözlük Ürünü
var glasses = new Product
{
    Name = "Ray-Ban Aviator",
    Description = "Klasik aviator tasarım",
    Price = 299.99m,
    StockQuantity = 50,
    SKU = "GLASS-001",
    OfferingType = ProductOfferingTypes.Purchsase,
    Brand = "Ray-Ban",
    Model = "Aviator",
    Year = 2023,
    Condition = "New",
    CategoryId = glassesCategoryId,
    ProductTemplateId = glassesTemplate.Id,
    Attributes = new List<ProductAttribute>
    {
        new() { AttributeKey = "FrameSize", AttributeValue = "Large", AttributeType = "Enum" },
        new() { AttributeKey = "FrameColor", AttributeValue = "Gold", AttributeType = "Text" },
        new() { AttributeKey = "LensColor", AttributeValue = "Green", AttributeType = "Text" },
        new() { AttributeKey = "LensType", AttributeValue = "Single Vision", AttributeType = "Enum" },
        new() { AttributeKey = "FrameMaterial", AttributeValue = "Metal", AttributeType = "Text" },
        new() { AttributeKey = "UVProtection", AttributeValue = "true", AttributeType = "Boolean" },
        new() { AttributeKey = "Prescription", AttributeValue = "false", AttributeType = "Boolean" }
    }
};
```

### 4. Oyuncak Satış Sitesi

```csharp
// Oyuncak Template
var toyTemplate = new ProductTemplate
{
    Name = "Toy Template",
    CategoryId = toyCategoryId,
    TemplateAttributes = new List<ProductTemplateAttribute>
    {
        new() { AttributeKey = "AgeRange", AttributeName = "Age Range", AttributeType = "Text", IsRequired = true, IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "ToyType", AttributeName = "Toy Type", AttributeType = "Enum", AllowedValues = "[\"Educational\",\"Action Figure\",\"Board Game\",\"Puzzle\",\"Building Blocks\",\"Doll\"]", IsRequired = true, IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "Material", AttributeName = "Material", AttributeType = "Text", IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "BatteryRequired", AttributeName = "Battery Required", AttributeType = "Boolean", IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "BatteryType", AttributeName = "Battery Type", AttributeType = "Text", IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "SafetyCertification", AttributeName = "Safety Certification", AttributeType = "Text", IsSearchable = true, IsFilterable = true },
        new() { AttributeKey = "Dimensions", AttributeName = "Dimensions", AttributeType = "Text", Unit = "cm", IsSearchable = true }
    }
};

// Oyuncak Ürünü
var toy = new Product
{
    Name = "LEGO Star Wars Millennium Falcon",
    Description = "Iconik Star Wars gemisi LEGO seti",
    Price = 159.99m,
    StockQuantity = 25,
    SKU = "TOY-001",
    OfferingType = ProductOfferingTypes.Purchsase,
    Brand = "LEGO",
    Model = "Millennium Falcon",
    Year = 2023,
    Condition = "New",
    CategoryId = toyCategoryId,
    ProductTemplateId = toyTemplate.Id,
    Attributes = new List<ProductAttribute>
    {
        new() { AttributeKey = "AgeRange", AttributeValue = "9+", AttributeType = "Text" },
        new() { AttributeKey = "ToyType", AttributeValue = "Building Blocks", AttributeType = "Enum" },
        new() { AttributeKey = "Material", AttributeValue = "Plastic", AttributeType = "Text" },
        new() { AttributeKey = "BatteryRequired", AttributeValue = "false", AttributeType = "Boolean" },
        new() { AttributeKey = "SafetyCertification", AttributeValue = "CE", AttributeType = "Text" },
        new() { AttributeKey = "Dimensions", AttributeValue = "84 x 56 x 13", AttributeType = "Text", Unit = "cm" }
    }
};
```

## Avantajları

1. **Tek Entity, Çoklu Kullanım**: Aynı Product entity'si farklı sektörlerde kullanılabilir
2. **Esnek Özellikler**: ProductAttribute ile dinamik özellikler eklenebilir
3. **Template Sistemi**: ProductTemplate ile standart özellikler tanımlanabilir
4. **Arama ve Filtreleme**: Attribute'lar arama ve filtreleme için işaretlenebilir
5. **Validasyon**: Template'de validation kuralları tanımlanabilir
6. **Kategori Hiyerarşisi**: Hiyerarşik kategori yapısı
7. **Fiyat Geçmişi**: ProductPriceHistory ile fiyat değişimleri takip edilebilir
8. **Medya Yönetimi**: ProductImage ile çoklu resim desteği

## Sorgulama Örnekleri

```csharp
// Villa arama - 4+ kişilik, havuzlu
var villas = products.Where(p => p.Category.Name == "Villa" &&
    p.Attributes.Any(a => a.AttributeKey == "MaxGuests" && int.Parse(a.AttributeValue) >= 4) &&
    p.Attributes.Any(a => a.AttributeKey == "Pool" && a.AttributeValue == "true"));

// Gözlük arama - Büyük beden, altın renk
var glasses = products.Where(p => p.Category.Name == "Glasses" &&
    p.Attributes.Any(a => a.AttributeKey == "FrameSize" && a.AttributeValue == "Large") &&
    p.Attributes.Any(a => a.AttributeKey == "FrameColor" && a.AttributeValue == "Gold"));
``` 