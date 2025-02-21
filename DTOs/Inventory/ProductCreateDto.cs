using System.ComponentModel.DataAnnotations;

namespace AshishGeneralStore.DTOs.Inventory
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Single unit price must be greater than 0.")]
        public decimal SingleUnitPrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Bulk unit price must be greater than 0.")]
        public decimal BulkUnitPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Bulk quantity must be at least 1.")]
        public int BulkQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "CategoryId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "A valid CategoryId is required.")]
        public int CategoryId { get; set; }
        public string Description { get; set; }
    }
}
