using Theoremone.SmartAc.Domain.Entities;

namespace Theoremone.SmartAc.Application.Common.Interfaces.Repository
{
    public interface IUpdatableAsyncRepository<TEntity> where TEntity : BaseEntity
    {
        Task AddAsync(TEntity entity);
        Task AddMultibleAsync(IEnumerable <TEntity> entities);
        void Update(TEntity entity);
        void UpdateMultible(IEnumerable<TEntity> entities);
        void DeleteAsync(TEntity entity);
        Task SaveChangesAsync();
    }
}
