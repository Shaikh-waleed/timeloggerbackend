using AutoMapper;
using TimeLogger.Business.IService;
using TimeLogger.Data.IRepository;
using TimeLogger.Infrastructure.Utility;
using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Business.Service
{
    public class BaseService<TBusinessModel, TEntity, TKey> : IBaseService<TBusinessModel, TEntity, TKey>
        where TBusinessModel : class 
        where TEntity : class
    {
        protected readonly IBaseRepository<TEntity, TKey> repository;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IMapper mapper;
        public BaseService(IMapper mapper, IBaseRepository<TEntity, TKey> baseRepository, IUnitOfWork unitOfWork)
        {
            this.repository = baseRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public virtual async Task<bool> Any(Expression<Func<TEntity, bool>> where = null)
        {
            IQueryable<TEntity> query = repository.Get();
            if (Check.NotNull(where))
                query = query.Where(where);
            return query.Any();
        }
        public virtual async Task<TBusinessModel> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var dataEntity = await repository.FirstOrDefaultAsync(where, orderBy, includeProperties);
            return mapper.Map<TEntity, TBusinessModel>(dataEntity);
        }

        public virtual async Task<List<TBusinessModel>> Get()
        {
            var dataEntities = await repository.GetAsync();
            return mapper.Map<List<TEntity>, List<TBusinessModel>>(dataEntities);
        }
        public virtual async Task<TBusinessModel> Get(TKey id)
        {
            var dataEntity = await repository.GetAsync(id);
            return mapper.Map<TEntity, TBusinessModel>(dataEntity);
        }
        public async virtual Task<List<TBusinessModel>> Get(Expression<Func<TEntity, bool>> where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var _entities = await repository.GetAsync(where, orderBy, includeProperties);
            return mapper.Map<List<TEntity>, List<TBusinessModel>>(_entities);
        }
        public virtual List<TBusinessModel> Get(out int count, Expression<Func<TEntity, bool>> where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var _entities = repository.GetAsync(where, orderBy, includeProperties).GetAwaiter().GetResult();
            count = _entities.Count();
            return mapper.Map<List<TEntity>, List<TBusinessModel>>(_entities);
        }
        public virtual List<TBusinessModel> Get(out int count, int pageNumber = 1, int pageSize = 20, Expression<Func<TEntity, bool>> where = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = repository.GetAsync(where, orderBy, includeProperties).GetAwaiter().GetResult().AsQueryable();
            count = query.Count();
            return mapper.Map<List<TEntity>, List<TBusinessModel>>(query.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList());
        }
        public async virtual Task<List<TBusinessModel>> Search(Expression<Func<TEntity, bool>> searchExprn)
        {
            var query = repository.Get();
            if (Check.NotNull(searchExprn))
            {
                query = query.Where(searchExprn);
            }
            return mapper.Map<List<TEntity>, List<TBusinessModel>>(query.ToList());
        }
        public virtual async Task<TBusinessModel> Add(TBusinessModel businessEntity)
        {
            var dataEntity = mapper.Map<TBusinessModel, TEntity>(businessEntity);
            dataEntity = await repository.AddAsync(dataEntity);
            await SaveChanges();
            businessEntity = mapper.Map<TEntity, TBusinessModel>(dataEntity);
            return businessEntity;
        }
        public virtual async Task<List<TBusinessModel>> AddRange(List<TBusinessModel> businessEntities)
        {
            var dataEntities = mapper.Map<List<TBusinessModel>, List<TEntity>>(businessEntities);
            await repository.AddAsync(dataEntities);
            await SaveChanges();
            businessEntities = mapper.Map<List<TEntity>, List<TBusinessModel>>(dataEntities);
            return businessEntities;
        }
        public virtual async Task Update(TBusinessModel businessEntity)
        {
            var dataEntity = mapper.Map<TBusinessModel, TEntity>(businessEntity);
            await repository.UpdateAsync(dataEntity);
            await SaveChanges();
        }

        public virtual async Task Delete(TBusinessModel businessEntity)
        {
            var dataEntity = mapper.Map<TBusinessModel, TEntity>(businessEntity);
            await repository.DeleteAsync(dataEntity);
            await SaveChanges();
        }
        public virtual async Task Delete(TKey id)
        {
            await repository.DeleteAsync(id);
            await SaveChanges();
        }
        public virtual async Task DeleteRange(Expression<Func<TEntity, bool>> deleteExpression)
        {
            await repository.DeleteAsync(deleteExpression);
            await SaveChanges();
        }

        public async Task SaveChanges()
        {
            if (Check.NotNull(unitOfWork))
            {
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}