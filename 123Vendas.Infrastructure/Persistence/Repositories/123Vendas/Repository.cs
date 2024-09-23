using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using _123Vendas.Core.Repositories._123Vendas;
using _123Vendas.Infrastructure.Persistence.Context;
using EFCore.BulkExtensions;

namespace _123Vendas.Infrastructure.Persistence.Repositories._123Vendas
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbSet<T> _dbSet;
        protected _123VendasDbContext _dbContext;

        public Repository(_123VendasDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<T?> GetById(int Id)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                var entity = await _dbSet.FindAsync(Id);
                if (entity != null)
                    _dbContext.Entry(entity).State = EntityState.Detached;
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | GetById | {ex?.InnerException?.Message}");
            }

        }

        public async Task<IEnumerable<T>> GetAll()
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                return await _dbSet.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | GetAll | {ex?.InnerException?.Message}");
            }
        }

        public async Task<int> Create(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | Create | {ex?.InnerException?.Message}");
            }
        }

        public async Task<T> CreateScope(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | CreateScope | {ex?.InnerException?.Message}");
            }
        }

        public async Task<int> CreateRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                await _dbContext.BulkInsertAsync(entities, options => options.BatchSize = 1000);
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | CreateRangeAsync | {ex?.InnerException?.Message}");
            }

        }
        public async Task<int> CreateOrUpdateRangeAsync(IEnumerable<T> entities)
        {
            try
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                await _dbContext.BulkInsertOrUpdateAsync(entities, options => options.BatchSize = 1000);
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | CreateOrUpdateRangeAsync | {ex?.InnerException?.Message}");
            }
        }
        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> prPredicate)
        {
            try
            {
                var linq = _dbSet
                    .Where(prPredicate)
                    .AsQueryable()
                    .AsNoTracking();
                IEnumerable<T> result = await linq.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | GetAll | {ex?.InnerException?.Message}");
            }

        }
        public async Task<IEnumerable<T>> GetAll(Func<IQueryable<T>, IQueryable<T>> includes)
        {
            try
            {
                IQueryable<T> query = _dbSet.AsNoTracking();
                if (includes != null)
                {
                    query = includes(query);
                }
                IEnumerable<T> results = await query.ToListAsync();
                foreach (T result in results)
                {
                    _dbContext.Entry(result).State = EntityState.Detached;
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | GetAll | {ex?.InnerException?.Message}");
            }

        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> prPredicate, Func<IQueryable<T>, IQueryable<T>> includes)
        {
            try
            {
                IQueryable<T> query = _dbSet.AsNoTracking();
                if (includes != null)
                {
                    query = includes(query);
                }
                IEnumerable<T> results = await query.Where(prPredicate).ToListAsync();
                foreach (T result in results)
                {
                    _dbContext.Entry(result).State = EntityState.Detached;
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | GetAll | {ex?.InnerException?.Message}");
            }
        }

        public async Task<T?> Get(Expression<Func<T, bool>> prPredicate)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                var linq = _dbSet
                    .Where(prPredicate)
                    .AsQueryable();
                T? result = await linq.FirstOrDefaultAsync();

                if (result != null)
                    _dbContext.Entry(result).State = EntityState.Detached;

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | Get | {ex?.InnerException?.Message}");
            }

        }
        public async Task<T?> Get(Expression<Func<T, bool>> prPredicate, Func<IQueryable<T>, IQueryable<T>> includes)
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                var linq = _dbSet.Where(prPredicate);

                if (includes != null)
                {
                    linq = includes(linq);
                }

                T? result = await linq.FirstOrDefaultAsync();

                if (result != null)
                    _dbContext.Entry(result).State = EntityState.Detached;

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | Get | {ex?.InnerException?.Message}");
            }
        }

        public async Task<T> Update(T entity)
        {
            try
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
                _dbSet.Update(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | Update | {ex?.InnerException?.Message}");
            }

        }

        public async Task<ICollection<T>> UpdateRange(ICollection<T> entities)
        {
            try
            {
                _dbSet.UpdateRange(entities);
                await _dbContext.SaveChangesAsync();
                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | UpdateRange | {ex?.InnerException?.Message}");
            }
        }

        public async Task SaveChanges()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | SaveChanges | {ex?.InnerException?.Message}");
            }

        }

        public async Task<int> Delete(T entity)
        {
            try
            {
                _dbContext.Remove(entity);
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | Delete | {ex?.InnerException?.Message}");
            }

        }

        public async Task<int> DeleteRange(IEnumerable<T> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | DeleteRange | {ex?.InnerException?.Message}");
            }

        }

        public async Task<int> Delete(Expression<Func<T, bool>> prPredicate)
        {
            try
            {
                var entityRange = _dbSet
                .Where(prPredicate)
                .AsEnumerable();
                _dbSet.RemoveRange(entityRange);
                return await _dbContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | Delete | {ex?.InnerException?.Message}");
            }

        }

        public async Task UpdateRange(IEnumerable<T> entities)
        {
            try
            {
                _dbContext.UpdateRange(entities);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | UpdateRange | {ex?.InnerException?.Message}");
            }

        }

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            try
            {
                return await _dbContext.Database.BeginTransactionAsync();

            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | BeginTransaction | {ex?.InnerException?.Message}");
            }

        }

        public async Task<List<T>> CreateRangeEntities(List<T> entities)
        {
            try
            {
                await _dbSet.AddRangeAsync(entities);
                await _dbContext.SaveChangesAsync();
                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | CreateRangeEntities | {ex?.InnerException?.Message}");
            }

        }

        public async Task<string?> DesManagerDecrypt(string value)
        {
            try
            {
                return await _dbContext.Database.SqlQuery<string>($"SELECT dbo.DesManagerDecrypt({value}) AS Value").FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | DesManagerDecrypt | {ex?.InnerException?.Message}");
            }

        }

        public async Task<string?> DesManagerEncrypt(string value)
        {
            try
            {
                return await _dbContext.Database.SqlQuery<string>($"SELECT dbo.DesManagerEncrypt({value}) AS Value").FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Repository | DesManagerDecrypt | {ex?.InnerException?.Message}");
            }
        }
    }
}
