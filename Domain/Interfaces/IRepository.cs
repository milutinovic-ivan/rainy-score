using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetQuery();
        Task<T?> GetByIdAsync(int id);
        Task<T?> FirstOrDefaultAsync(Func<IQueryable<T>, IQueryable<T>>? func);
        Task<T?> SingleOrDefaultAsync(Func<IQueryable<T>, IQueryable<T>>? func);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        Task DeleteAll();
        Task SaveChangesAsync();
    }
}
