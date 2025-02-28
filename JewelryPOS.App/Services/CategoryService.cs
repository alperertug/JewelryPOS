using JewelryPOS.App.Data.Interface;
using JewelryPOS.App.Models;
using JewelryPOS.App.Services.Interfaces;

namespace JewelryPOS.App.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Repository<Category>().GetAllAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<Category>().GetByIdAsync(id);
        }

        public IQueryable<Category> GetQuery()
        {
            return _unitOfWork.Repository<Category>().GetQuery();
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _unitOfWork.Repository<Category>().Update(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            await _unitOfWork.Repository<Category>().Remove(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
