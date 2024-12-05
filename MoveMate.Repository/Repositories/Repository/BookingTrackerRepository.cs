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
using MoveMate.Domain.Enums;

namespace MoveMate.Repository.Repositories.Repository
{
    public class BookingTrackerRepository : GenericRepository<BookingTracker>, IBookingTrackerRepository
    {
        public BookingTrackerRepository(MoveMateDbContext context) : base(context)
        {
        }
        public async Task<BookingTracker> GetBookingTrackerByBookingIdAsync(int bookingId)
        {
            IQueryable<BookingTracker> query = _dbSet;
            return await query
                            .Where(a => a.BookingId == bookingId)
                .FirstOrDefaultAsync();
        }
        public async Task<BookingTracker> GetBookingTrackerByBookingIdAndStatusAndTypeAsync(int bookingId, string status, string type)
        {
            IQueryable<BookingTracker> query = _dbSet;
            return await query
                            .Where(a => a.BookingId == bookingId && a.Status == status && a.Type == type)
                .FirstOrDefaultAsync();
        }

        public async Task<List<BookingTracker>> GetBookingTrackerByTypeAndBookingIdAsync(string type, int bookingId)
        {
            IQueryable<BookingTracker> query = _dbSet;

            var assignments = await query
                .Where(a => a.BookingId == bookingId && a.Type == TrackerEnums.MONETARY.ToString())
                .ToListAsync(); // Execute and return the list of assignments

            return assignments;
        }
    }
}