using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoveMate.Repository.DBContext;

namespace MoveMate.Repository.Repositories.Repository
{
    public class HolidaySettingRepository : GenericRepository<HolidaySetting>, IHolidaySettingRepository
    {
        public HolidaySettingRepository(MoveMateDbContext context) : base(context)
        {
            
        }
        
        public async Task<bool> IsHolidayAsync(DateTime bookingAt)
        {
            var bookingDateOnly = DateOnly.FromDateTime(bookingAt);

            return await _context.Set<HolidaySetting>()
                .AnyAsync(holiday => holiday.Day.HasValue && holiday.Day.Value == bookingDateOnly);
        }
    }
}
