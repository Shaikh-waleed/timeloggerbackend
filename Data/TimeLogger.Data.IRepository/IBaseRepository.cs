using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Data.IRepository
{
    public interface IBaseRepository<TEntity, TKey>
    {
        /// <summary>
        /// To get single or null entity with childs and sort filter
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Get();

        /// <summary>
        /// Asynchronously get by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(TKey id);

        /// <summary>
        /// To get entity with childs and sort filter
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> where = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// To add entity
        /// </summary>
        /// <param name="entity"></param>
        Task<TEntity> AddAsync(TEntity entity);

        /// <summary>
        /// To add list of items
        /// </summary>
        /// <param name="entity"></param>
        Task AddAsync(List<TEntity> entities);

        /// <summary>
        /// To update entity
        /// </summary>
        /// <param name="entity">
        /// </param>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Update the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        Task UpdateAsync(List<TEntity> entities);

        /// <summary>
        ///Asynchronously delete by Id
        /// </summary>
        /// <param>
        ///     <name>id</name>
        /// </param>
        Task DeleteAsync(TKey id);

        /// <summary>
        /// Delete entity
        /// </summary>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
