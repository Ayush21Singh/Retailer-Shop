using AshishGeneralStore.Models;
using AshishGeneralStore.DTOs.Inventory;
namespace AshishGeneralStore.Services.Admin
{
    public interface IInventoryService
    {
        Task<List<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int productId);
        Task<Product> AddProductAsync(ProductCreateDto product);
        Task<bool> UpdateProductAsync(int productId, ProductUpdateDto product);
        Task<bool> DeleteProductAsync(int productId);
    }
}
