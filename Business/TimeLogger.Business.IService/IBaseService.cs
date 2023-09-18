using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Business.IService
{
    public interface IBaseService<TBusinessModel, TEntity, TKey>
    {
        Task<bool> Any(Expression<Func<TEntity, bool>> where = null);
        Task<TBusinessModel> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<List<TBusinessModel>> Get();
        Task<TBusinessModel> Get(TKey id);
        Task<List<TBusinessModel>> Get(Expression<Func<TEntity, bool>> where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties);
        List<TBusinessModel> Get(out int count, Expression<Func<TEntity, bool>> where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includePropertie);
        List<TBusinessModel> Get(out int count, int pageNamber = 1, int pageSize = 20, Expression<Func<TEntity, bool>> where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<List<TBusinessModel>> Search(Expression<Func<TEntity, bool>> searchExprn);
        Task<TBusinessModel> Add(TBusinessModel entity);
        Task<List<TBusinessModel>> AddRange(List<TBusinessModel> businessEntities);
        Task Update(TBusinessModel entity);
        Task Delete(TBusinessModel entity);
        Task Delete(TKey id);
        Task DeleteRange(Expression<Func<TEntity, bool>> deleteExpression);
    }
}
