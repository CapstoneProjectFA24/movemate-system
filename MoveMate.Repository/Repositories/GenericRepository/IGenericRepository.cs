using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Repository.Repositories.UnitOfWork;

namespace MoveMate.Repository.Repositories.GenericRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> AddAsync(TEntity entity);

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null);

        PagedResult<TEntity> GetWithPagination(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null);

        Task<TEntity> UpdateAsync(TEntity entity);
        Task UpdateEntityAsync(TEntity entity);
        void Update(TEntity entity);
        void UpdateRange(List<TEntity> entities);
        void SoftRemove(TEntity entity);
        Task AddRangeAsync(List<TEntity> entities);
        void SoftRemoveRange(List<TEntity> entities);
        TEntity Remove(TEntity entity);

        public IQueryable<TEntity> FilterAll(bool? isAscending, string? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null, string[]? includeProperties = null, int pageIndex = 0,
            int pageSize = 10);

        public IQueryable<TEntity> GetAllWithoutPaging(bool? isAscending, string? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null, string[]? includeProperties = null);

        IQueryable<TEntity> FilterByExpression(Expression<Func<TEntity, bool>> predicate,
            string[]? includeProperties = null);

        Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate,
            params Expression<Func<TEntity, object>>[]? includeProperties);
    }
}