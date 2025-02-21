namespace AshishGeneralStore.Models;

public class Order
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerItem { get; set; } // Price per unit at sale time
    public DateTime OrderDate { get; set; }
    public int? UserId { get; set; } // Nullable for guest purchases
    public User User { get; set; }
    public decimal TotalPrice => Quantity * PricePerItem; // Calculated, not stored
    public string OrderStatus { get; set; } // e.g., "Pending", "Shipped", "Delivered"
    public DateTime? ShippedDate { get; set; } // For shipping analytics
}