using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Weather.Domain.Models;

namespace Weather.Infrastructure.Repository
{
   public interface IBaseRepository<TEntity> : IDisposable where TEntity : class, IEntity
   {
      Task<TEntity> CreateAsync(TEntity entity);
      Task<TEntity> UpdateAsync(TEntity entity);
      Task<TEntity> DeleteAsync(TEntity entity);
      Task<IEnumerable<TEntity>> GetAllAsync();
      Task<TEntity?> GetByIdAsync(int id);
      Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<int> ids);
      Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
      Task<IEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
      Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
      Task<int> CountAsync();
      Task<int> PredicateCountAsync(Expression<Func<TEntity, bool>> predicate);
      Task<int> SaveAsync(CancellationToken? cancellationToken);
   }
   public class BaseRepository<TEntity>(DbContext context) : IBaseRepository<TEntity> where TEntity : class, IEntity
   {
      protected readonly DbContext _context = context;

      public async Task<int> CountAsync()
      {
         return await GetDbSetAsync().CountAsync();
      }

      public async Task<TEntity> CreateAsync(TEntity entity)
      {
         return (await GetDbSetAsync().AddAsync(entity)).Entity;
      }

      public async Task<TEntity> DeleteAsync(TEntity entity)
      {
         return GetDbSetAsync().Remove(entity).Entity;
      }

      public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
      {
         return await GetDbSetAsync().AnyAsync(predicate);
      }

      public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
      {
         return await GetDbSetAsync().Where(predicate).ToListAsync();
      }

      public async Task<IEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
      {
         return await GetDbSetAsync().FirstOrDefaultAsync(predicate);
      }

      public async Task<IEnumerable<TEntity>> GetAllAsync()
      {
         return await GetDbSetAsync().ToListAsync();
      }

      public async Task<TEntity?> GetByIdAsync(int id)
      {
         return await GetDbSetAsync().FirstOrDefaultAsync(e => e.Id == id);
      }

      public async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<int> ids)
      {
         return await GetDbSetAsync().Where(e => ids.Contains(e.Id)).ToListAsync();
      }

      public async Task<int> PredicateCountAsync(Expression<Func<TEntity, bool>> predicate)
      {
         return await GetDbSetAsync().CountAsync(predicate);
      }

      public async Task<int> SaveAsync(CancellationToken? cancellationToken)
      {
         return await _context.SaveChangesAsync(cancellationToken ?? CancellationToken.None);
      }

      public async Task<TEntity> UpdateAsync(TEntity entity)
      {
         return GetDbSetAsync().Update(entity).Entity;
      }

      protected DbSet<TEntity> GetDbSetAsync()
      {
         return _context.Set<TEntity>();
      }

      public void Dispose()
      {
         _context?.Dispose();
      }
   }
}
