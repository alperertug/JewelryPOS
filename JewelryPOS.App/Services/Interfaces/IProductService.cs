using JewelryPOS.App.Models;

namespace JewelryPOS.App.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetAllProductsAsNoTrackingAsync();
        Task<IEnumerable<Product>> GetAllProductsWithCategoryAsNoTrackingAsync();
        Task<Product> GetProductByIdAsync(Guid id);
        Task<Product> GetProductByBarcodeAsync(string barcode);
        IQueryable<Product> GetQuery();
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Guid id);
    }
}
