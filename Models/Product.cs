namespace AshishGeneralStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal SingleUnitPrice { get; set; } // Price for a single unit
        public decimal BulkUnitPrice { get; set; }  // Price per unit when buying in bulk
        public int BulkQuantity { get; set; }       // Minimum quantity for bulk pricing
        public int Stock { get; set; }              // Total available stock
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
