using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace OnlinePropertyBookingPlatform.Repositories
{
    public class CrudRepository<TEntity> where TEntity : class
    {
        private readonly PropertyManagementContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public CrudRepository(PropertyManagementContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        // Get All
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Get by Id
        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Find by condition
        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        // Add
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // Update
        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        // Delete
        public async Task DeleteAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
