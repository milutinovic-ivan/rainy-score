using Application.Intefraces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories
{
    public class EfUnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly ScoreDbContext _context;
        private IDbContextTransaction? _tx;

        public EfUnitOfWork(ScoreDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            _tx = await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_tx == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            await _tx.CommitAsync(ct);
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_tx == null)
            {
                throw new InvalidOperationException("Transaction not started.");
            }

            await _tx.RollbackAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }

        public async ValueTask DisposeAsync()
        {
            if (_tx != null)
            {
                await _tx.DisposeAsync();
                _tx = null;
            }
        }
    }
}
