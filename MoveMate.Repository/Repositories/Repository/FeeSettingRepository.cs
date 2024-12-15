using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.Enums;
using MoveMate.Repository.DBContext;

namespace MoveMate.Repository.Repositories.Repository
{
    public class FeeSettingRepository : GenericRepository<FeeSetting>, IFeeSettingRepository
    {
        public FeeSettingRepository(MoveMateDbContext context) : base(context)
        {
        }

        public async Task<List<FeeSetting>> GetCommonFeeSettingsAsync()
        {
            return await _context.Set<FeeSetting>()
                .Where(f => f.Type == TypeFeeEnums.COMMON.ToString() && f.IsActived == true)
                .ToListAsync();
        }

        public async Task<List<FeeSetting>> GetWeekendFeeSettingsAsync()
        {
            return await _context.Set<FeeSetting>()
                .Where(f => f.Type == TypeFeeEnums.WEEKEND.ToString() && f.IsActived == true)
                .ToListAsync();
        }

        public async Task<List<FeeSetting>> GetOBHFeeSettingsAsync()
        {

            string outside = TypeFeeEnums.OUTSIDE_BUSINESS_HOURS.ToString();
            return await _context.Set<FeeSetting>()
                .Where(f => f.Type == TypeFeeEnums.OUTSIDE_BUSINESS_HOURS.ToString() && f.IsActived == true)
                .ToListAsync();
        }

        public async Task<List<FeeSetting>> GetPercentSystemFeeSettingsAsync()
        {
            return await _context.Set<FeeSetting>()
                .Where(f => f.Unit == UnitEnums.PERCENT.ToString() && f.IsActived == true && f.Type == TypeFeeEnums.SYSTEM.ToString())
                .ToListAsync();
        }

        public async Task<List<FeeSetting>> GetPercentHolidayFeeSettingsAsync()
        {
            return await _context.Set<FeeSetting>()
                .Where(f => f.Unit == UnitEnums.PERCENT.ToString() && f.IsActived == true && f.Type == TypeFeeEnums.HOLIDAY.ToString())
                .ToListAsync();
        }

        public async Task<FeeSetting?> GetReviewerFeeSettingsAsync()
        {
            return await _context.Set<FeeSetting>()
                .Where(f => f.Type == TypeFeeEnums.REVIEWER.ToString() && f.IsActived == true)
                .FirstOrDefaultAsync();
        }
        public async Task<List<FeeSetting>> GetByServiceIdAsync(int serviceId)
        {
            IQueryable<FeeSetting> query = _dbSet;
            var assignments = await query
                    .Where(a => a.ServiceId == serviceId && a.IsActived == true)
                    .ToListAsync(); // Execute and return the list of assignments

            return assignments;
        }

        //public List<FeeSetting> GetTruckFeeSettings(int cateTruckId)
        //{
        //    return _context.Set<FeeSetting>()
        //        .Where(f => f.Type == TypeServiceEnums.TRUCK.ToString() && f.IsActived == true &&
        //                    f.TruckCategoryId == cateTruckId && f.ServiceId == null)
        //        .ToList();
        //}
    }
}