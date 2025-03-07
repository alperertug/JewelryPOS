using System.Linq.Expressions;

namespace JewelryPOS.App.Data.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindByUsernameAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindSingularAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetQuery();

        Task AddAsync(T entity);
        void Update(T entity);
        Task Remove(Guid id);
    }
}
