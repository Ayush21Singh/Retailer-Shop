using AshishGeneralStore.DTOs.PurchaseSellWidget;

namespace AshishGeneralStore.Services.Admin.Dashboard
{
    public interface IAnalyticsService
    {
        Task<List<PurchaseSellStatsDto>> GetPurchaseSellStatsAsync(string periodType, int? daysCount, DateTime? startDate, DateTime? endDate, int? categoryId);
    }
}
