﻿using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.DBContext;
using Microsoft.EntityFrameworkCore;

namespace MoveMate.Repository.Repositories.Repository
{
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        private readonly MoveMateDbContext _context;

        public ServiceRepository(MoveMateDbContext context) : base(context)
        {
            _context = context;
        }

        public virtual async Task<Service?> GetByIdAsyncV1(int id, string includeProperties = "")
        {
            IQueryable<Service> query = _dbSet;

            // Apply includes
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            // Filter by ID
            query = query.Where(a => a.Id == id);

            // Execute the query and get the result
            var result = await query.FirstOrDefaultAsync();

            return result;
        }

        public virtual IEnumerable<Service> GetAll(
            Expression<Func<Service, bool>> filter = null,
            Func<IQueryable<Service>, IOrderedQueryable<Service>> orderBy = null,
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<Service> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = query.Include(s => s.InverseParentService)
                .ThenInclude(ts => ts.TruckCategory);

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
    }
}