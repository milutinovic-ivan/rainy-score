using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T?> FirstOrDefaultAsync(Func<IQueryable<T>, IQueryable<T>>? func);
        Task<T?> SingleOrDefaultAsync(Func<IQueryable<T>, IQueryable<T>>? func);
        Task SaveChangesAsync();
    }
}
