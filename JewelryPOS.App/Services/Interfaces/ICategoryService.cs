using JewelryPOS.App.Models;

namespace JewelryPOS.App.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<Category>> GetAllCategoriesAsNoTrackingAsync();
        Task<Category> GetCategoryByIdAsync(Guid id);
        IQueryable<Category> GetQuery();
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Guid id);
    }
}
