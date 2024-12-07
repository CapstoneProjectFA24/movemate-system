using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Repository.DBContext;

namespace MoveMate.Repository.Repositories.Repository
{
    public class ScheduleBookingRepository : GenericRepository<ScheduleBooking>, IScheduleBookingRepository
    {
        public ScheduleBookingRepository(MoveMateDbContext context) : base(context)
        {
        }

        public virtual async Task<ScheduleBooking?> GetByIdAsyncV1(int id, string includeProperties = "")
        {
            IQueryable<ScheduleBooking> query = _dbSet;

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
        public virtual async Task<ScheduleBooking?> GetByIdAsync(int id, string includeProperties = "")
        {
            IQueryable<ScheduleBooking> query = _dbSet;

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
        public virtual async Task<ScheduleBooking?> GetByShard(string shard)
        {
            IQueryable<ScheduleBooking> query = _dbSet;
            query = query.Where(scheduleBooking => scheduleBooking.Shard == shard);
            return await query.FirstOrDefaultAsync();
        }
    }
}