using etrade_core.domain.CategoryModule.Entities;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.domain.ProductModule.Enums;
using etrade_core.persistence.Context;

namespace etrade_core.persistence.data
{
    public static class SeedData
    {
        public static async Task SeedAsync(DomainDbContext context)
        {
            // Kategoriler oluştur
            var categories = await CreateCategoriesAsync(context);
            
            // Template'ler oluştur
            var templates = await CreateTemplatesAsync(context, categories);
            
            // Ürünler oluştur
            await CreateProductsAsync(context, categories, templates);
            
            await context.SaveChangesAsync();
        }

        private static async Task<Dictionary<string, Category>> CreateCategoriesAsync(DomainDbContext context)
        {
            var categories = new Dictionary<string, Category>();

            // Ana kategoriler
            var villaCategory = new Category
            {
                Name = "Villa Kiralama",
                Description = "Lüks villa kiralama hizmetleri",
                IsActive = true,
                DisplayOrder = 1
            };

            var boatCategory = new Category
            {
                Name = "Tekne Kiralama",
                Description = "Tekne ve yacht kiralama hizmetleri",
                IsActive = true,
                DisplayOrder = 2
            };

            var glassesCategory = new Category
            {
                Name = "Gözlük",
                Description = "Güneş gözlüğü ve numaralı gözlük",
                IsActive = true,
                DisplayOrder = 3
            };

            var toyCategory = new Category
            {
                Name = "Oyuncak",
                Description = "Çeşitli oyuncaklar",
                IsActive = true,
                DisplayOrder = 4
            };

            context.Categories.AddRange(villaCategory, boatCategory, glassesCategory, toyCategory);
            await context.SaveChangesAsync();

            categories["Villa"] = villaCategory;
            categories["Boat"] = boatCategory;
            categories["Glasses"] = glassesCategory;
            categories["Toy"] = toyCategory;

            return categories;
        }

        private static async Task<Dictionary<string, ProductTemplate>> CreateTemplatesAsync(
            DomainDbContext context, Dictionary<string, Category> categories)
        {
            var templates = new Dictionary<string, ProductTemplate>();

            // Villa Template
            var villaTemplate = new ProductTemplate
            {
                Name = "Villa Template",
                Description = "Villa kiralama için standart template",
                CategoryId = categories["Villa"].Id,
                IsActive = true,
                TemplateAttributes = new List<ProductTemplateAttribute>
                {
                    new() { AttributeKey = "MaxGuests", AttributeName = "Maximum Guests", AttributeType = "Number", Unit = "persons", IsRequired = true, IsSearchable = true, IsFilterable = true, DisplayOrder = 1 },
                    new() { AttributeKey = "Bedrooms", AttributeName = "Number of Bedrooms", AttributeType = "Number", IsRequired = true, IsSearchable = true, IsFilterable = true, DisplayOrder = 2 },
                    new() { AttributeKey = "Bathrooms", AttributeName = "Number of Bathrooms", AttributeType = "Number", IsRequired = true, IsSearchable = true, IsFilterable = true, DisplayOrder = 3 },
                    new() { AttributeKey = "Pool", AttributeName = "Has Pool", AttributeType = "Boolean", IsSearchable = true, IsFilterable = true, DisplayOrder = 4 },
                    new() { AttributeKey = "Location", AttributeName = "Location", AttributeType = "Text", IsRequired = true, IsSearchable = true, DisplayOrder = 5 }
                }
            };

            // Boat Template
            var boatTemplate = new ProductTemplate
            {
                Name = "Boat Template",
                Description = "Tekne kiralama için standart template",
                CategoryId = categories["Boat"].Id,
                IsActive = true,
                TemplateAttributes = new List<ProductTemplateAttribute>
                {
                    new() { AttributeKey = "MaxPassengers", AttributeName = "Maximum Passengers", AttributeType = "Number", Unit = "persons", IsRequired = true, IsSearchable = true, IsFilterable = true, DisplayOrder = 1 },
                    new() { AttributeKey = "BoatLength", AttributeName = "Boat Length", AttributeType = "Number", Unit = "meters", IsRequired = true, IsSearchable = true, IsFilterable = true, DisplayOrder = 2 },
                    new() { AttributeKey = "BoatType", AttributeName = "Boat Type", AttributeType = "Enum", AllowedValues = "[\"Yacht\",\"Speedboat\",\"Sailboat\",\"Catamaran\"]", IsRequired = true, IsSearchable = true, IsFilterable = true, DisplayOrder = 3 },
                    new() { AttributeKey = "CaptainIncluded", AttributeName = "Captain Included", AttributeType = "Boolean", IsSearchable = true, IsFilterable = true, DisplayOrder = 4 }
                }
            };

            // Glasses Template
            var glassesTemplate = new ProductTemplate
            {
                Name = "Glasses Template",
                Description = "Gözlük satışı için standart template",
                CategoryId = categories["Glasses"].Id,
                IsActive = true,
                TemplateAttributes = new List<ProductTemplateAttribute>
                {
                    new() { AttributeKey = "FrameSize", AttributeName = "Frame Size", AttributeType = "Enum", AllowedValues = "[\"Small\",\"Medium\",\"Large\",\"Extra Large\"]", IsRequired = true, IsSearchable = true, IsFilterable = true, DisplayOrder = 1 },
                    new() { AttributeKey = "FrameColor", AttributeName = "Frame Color", AttributeType = "Text", IsRequired = true, IsSearchable = true, IsFilterable = true, DisplayOrder = 2 },
                    new() { AttributeKey = "LensType", AttributeName = "Lens Type", AttributeType = "Enum", AllowedValues = "[\"Single Vision\",\"Bifocal\",\"Progressive\",\"Computer\"]", IsRequired = true, IsSearchable = true, IsFilterable = true, DisplayOrder = 3 },
                    new() { AttributeKey = "UVProtection", AttributeName = "UV Protection", AttributeType = "Boolean", IsSearchable = true, IsFilterable = true, DisplayOrder = 4 }
                }
            };

            context.ProductTemplates.AddRange(villaTemplate, boatTemplate, glassesTemplate);
            await context.SaveChangesAsync();

            templates["Villa"] = villaTemplate;
            templates["Boat"] = boatTemplate;
            templates["Glasses"] = glassesTemplate;

            return templates;
        }

        private static async Task CreateProductsAsync(
            DomainDbContext context, 
            Dictionary<string, Category> categories, 
            Dictionary<string, ProductTemplate> templates)
        {
            // Villa Ürünü
            var villa = new Product
            {
                Name = "Lüks Villa - Antalya",
                Description = "Deniz manzaralı lüks villa, özel havuzlu",
                Price = 2500.00m,
                StockQuantity = 1,
                SKU = "VIL-001",
                OfferingType = ProductOfferingTypes.Rental,
                Brand = "Premium Villas",
                Model = "Luxury Villa",
                Year = 2020,
                Condition = "New",
                CategoryId = categories["Villa"].Id,
                ProductTemplateId = templates["Villa"].Id,
                Attributes = new List<ProductAttribute>
                {
                    new() { AttributeKey = "MaxGuests", AttributeValue = "8", AttributeType = "Number", Unit = "persons", DisplayOrder = 1 },
                    new() { AttributeKey = "Bedrooms", AttributeValue = "4", AttributeType = "Number", DisplayOrder = 2 },
                    new() { AttributeKey = "Bathrooms", AttributeValue = "3", AttributeType = "Number", DisplayOrder = 3 },
                    new() { AttributeKey = "Pool", AttributeValue = "true", AttributeType = "Boolean", DisplayOrder = 4 },
                    new() { AttributeKey = "Location", AttributeValue = "Antalya, Kemer", AttributeType = "Text", DisplayOrder = 5 }
                },
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "https://example.com/villa1.jpg", AltText = "Villa Dış Görünüm", Title = "Villa", IsPrimary = true, DisplayOrder = 1, ImageType = "Thumbnail" },
                    new() { ImageUrl = "https://example.com/villa2.jpg", AltText = "Havuz", Title = "Havuz", IsPrimary = false, DisplayOrder = 2, ImageType = "Gallery" }
                }
            };

            // Tekne Ürünü
            var boat = new Product
            {
                Name = "Lüks Yacht - Bodrum",
                Description = "Denizde unutulmaz anlar için lüks yacht",
                Price = 5000.00m,
                StockQuantity = 1,
                SKU = "BOAT-001",
                OfferingType = ProductOfferingTypes.Rental,
                Brand = "Bodrum Yachts",
                Model = "Luxury Yacht",
                Year = 2019,
                Condition = "Excellent",
                CategoryId = categories["Boat"].Id,
                ProductTemplateId = templates["Boat"].Id,
                Attributes = new List<ProductAttribute>
                {
                    new() { AttributeKey = "MaxPassengers", AttributeValue = "12", AttributeType = "Number", Unit = "persons", DisplayOrder = 1 },
                    new() { AttributeKey = "BoatLength", AttributeValue = "15", AttributeType = "Number", Unit = "meters", DisplayOrder = 2 },
                    new() { AttributeKey = "BoatType", AttributeValue = "Yacht", AttributeType = "Enum", DisplayOrder = 3 },
                    new() { AttributeKey = "CaptainIncluded", AttributeValue = "true", AttributeType = "Boolean", DisplayOrder = 4 }
                },
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "https://example.com/yacht1.jpg", AltText = "Yacht", Title = "Lüks Yacht", IsPrimary = true, DisplayOrder = 1, ImageType = "Thumbnail" }
                }
            };

            // Gözlük Ürünü
            var glasses = new Product
            {
                Name = "Ray-Ban Aviator",
                Description = "Klasik aviator tasarım, altın çerçeveli",
                Price = 299.99m,
                StockQuantity = 50,
                SKU = "GLASS-001",
                OfferingType = ProductOfferingTypes.Purchsase,
                Brand = "Ray-Ban",
                Model = "Aviator",
                Year = 2023,
                Condition = "New",
                CategoryId = categories["Glasses"].Id,
                ProductTemplateId = templates["Glasses"].Id,
                Attributes = new List<ProductAttribute>
                {
                    new() { AttributeKey = "FrameSize", AttributeValue = "Large", AttributeType = "Enum", DisplayOrder = 1 },
                    new() { AttributeKey = "FrameColor", AttributeValue = "Gold", AttributeType = "Text", DisplayOrder = 2 },
                    new() { AttributeKey = "LensType", AttributeValue = "Single Vision", AttributeType = "Enum", DisplayOrder = 3 },
                    new() { AttributeKey = "UVProtection", AttributeValue = "true", AttributeType = "Boolean", DisplayOrder = 4 }
                },
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "https://example.com/glasses1.jpg", AltText = "Ray-Ban Aviator", Title = "Ray-Ban Aviator", IsPrimary = true, DisplayOrder = 1, ImageType = "Thumbnail" }
                }
            };

            context.Products.AddRange(villa, boat, glasses);
        }
    }
} 