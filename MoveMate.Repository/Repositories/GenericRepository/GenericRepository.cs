using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.Models;

//using ShopRepository.Models;
using System.Linq.Expressions;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Domain.DBContext;
using Microsoft.EntityFrameworkCore.Metadata;


namespace MoveMate.Repository.Repositories.GenericRepository
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected DbSet<TEntity> _dbSet;
        protected readonly DbContext _context;

        public GenericRepository(MoveMateDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual Task<List<TEntity>> GetAllAsync() => _dbSet.ToListAsync();

        public virtual async Task<TEntity> GetByIdAsync(int id,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            //var result = await _dbSet.FindAsync(id);
            //// todo should throw exception when not found
            //if (result == null)
            //    throw new Exception($"Not Found by ID: [{id}] of [{typeof(TEntity).Name}]");
            //return result;

            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateEntityAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public virtual void SoftRemove(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual async Task<List<TEntity>> SaveOrUpdateRangeAsync(List<TEntity> entities)
        {
            var primaryKeyProperties = _context.Model.FindEntityType(typeof(TEntity))
                .FindPrimaryKey()
                .Properties;

            foreach (var entity in entities)
            {
                // Lấy giá trị của khóa chính từ thực thể
                var key = GetPrimaryKeyValue(entity, primaryKeyProperties);

                // Kiểm tra xem thực thể đã tồn tại chưa
                var entityInDb = await _dbSet.FindAsync(key);

                if (entityInDb != null)
                {
                    // Nếu đã tồn tại, cập nhật giá trị mới
                    _context.Entry(entityInDb).CurrentValues.SetValues(entity);
                    _dbSet.Update(entityInDb);
                }
                else
                {
                    // Nếu chưa tồn tại, thêm mới
                    await _dbSet.AddAsync(entity);
                }
            }

            return entities;
        }

        // Hàm phụ trợ để lấy giá trị khóa chính
        private object GetPrimaryKeyValue(TEntity entity, IEnumerable<IProperty> primaryKeyProperties)
        {
            var keyValues = primaryKeyProperties
                .Select(p => entity.GetType().GetProperty(p.Name)?.GetValue(entity, null))
                .ToArray();

            return keyValues.Length == 1 ? keyValues[0] : keyValues; // Nếu có một khóa chính, trả về nó, nếu nhiều khóa chính, trả về mảng
        }

        public virtual async Task<TEntity> SaveOrUpdateAsync(TEntity entity)
        {
            // Kiểm tra xem thực thể đã tồn tại chưa, dựa vào khóa chính (primary key)
            var entityInDb = await _dbSet.FindAsync(GetPrimaryKeyValue(entity));

            if (entityInDb != null)
            {
                // Nếu tồn tại, cập nhật thực thể
                _context.Entry(entityInDb).CurrentValues.SetValues(entity);
                _dbSet.Update(entityInDb);
            }
            else
            {
                // Nếu không tồn tại, thêm mới thực thể
                await _dbSet.AddAsync(entity);
            }

            return entity;
        }

        // Hàm phụ trợ để lấy giá trị khóa chính của thực thể
        private object GetPrimaryKeyValue(TEntity entity)
        {
            var key = _context.Model.FindEntityType(typeof(TEntity))
                                    .FindPrimaryKey()
                                    .Properties
                                    .Select(p => entity.GetType().GetProperty(p.Name).GetValue(entity, null))
                                    .SingleOrDefault();

            return key;
        }


        public virtual async Task AddRangeAsync(List<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void SoftRemoveRange(List<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public TEntity Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
            return entity;
        }

        public void RemoveRange(List<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }


        public virtual void UpdateRange(List<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public IQueryable<TEntity> FindAll(params Expression<Func<TEntity, object>>[]? includeProperties)
        {
            IQueryable<TEntity> items = _dbSet.AsNoTracking();
            if (includeProperties != null)
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }

            return items;
        }


        public IQueryable<TEntity> FilterAll(
            bool? isAscending,
            string? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            string[]? includeProperties = null,
            int pageIndex = 0,
            int pageSize = 10)
        {
            IQueryable<TEntity> items = _dbSet.AsNoTracking();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }

            if (predicate != null)
            {
                items = items.Where(predicate);
            }

            if (!string.IsNullOrEmpty(orderBy)) // Check if orderBy is provided
            {
                items = ApplyOrder(items, orderBy, isAscending ?? true);
            }

            return items.Skip(pageIndex * pageSize).Take(pageSize);
        }

        public IQueryable<TEntity> GetAllWithoutPaging(
            bool? isAscending,
            string? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            string[]? includeProperties = null)
        {
            IQueryable<TEntity> items = _dbSet.AsNoTracking();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }

            if (predicate != null)
            {
                items = items.Where(predicate);
            }

            if (!string.IsNullOrEmpty(orderBy)) // Check if orderBy is provided
            {
                items = ApplyOrder(items, orderBy, isAscending ?? true);
            }

            return items;
        }

        public IQueryable<TEntity> FilterByExpression(Expression<Func<TEntity, bool>> predicate,
            string[]? includeProperties = null)
        {
            IQueryable<TEntity> items = _dbSet.AsNoTracking();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }

            return items.Where(predicate);
        }

        private IQueryable<TEntity> ApplyOrder(IQueryable<TEntity> source, string orderBy, bool isAscending)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, orderBy);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = isAscending ? "OrderBy" : "OrderByDescending";
            var orderByExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(TEntity), property.Type },
                source.Expression,
                lambda
            );
            return source.Provider.CreateQuery<TEntity>(orderByExpression);
        }

        public async Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate,
            params Expression<Func<TEntity, object>>[]? includeProperties)
        {
            return await FindAll(includeProperties).SingleOrDefaultAsync(predicate);
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                         (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Implementing pagination
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                // Ensure the pageIndex and pageSize are valid
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize =
                    pageSize.Value > 0
                        ? pageSize.Value
                        : 10; // Assuming a default pageSize of 10 if an invalid value is passed
                if (pageSize.Value > 0)
                {
                    query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
                }
            }

            return query.ToList();
        }
        
        public virtual (IEnumerable<TEntity> Data, int Count) GetWithCount(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Get the total count before pagination
            int count = query.Count();

            // Implementing pagination
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Default pageSize of 10 if invalid
                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            // Returning data with count
            return (Data: query.ToList(), Count: count);
        }

        public virtual PagedResult<TEntity> GetWithPagination(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = _dbSet;

            var pagin = new Pagination();
            pagin.totalItemsCount = query.Count();
            pagin.pageSize = pageSize ?? -1;
            pagin.pageIndex = pageIndex ?? 0;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                         (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Implementing pagination
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                // Ensure the pageIndex and pageSize are valid
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize =
                    pageSize.Value > 0
                        ? pageSize.Value
                        : 10; // Assuming a default pageSize of 10 if an invalid value is passed
                if (pageSize.Value > 0)
                {
                    query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
                }
            }

            return new PagedResult<TEntity>
            {
                Results = query.ToList(),
                Pagination = pagin
            };
        }

        /*public async Task<IReadOnlyList<T>> ListAsync(ISpecifications<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }
        public async Task<int> CountAsync(ISpecifications<T> specifications)
        {
            return await ApplySpecification(specifications).CountAsync();
        }
        private IQueryable<T> ApplySpecification(ISpecifications<T> specifications)
        {
            return SpecificationEvaluatOr<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), specifications);
        }*/
    }
}