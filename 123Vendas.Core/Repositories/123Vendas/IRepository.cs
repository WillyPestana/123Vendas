using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace _123Vendas.Core.Repositories._123Vendas
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetById(int Id);
        Task<IEnumerable<T>> GetAll();
        Task<int> Create(T entity);
        Task<T> CreateScope(T entity);
        Task<int> CreateRangeAsync(IEnumerable<T> entities);
        Task<int> CreateOrUpdateRangeAsync(IEnumerable<T> entities);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> prPredicate);
        Task<IEnumerable<T>> GetAll(Func<IQueryable<T>, IQueryable<T>> includes);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> prPredicate, Func<IQueryable<T>, IQueryable<T>> includes);
        Task<T?> Get(Expression<Func<T, bool>> prPredicate);
        Task<T?> Get(Expression<Func<T, bool>> prPredicate, Func<IQueryable<T>, IQueryable<T>> includes);
        Task<T> Update(T entity);
        Task<ICollection<T>> UpdateRange(ICollection<T> entities);
        Task SaveChanges();
        Task<int> Delete(T entity);
        Task<int> DeleteRange(IEnumerable<T> entities);
        Task<int> Delete(Expression<Func<T, bool>> prPredicate);
        Task UpdateRange(IEnumerable<T> entities);
        Task<IDbContextTransaction> BeginTransaction();
        Task<List<T>> CreateRangeEntities(List<T> entities);
        Task<string?> DesManagerDecrypt(string value);
        Task<string?> DesManagerEncrypt(string value);
    }
}
