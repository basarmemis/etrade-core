using etrade_core.application.IRepositories;
using etrade_core.domain.ProductModule.Entities;
using etrade_core.domain.OrderModule.Entities;
using etrade_core.domain.CategoryModule.Entities;

namespace etrade_core.persistence.Examples
{
    /// <summary>
    /// Dapper kullanım örnekleri
    /// Performans odaklı sorgular için
    /// </summary>
    public static class DapperUsageExamples
    {
        /// <summary>
        /// Product listesi için projection-only sorgu örneği
        /// </summary>
        public static async Task<IEnumerable<ProductListDto>> GetProductListAsync(IDapperUnitOfWork dapperUow)
        {
            const string sql = @"
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
                ORDER BY p.CreatedDate DESC";

            return await dapperUow.Products.QueryAsync<ProductListDto>(sql);
        }

        /// <summary>
        /// Sayfalama ile product listesi örneği
        /// </summary>
        public static async Task<(IEnumerable<ProductListDto> Items, int TotalCount)> GetProductListWithPaginationAsync(
            IDapperUnitOfWork dapperUow, int pageNumber, int pageSize)
        {
            const string sql = @"
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
                ORDER BY p.CreatedDate DESC";

            return await dapperUow.Products.QueryWithPaginationAsync<ProductListDto>(sql, pageNumber, pageSize);
        }

        /// <summary>
        /// Complex join ile order detayları örneği
        /// </summary>
        public static async Task<IEnumerable<OrderDetailDto>> GetOrderDetailsAsync(IDapperUnitOfWork dapperUow, long orderId)
        {
            const string sql = @"
                SELECT 
                    o.Id,
                    o.OrderNumber,
                    o.Status,
                    o.TotalAmount,
                    o.CreatedDate,
                    up.FirstName,
                    up.LastName,
                    up.Email,
                    oi.Id as OrderItemId,
                    oi.ProductId,
                    oi.Quantity,
                    oi.UnitPrice,
                    p.Name as ProductName,
                    c.Name as CategoryName
                FROM Orders o
                INNER JOIN UserProfiles up ON o.UserProfileId = up.Id
                INNER JOIN OrderItems oi ON o.Id = oi.OrderId
                INNER JOIN Products p ON oi.ProductId = p.Id
                LEFT JOIN Categories c ON p.CategoryId = c.Id
                WHERE o.Id = @OrderId AND o.IsDeleted = false";

            return await dapperUow.Orders.QueryAsync<OrderDetailDto>(sql, new { OrderId = orderId });
        }

        /// <summary>
        /// Stored procedure kullanım örneği
        /// </summary>
        public static async Task<IEnumerable<ProductStatsDto>> GetProductStatisticsAsync(IDapperUnitOfWork dapperUow)
        {
            const string storedProcedureName = "GetProductStatistics";
            return await dapperUow.Products.QueryStoredProcedureAsync<ProductStatsDto>(storedProcedureName);
        }

        /// <summary>
        /// Multiple result set örneği
        /// </summary>
        public static async Task<(IEnumerable<Product> Products, IEnumerable<Category> Categories)> GetProductsAndCategoriesAsync(
            IDapperUnitOfWork dapperUow)
        {
            const string sql = @"
                SELECT * FROM Products WHERE IsDeleted = false;
                SELECT * FROM Categories WHERE IsDeleted = false;";

            var results = await dapperUow.Products.QueryMultipleAsync(sql);
            var resultList = results.ToList();
            
            var products = resultList[0].Cast<Product>();
            var categories = resultList[1].Cast<Category>();
            
            return (products, categories);
        }

        /// <summary>
        /// Bulk insert örneği
        /// </summary>
        public static async Task<int> BulkInsertProductsAsync(IDapperUnitOfWork dapperUow, IEnumerable<Product> products)
        {
            return await dapperUow.Products.BulkInsertAsync(products, "Products");
        }

        /// <summary>
        /// Transaction içinde Dapper kullanım örneği
        /// </summary>
        public static async Task<bool> CreateOrderWithItemsAsync(IDapperUnitOfWork dapperUow, Order order, IEnumerable<OrderItem> items)
        {
            try
            {
                await dapperUow.ExecuteInTransactionAsync(async transaction =>
                {
                    // Order oluştur
                    const string orderSql = @"
                        INSERT INTO Orders (OrderNumber, UserProfileId, Status, TotalAmount, CreatedDate, IsDeleted)
                        VALUES (@OrderNumber, @UserProfileId, @Status, @TotalAmount, @CreatedDate, @IsDeleted)
                        RETURNING Id";

                    var orderId = await dapperUow.Orders.QueryScalarAsync<long>(orderSql, order, transaction);

                    // Order items oluştur
                    foreach (var item in items)
                    {
                        item.OrderId = orderId;
                        const string itemSql = @"
                            INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice, CreatedDate)
                            VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @CreatedDate)";

                        await dapperUow.OrderItems.ExecuteAsync(itemSql, item, transaction);
                    }

                    return true;
                });

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    // DTO classes for examples
    public class ProductListDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ImageCount { get; set; }
    }

    public class OrderDetailDto
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long OrderItemId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    public class ProductStatsDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalQuantity { get; set; }
    }
} 