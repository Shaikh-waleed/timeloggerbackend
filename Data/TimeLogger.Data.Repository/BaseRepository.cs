using TimeLogger.Data.Database;
using TimeLogger.Data.IRepository;
using TimeLogger.Infrastructure.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TimeLogger.Data.Repository
{
    public class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : class
    {
        private IApplicationDbContext dbContext;
        private DbSet<TEntity> dbSet;

        public DbSet<TEntity> DbSet
        {
            get { return dbSet; }
            private set { dbSet = value; }
        }

        public IApplicationDbContext DbContext
        {
            get { return dbContext; }
            private set { dbContext = value; }
        }

        public BaseRepository(IApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<TEntity>();
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            if (Check.NotNull(where))
                query = query.Where(where);
            if (Check.NotNull(includeProperties))
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            if (Check.NotNull(orderBy))
                return await orderBy(query).FirstOrDefaultAsync();

            return await query.FirstOrDefaultAsync();
        }

        public virtual IQueryable<TEntity> Get()
        {
            return dbSet.AsQueryable();
        }

        public virtual async Task<TEntity> GetAsync(TKey id)
        {
            var entity = await dbSet.FindAsync(id);
            dbContext.Entry(entity).State = EntityState.Detached;
            return entity;
        }


        public virtual async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = dbSet;

            if (Check.NotNull(where))
                query = query.Where(where);
            if (Check.NotNull(includeProperties))
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            if (Check.NotNull(orderBy))              
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        private TEntity Add(TEntity entity)
        {
            var entry = dbSet.Add(entity).Entity;

            return entry;
        }

        public virtual Task<TEntity> AddAsync(TEntity entity)
        {
            return Task.FromResult(Add(entity));
        }

        public virtual void Add(List<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        public virtual async Task AddAsync(List<TEntity> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        private void Update(TEntity entityToUpdate)
        {
            dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            await Task.Run(() => Update(entity));
        }

        private void Update(List<TEntity> entitiesToUpdate)
        {
            foreach (var entity in entitiesToUpdate)
            {
                Update(entity);
            }
        }

        public virtual async Task UpdateAsync(List<TEntity> entities)
        {
            await Task.Run(() => Update(entities));
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);
            if (entityToDelete != null)
            {
                Delete(entityToDelete);
            }
        }

        private void Delete(TEntity entity)
        {
            if (dbContext.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            return Task.Run(() => Delete(entity));
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> where)
        {
            dbSet.RemoveRange(await dbSet.Where(where).ToListAsync());
        }
    }
}