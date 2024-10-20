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
    public class BookingTrackerRepository : GenericRepository<BookingTracker>, IBookingTrackerRepository
    {
        public BookingTrackerRepository(MoveMateDbContext context) : base(context)
        {
        }
        public async Task<BookingTracker> GetBookingTrackerByBookingIdAsync(int bookingId)
        {
            return await _dbSet
                .Where(a => a.BookingId == bookingId)
                .FirstOrDefaultAsync();
        }
    }
}