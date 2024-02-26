using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace TaskAppl.DataAccess.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity> GetById(int id);
        Task Create(TEntity entity);
        Task Update(int id, TEntity entity);
        Task UpdateRange(TEntity[] entity);
        Task Delete(int id);
        Task<bool> IsExists(int id);
        DbSet<TEntity> GetContext();
        int SaveContext();
        Task<int> SaveContextAsync();
        EntityEntry<TEntity> GetEntry(TEntity entity);

    }
}
