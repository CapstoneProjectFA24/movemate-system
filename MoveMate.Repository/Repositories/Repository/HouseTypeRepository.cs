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
namespace MoveMate.Repository.Repositories.Repository
{
    public class HouseTypeRepository : GenericRepository<HouseType>, IHouseTypeRepository
    {
        public HouseTypeRepository(MoveMateDbContext context) : base(context)
        {
        }

        public virtual async Task<HouseType?> GetByIdAsyncV1(int id, string includeProperties = "")
        {
            IQueryable<HouseType> query = _dbSet;

            // Apply includes
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            // Filter by ID
            query = query.Where(a => a.Id == id);

            // Execute the query and get the result
            var result = await query.FirstOrDefaultAsync();

            return result;
        }
    }
}
