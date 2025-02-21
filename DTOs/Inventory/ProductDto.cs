namespace AshishGeneralStore.DTOs.Inventory
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal SingleUnitPrice { get; set; }
        public decimal BulkUnitPrice { get; set; }
        public int BulkQuantity { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }
}
