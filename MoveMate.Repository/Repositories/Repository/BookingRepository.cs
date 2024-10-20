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
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace MoveMate.Repository.Repositories.Repository
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        
        public BookingRepository(MoveMateDbContext context) : base(context)
        {
        }

        public virtual async Task<Booking?> GetByIdAsyncV1(int id, string includeProperties = "")
        {
            IQueryable<Booking> query = _dbSet;
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            // Include BookingTrackers and their related TrackerResources (fix typo here)
            query = query
                .Include(b => b.BookingDetails)
                .Include(b => b.ServiceDetails)
                .Include(b => b.FeeDetails)
                .Include(b => b.BookingTrackers)              
                .ThenInclude(bt => bt.TrackerSources); // Use 'TrackerResources' instead of 'TrackerSources'

            query = query.Where(a => a.Id == id);

            var result = await query.FirstOrDefaultAsync();
            return result;
        }

        public virtual async Task<Booking?> GetByIdAsync(int id, string includeProperties = "")
        {
            IQueryable<Booking> query = _dbSet;

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

        public virtual async Task<Booking?> GetByBookingIdAndUserIdAsync(int bookingId, int userId)
        {
            IQueryable<Booking> query = _dbSet;
            query = query.Where(b => b.Id == bookingId && b.UserId == userId);
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<Booking?> GetAsync(
     Expression<Func<Booking, bool>> filter,
     Func<IQueryable<Booking>, IIncludableQueryable<Booking, object>>? include = null)
        {
            IQueryable<Booking> query = _dbSet;

            // Apply include if provided
            if (include != null)
            {
                query = include(query);
            }

            // Apply the filter expression to the query
            query = query.Where(filter);

            // Execute the query and return the first result or null if no matches
            return await query.AsNoTracking().FirstOrDefaultAsync(); // Note: Using AsNoTracking to avoid unintended tracking
        }


    }
}