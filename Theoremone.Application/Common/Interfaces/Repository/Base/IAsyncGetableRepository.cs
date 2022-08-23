using System.Linq.Expressions;
using Theoremone.SmartAc.Domain.Entities;

namespace Theoremone.SmartAc.Application.Common.Interfaces.Repository
{
  public   interface IReadableAsyncRepository<TEntity,Tkey> where TEntity : BaseEntity
    {
       
        Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> AsQueryable();

        Task<TEntity?> GetAsync(Tkey id);

        Task<TEntity?> FiratOrDefault(Expression<Func<TEntity, bool>> predicate);
       

    }
}
