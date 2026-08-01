using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Common;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Persistence.Repositories
{
    public class Repository<T> : IRepository<T>
        where T : BaseEntity
    {
        protected readonly AppDbContext _context;

        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}