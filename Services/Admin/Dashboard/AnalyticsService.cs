using AshishGeneralStore.Data;
using AshishGeneralStore.DTOs.PurchaseSellWidget;
using Microsoft.EntityFrameworkCore;

namespace AshishGeneralStore.Services.Admin.Dashboard
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly StoreDbContext _context;
        public AnalyticsService(StoreDbContext context)
        {
            _context = context;
        }
        public async Task<List<PurchaseSellStatsDto>> GetPurchaseSellStatsAsync(string periodType, int? daysCount, DateTime? startDate, DateTime? endDate, int? categoryId)
        {
            // Determine date range based on periodType
            DateTime end = endDate ?? DateTime.UtcNow;
            DateTime start;

            switch (periodType.ToLower())
            {
                case "day":
                    start = end.Date;
                    break;
                case "week":
                    start = end.AddDays(-7).Date;
                    break;
                case "days":
                    start = end.AddDays(-(daysCount ?? 7)).Date; // Default to 7 if daysCount null
                    break;
                case "month":
                    start = end.AddMonths(-1).Date;
                    break;
                default:
                    start = startDate ?? end.AddDays(-7).Date; // Default to 7 days if custom range incomplete
                    break;
            }

            // Query purchases
            var purchaseQuery = _context.Purchases
                .Where(p => p.PurchaseDate >= start && p.PurchaseDate <= end);
            if (categoryId.HasValue)
            {
                purchaseQuery = purchaseQuery.Where(p => p.Product.CategoryId == categoryId.Value);
            }
            var purchaseStats = await purchaseQuery
                .GroupBy(p => p.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalPurchased = g.Sum(p => p.Quantity)
                })
                .ToListAsync();

            // Query sales
            var sellQuery = _context.Orders
                .Where(o => o.OrderDate >= start && o.OrderDate <= end);
            if (categoryId.HasValue)
            {
                sellQuery = sellQuery.Where(o => o.Product.CategoryId == categoryId.Value);
            }
            var sellStats = await sellQuery
                .GroupBy(o => o.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(o => o.Quantity)
                })
                .ToListAsync();

            // Join with products
            var productsQuery = _context.Products.AsQueryable();
            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }
            var products = await productsQuery
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();

            var stats = from product in products
                        join purchase in purchaseStats
                            on product.Id equals purchase.ProductId into purchases
                        from purchase in purchases.DefaultIfEmpty()
                        join sell in sellStats
                            on product.Id equals sell.ProductId into sells
                        from sell in sells.DefaultIfEmpty()
                        select new PurchaseSellStatsDto
                        {
                            ProductId = product.Id,
                            ProductName = product.Name,
                            TotalPurchased = purchase?.TotalPurchased ?? 0,
                            TotalSold = sell?.TotalSold ?? 0
                        };

            return stats.ToList();
        }
    }
}
