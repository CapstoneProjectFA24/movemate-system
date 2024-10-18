using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.DBContext;
using MoveMate.Domain.Enums;

namespace MoveMate.Repository.Repositories.Repository
{
    public class BookingStaffDailyRepository : GenericRepository<BookingStaffDaily>, IBookingStaffDailyRepository
    {
        public BookingStaffDailyRepository(MoveMateDbContext context) : base(context)
        {
        }
        
        public virtual async Task<List<BookingStaffDaily>> GetStaffActiveNowBookingStaffDailies(int roleId)
        {
            IQueryable<BookingStaffDaily> query = _dbSet;

            var activeBookingStaffDailies = await query
                .Where(bsd => bsd.IsActived == true && bsd.User != null && bsd.User.RoleId == roleId && bsd.Status == BookingStaffDailyEnums.NOW.ToString())
                .ToListAsync();

            return activeBookingStaffDailies;
        }
        
        public virtual async Task<List<BookingStaffDaily>> GetBookingStaffDailiesNow(int roleId)
        {
            IQueryable<BookingStaffDaily> query = _dbSet;

            var activeBookingStaffDailies = await query
                .Where(bsd => bsd.Status == BookingStaffDailyEnums.NOW.ToString() && bsd.User != null && bsd.User.RoleId == roleId)
                .ToListAsync();

            return activeBookingStaffDailies;
        }
    }
}