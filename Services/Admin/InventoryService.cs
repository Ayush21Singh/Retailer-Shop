using AshishGeneralStore.Data;
using AshishGeneralStore.Models;
using AshishGeneralStore.Common;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using AshishGeneralStore.DTOs.Inventory;

namespace AshishGeneralStore.Services.Admin
{
    public class InventoryService : IInventoryService
    {
        private readonly StoreDbContext _context;
        private readonly ElasticsearchClient _esClient;

        public InventoryService(StoreDbContext context, ElasticsearchService esService)
        {
            _context = context;
            _esClient = esService.Client; ;
        }
        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            return await _context.Products
            .Include(p => p.Category)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                SingleUnitPrice = p.SingleUnitPrice,
                BulkUnitPrice = p.BulkUnitPrice,
                BulkQuantity = p.BulkQuantity,
                Stock = p.Stock,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Description = p.Description
            })
            .ToListAsync();
        }

        public async Task<ProductDto> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products
             .Include(p => p.Category)
             .Select(p => new ProductDto
             {
                 Id = p.Id,
                 Name = p.Name,
                 SingleUnitPrice = p.SingleUnitPrice,
                 BulkUnitPrice = p.BulkUnitPrice,
                 BulkQuantity = p.BulkQuantity,
                 Stock = p.Stock,
                 CategoryId = p.CategoryId,
                 CategoryName = p.Category.Name,
                 Description = p.Description
             })
             .FirstOrDefaultAsync(p => p.Id == productId);

            return product ?? throw new Exception($"Product with ID {productId} not found.");
        }

        public async Task<Product> AddProductAsync(ProductCreateDto productDto)
        {
            // Add product to the database
            var product = new Product
            {
                Name = productDto.Name,
                SingleUnitPrice = productDto.SingleUnitPrice,
                BulkUnitPrice = productDto.BulkUnitPrice,
                BulkQuantity = productDto.BulkQuantity,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId,
                Description = productDto.Description
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Index in Elasticsearch
            var response = await _esClient.IndexAsync(product, i => i
                .Id(product.Id.ToString()) // Ensure correct ID
                .Index(Constants.Elasticsearch.ProductsIndex) // Specify the index
            );

            if (!response.IsValidResponse)
            {
                Console.WriteLine($"Elasticsearch indexing failed: {response.DebugInformation}");
            }

            return product;
        }

        public async Task<bool> UpdateProductAsync(int productId, ProductUpdateDto updatedProductDto)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            // Update all fields, including new pricing fields
            product.Name = updatedProductDto.Name;
            product.SingleUnitPrice = updatedProductDto.SingleUnitPrice;
            product.BulkUnitPrice = updatedProductDto.BulkUnitPrice;
            product.BulkQuantity = updatedProductDto.BulkQuantity;
            product.Stock = updatedProductDto.Stock;
            product.CategoryId = updatedProductDto.CategoryId;
            product.Description = updatedProductDto.Description;

            await _context.SaveChangesAsync();

            // Update in Elasticsearch
            var response = await _esClient.IndexAsync(product, i => i
                .Id(productId.ToString())
                .Index(Constants.Elasticsearch.ProductsIndex));

            if (!response.IsValidResponse)
            {
                Console.WriteLine($"Elasticsearch indexing failed: {response.DebugInformation}");
            }

            return true;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Delete from Elasticsearch
            var deleteResponse = await _esClient.DeleteAsync<Product>(
                Constants.Elasticsearch.ProductsIndex, productId.ToString());

            if (!deleteResponse.IsValidResponse)
            {
                Console.WriteLine($"Elasticsearch deletion failed: {deleteResponse.DebugInformation}");
            }

            return true;
        }
    }
}
