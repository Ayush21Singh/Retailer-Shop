using System.ComponentModel.DataAnnotations;

namespace AshishGeneralStore.DTOs.Inventory
{
    public class CategoryCreateUpdateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }
    }
}
