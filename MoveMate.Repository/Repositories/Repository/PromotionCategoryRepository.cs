using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.DBContext;
using Microsoft.EntityFrameworkCore;
using Google.Api;

namespace MoveMate.Repository.Repositories.Repository
{
    public class PromotionCategoryRepository : GenericRepository<PromotionCategory>, IPromotionCategoryRepository
    {
        private readonly MoveMateDbContext _context;
        public PromotionCategoryRepository(MoveMateDbContext context) : base(context)
        {
            _context = context;
        }

        public virtual async Task<PromotionCategory?> GetByIdAsync(int id, string includeProperties = "")
        {
            IQueryable<PromotionCategory> query = _dbSet;

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

        public async Task<int?> GetPromotionIdByServiceId(int serviceId)
        {
            var promotion = await _context.PromotionCategories
                .Where(p => p.ServiceId == serviceId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            return promotion == 0 ? (int?)null : promotion; 
        }

        public async Task<List<PromotionCategory>> GetPromotionByServiceIdAsync(int serviceId)
        {
            return await _context.PromotionCategories
                .Where(p => p.ServiceId == serviceId)
                .ToListAsync();
        }


    }
}