using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;
using JewelryPOS.App.Data.Interface;
using Microsoft.EntityFrameworkCore;

namespace JewelryPOS.App.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.Repository<Product>().GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsNoTrackingAsync()
        {
            return await _unitOfWork.Repository<Product>()
                .GetQuery()
                .AsNoTracking()
                .Where(i => i.IsActive == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithCategoryAsNoTrackingAsync()
        {
            return await _unitOfWork.Repository<Product>()
                .GetQuery()
                .AsNoTracking()
                .Include(c => c.Category)
                .Where(i => i.IsActive == true)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        }

        public IQueryable<Product> GetQuery()
        {
            return _unitOfWork.Repository<Product>().GetQuery();
        }

        public async Task AddProductAsync(Product product)
        {
            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Guid id)
        {
            await _unitOfWork.Repository<Product>().Remove(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
