-- MultiTenant Infrastructure Migration Script
-- Bu script mevcut tablolara TenantId kolonu ekler ve gerekli index'leri oluşturur

-- 1. Tenants tablosunu oluştur (eğer yoksa)
CREATE TABLE IF NOT EXISTS "Tenants" (
    "Id" SERIAL PRIMARY KEY,
    "TenantId" VARCHAR(50) NOT NULL UNIQUE,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "ConnectionString" TEXT,
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    "Configuration" TEXT,
    "CreatedDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedDate" TIMESTAMP,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT false
);

-- 2. Default tenant'ı ekle
INSERT INTO "Tenants" ("TenantId", "Name", "Description", "IsActive", "CreatedDate")
VALUES ('default', 'Default Tenant', 'Default tenant for existing data', true, CURRENT_TIMESTAMP)
ON CONFLICT ("TenantId") DO NOTHING;

-- 3. Mevcut tablolara TenantId kolonu ekle
-- UserProfiles tablosu
ALTER TABLE "UserProfiles" ADD COLUMN IF NOT EXISTS "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';

-- Products tablosu
ALTER TABLE "Products" ADD COLUMN IF NOT EXISTS "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';

-- Orders tablosu
ALTER TABLE "Orders" ADD COLUMN IF NOT EXISTS "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';

-- OrderItems tablosu
ALTER TABLE "OrderItems" ADD COLUMN IF NOT EXISTS "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';

-- Categories tablosu
ALTER TABLE "Categories" ADD COLUMN IF NOT EXISTS "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';

-- ProductAttributes tablosu
ALTER TABLE "ProductAttributes" ADD COLUMN IF NOT EXISTS "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';

-- ProductImages tablosu
ALTER TABLE "ProductImages" ADD COLUMN IF NOT EXISTS "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';

-- ProductPriceHistories tablosu
ALTER TABLE "ProductPriceHistories" ADD COLUMN IF NOT EXISTS "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';

-- ProductTemplates tablosu
ALTER TABLE "ProductTemplates" ADD COLUMN IF NOT EXISTS "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';

-- ProductTemplateAttributes tablosu
ALTER TABLE "ProductTemplateAttributes" ADD COLUMN IF NOT EXISTS "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';

-- OrderItemAttributes tablosu
ALTER TABLE "OrderItemAttributes" ADD COLUMN IF NOT EXISTS "TenantId" VARCHAR(50) NOT NULL DEFAULT 'default';

-- 4. TenantId kolonları için index'ler oluştur
-- Composite index'ler performance için önemli
CREATE INDEX IF NOT EXISTS "IX_UserProfiles_TenantId" ON "UserProfiles" ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_UserProfiles_TenantId_IsDeleted" ON "UserProfiles" ("TenantId", "IsDeleted");

CREATE INDEX IF NOT EXISTS "IX_Products_TenantId" ON "Products" ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_Products_TenantId_CategoryId" ON "Products" ("TenantId", "CategoryId");
CREATE INDEX IF NOT EXISTS "IX_Products_TenantId_IsDeleted" ON "Products" ("TenantId", "IsDeleted");

CREATE INDEX IF NOT EXISTS "IX_Orders_TenantId" ON "Orders" ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_Orders_TenantId_UserProfileId" ON "Orders" ("TenantId", "UserProfileId");
CREATE INDEX IF NOT EXISTS "IX_Orders_TenantId_Status" ON "Orders" ("TenantId", "Status");
CREATE INDEX IF NOT EXISTS "IX_Orders_TenantId_IsDeleted" ON "Orders" ("TenantId", "IsDeleted");

CREATE INDEX IF NOT EXISTS "IX_OrderItems_TenantId" ON "OrderItems" ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_OrderItems_TenantId_OrderId" ON "OrderItems" ("TenantId", "OrderId");
CREATE INDEX IF NOT EXISTS "IX_OrderItems_TenantId_IsDeleted" ON "OrderItems" ("TenantId", "IsDeleted");

CREATE INDEX IF NOT EXISTS "IX_Categories_TenantId" ON "Categories" ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_Categories_TenantId_ParentCategoryId" ON "Categories" ("TenantId", "ParentCategoryId");
CREATE INDEX IF NOT EXISTS "IX_Categories_TenantId_IsDeleted" ON "Categories" ("TenantId", "IsDeleted");

CREATE INDEX IF NOT EXISTS "IX_ProductAttributes_TenantId" ON "ProductAttributes" ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_ProductAttributes_TenantId_ProductId" ON "ProductAttributes" ("TenantId", "ProductId");
CREATE INDEX IF NOT EXISTS "IX_ProductAttributes_TenantId_IsDeleted" ON "ProductAttributes" ("TenantId", "IsDeleted");

CREATE INDEX IF NOT EXISTS "IX_ProductImages_TenantId" ON "ProductImages" ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_ProductImages_TenantId_ProductId" ON "ProductImages" ("TenantId", "ProductId");
CREATE INDEX IF NOT EXISTS "IX_ProductImages_TenantId_IsDeleted" ON "ProductImages" ("TenantId", "IsDeleted");

CREATE INDEX IF NOT EXISTS "IX_ProductPriceHistories_TenantId" ON "ProductPriceHistories" ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_ProductPriceHistories_TenantId_ProductId" ON "ProductPriceHistories" ("TenantId", "ProductId");
CREATE INDEX IF NOT EXISTS "IX_ProductPriceHistories_TenantId_IsDeleted" ON "ProductPriceHistories" ("TenantId", "IsDeleted");

CREATE INDEX IF NOT EXISTS "IX_ProductTemplates_TenantId" ON "ProductTemplates" ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_ProductTemplates_TenantId_CategoryId" ON "ProductTemplates" ("TenantId", "CategoryId");
CREATE INDEX IF NOT EXISTS "IX_ProductTemplates_TenantId_IsDeleted" ON "ProductTemplates" ("TenantId", "IsDeleted");

CREATE INDEX IF NOT EXISTS "IX_ProductTemplateAttributes_TenantId" ON "ProductTemplateAttributes" ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_ProductTemplateAttributes_TenantId_ProductTemplateId" ON "ProductTemplateAttributes" ("TenantId", "ProductTemplateId");
CREATE INDEX IF NOT EXISTS "IX_ProductTemplateAttributes_TenantId_IsDeleted" ON "ProductTemplateAttributes" ("TenantId", "IsDeleted");

CREATE INDEX IF NOT EXISTS "IX_OrderItemAttributes_TenantId" ON "OrderItemAttributes" ("TenantId");
CREATE INDEX IF NOT EXISTS "IX_OrderItemAttributes_TenantId_OrderItemId" ON "OrderItemAttributes" ("TenantId", "OrderItemId");
CREATE INDEX IF NOT EXISTS "IX_OrderItemAttributes_TenantId_IsDeleted" ON "OrderItemAttributes" ("TenantId", "IsDeleted");

-- 5. Foreign key constraint'leri güncelle (eğer gerekirse)
-- Not: PostgreSQL'de foreign key constraint'ler otomatik olarak TenantId'yi içermez
-- Bu yüzden application seviyesinde tenant isolation sağlanır

-- 6. Row Level Security (RLS) - Opsiyonel
-- Güvenlik için RLS'yi etkinleştir
-- ALTER TABLE "UserProfiles" ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE "Products" ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE "Orders" ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE "OrderItems" ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE "Categories" ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE "ProductAttributes" ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE "ProductImages" ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE "ProductPriceHistories" ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE "ProductTemplates" ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE "ProductTemplateAttributes" ENABLE ROW LEVEL SECURITY;
-- ALTER TABLE "OrderItemAttributes" ENABLE ROW LEVEL SECURITY;

-- 7. RLS Policy'leri oluştur (eğer RLS etkinse)
-- CREATE POLICY tenant_isolation_policy ON "UserProfiles"
--     FOR ALL
--     USING (TenantId = current_setting('app.current_tenant_id')::text);

-- CREATE POLICY tenant_isolation_policy ON "Products"
--     FOR ALL
--     USING (TenantId = current_setting('app.current_tenant_id')::text);

-- CREATE POLICY tenant_isolation_policy ON "Orders"
--     FOR ALL
--     USING (TenantId = current_setting('app.current_tenant_id')::text);

-- CREATE POLICY tenant_isolation_policy ON "OrderItems"
--     FOR ALL
--     USING (TenantId = current_setting('app.current_tenant_id')::text);

-- CREATE POLICY tenant_isolation_policy ON "Categories"
--     FOR ALL
--     USING (TenantId = current_setting('app.current_tenant_id')::text);

-- CREATE POLICY tenant_isolation_policy ON "ProductAttributes"
--     FOR ALL
--     USING (TenantId = current_setting('app.current_tenant_id')::text);

-- CREATE POLICY tenant_isolation_policy ON "ProductImages"
--     FOR ALL
--     USING (TenantId = current_setting('app.current_tenant_id')::text);

-- CREATE POLICY tenant_isolation_policy ON "ProductPriceHistories"
--     FOR ALL
--     USING (TenantId = current_setting('app.current_tenant_id')::text);

-- CREATE POLICY tenant_isolation_policy ON "ProductTemplates"
--     FOR ALL
--     USING (TenantId = current_setting('app.current_tenant_id')::text);

-- CREATE POLICY tenant_isolation_policy ON "ProductTemplateAttributes"
--     FOR ALL
--     USING (TenantId = current_setting('app.current_tenant_id')::text);

-- CREATE POLICY tenant_isolation_policy ON "OrderItemAttributes"
--     FOR ALL
--     USING (TenantId = current_setting('app.current_tenant_id')::text);

-- 8. Migration tamamlandı mesajı
DO $$
BEGIN
    RAISE NOTICE 'MultiTenant migration completed successfully!';
    RAISE NOTICE 'All tables now have TenantId column with proper indexing.';
    RAISE NOTICE 'Default tenant "default" has been created.';
    RAISE NOTICE 'Remember to update your application configuration to use tenant services.';
END $$; 