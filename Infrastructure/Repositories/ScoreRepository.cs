using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;
using System.Reflection.Metadata.Ecma335;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class ScoreRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ScoreDbContext _context;

        public ScoreRepository(ScoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task<T?> FirstOrDefaultAsync(Func<IQueryable<T>, IQueryable<T>>? func = null)
        {
            var query = _context.Set<T>().AsQueryable();
            var resultQuery = func == null ? query : func(query);
            return await resultQuery.FirstOrDefaultAsync();
        }

        public async Task<T?> SingleOrDefaultAsync(Func<IQueryable<T>, IQueryable<T>>? func)
        {
            var query = _context.Set<T>().AsQueryable();
            var resultQuery = func == null ? query : func(query);
            return await resultQuery.SingleOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
