using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using TaskAppl.DataAccess.Interfaces;
using TaskAppl.Shared.Helpres;
using TaskAppl.Shared.Interfaces;

namespace TaskAppl.DataAccess.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        async Task<IEnumerable<TEntity>> IGenericRepository<TEntity>.GetAll() => _context.Set<TEntity>();
        
        async Task<TEntity> IGenericRepository<TEntity>.GetById(int id)
        {
            return await GetById(id);
        }

        async Task IGenericRepository<TEntity>.Create(TEntity entity)
        {
            Type type = typeof(TEntity);
            if (type.HasProperty("CreatedAt"))
            {
                var prop = type.GetProperty("CreatedAt");
                prop?.SetValue(entity, DateTime.UtcNow);
            }
            if (type.HasProperty("UpdatedAt"))
            {
                var prop = type.GetProperty("UpdatedAt");
                prop?.SetValue(entity, DateTime.UtcNow);
            }
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        async Task IGenericRepository<TEntity>.Update(int id, TEntity entity)
        {
            Type type = typeof(TEntity);
            if (type.HasProperty("UpdatedAt"))
            {
                var prop = type.GetProperty("UpdatedAt");
                prop?.SetValue(entity, DateTime.UtcNow);
            }
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }

        async Task IGenericRepository<TEntity>.UpdateRange(TEntity[] entity)
        {
            _context.Set<TEntity>().UpdateRange(entity);
            await _context.SaveChangesAsync();
        }

        async Task IGenericRepository<TEntity>.Delete(int id)
        {
            Type type = typeof(TEntity);
            TEntity entity = await GetById(id);
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        async Task<bool> IGenericRepository<TEntity>.IsExists(int id)
        {
            return await _context.Set<TEntity>().AnyAsync(e => e.Id == id);

        }

        DbSet<TEntity> IGenericRepository<TEntity>.GetContext()
        {
            return _context.Set<TEntity>();
        }

        int IGenericRepository<TEntity>.SaveContext() => _context.SaveChanges();

        async Task<int> IGenericRepository<TEntity>.SaveContextAsync() => await _context.SaveChangesAsync();

        EntityEntry<TEntity> IGenericRepository<TEntity>.GetEntry(TEntity entity)
        {
            return _context.Entry(entity);
        }

        private async Task<TEntity> GetById(int id)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id);
        }

    }

}
