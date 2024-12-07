using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using MoveMate.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoveMate.Repository.DBContext;

namespace MoveMate.Repository.Repositories.Repository
{
    public class TruckRepository : GenericRepository<Truck>, ITruckRepository
    {
        public TruckRepository(MoveMateDbContext context) : base(context)
        {
        }

        public virtual async Task<Truck?> GetByIdAsync(int id, string includeProperties = "")
        {
            IQueryable<Truck> query = _dbSet;

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

        public async Task<Truck?> FindByUserIdAsync(int userId)
        {
            IQueryable<Truck?> query = _dbSet;
            
            return await query
                .FirstOrDefaultAsync(s => s!.UserId == userId);
        }
    }
}