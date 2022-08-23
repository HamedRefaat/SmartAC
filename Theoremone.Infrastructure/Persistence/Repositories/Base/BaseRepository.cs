using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using Theoremone.SmartAc.Application.Common.Interfaces.Repository;
using Theoremone.SmartAc.Domain.Entities;

namespace Theoremone.SmartAc.Infrastructure.Persistence.Repositories.Base
{
    public abstract class BaseRepository<TEntity, TKey> : IReadableAsyncRepository<TEntity, TKey>, IUpdatableAsyncRepository<TEntity> where TEntity : BaseEntity
    {

        private readonly SmartAcContext _context;
        private readonly DbSet<TEntity> _entities;
        public BaseRepository(SmartAcContext context)
        {
            this._context = context;
            _entities = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            await _entities.AddAsync(entity);
        }

        public void DeleteAsync(TEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));
            _entities.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _entities.Update(entity);
        }

        public void UpdateMultible(IEnumerable<TEntity> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }


        public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _entities.AsNoTracking().Where(predicate).ToListAsync();
        }

        public IQueryable<TEntity> AsQueryable()
        {

            return _entities.AsQueryable();
        }

        public async Task<TEntity?> GetAsync(TKey id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));
            return await _entities.FindAsync(id);
        }

        public async Task<TEntity?> FiratOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return await _entities.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _entities.AnyAsync(predicate);
        }

        public async Task AddMultibleAsync(IEnumerable<TEntity> entities)
        {
            await _entities.AddRangeAsync(entities);
        }


    }

}
