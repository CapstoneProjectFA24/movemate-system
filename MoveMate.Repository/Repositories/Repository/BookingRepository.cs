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
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(MoveMateDbContext context) : base(context)
        {
        }

        public virtual async Task<Booking?> GetByIdAsyncV1(int id, string includeProperties = "")
        {
            IQueryable<Booking> query = _dbSet;

            // Include BookingTrackers and their related TrackerResources (fix typo here)
            query = query.Include(b => b.BookingTrackers)
                         .ThenInclude(bt => bt.TrackerSources); // Use 'TrackerResources' instead of 'TrackerSources'

            query = query.Where(a => a.Id == id);

            var result = await query.FirstOrDefaultAsync();
            return result;
        }

    }
}
