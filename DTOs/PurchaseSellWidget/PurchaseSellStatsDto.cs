namespace AshishGeneralStore.DTOs.PurchaseSellWidget
{
    public class PurchaseSellStatsDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalPurchased { get; set; }
        public int TotalSold { get; set; }
    }
}
