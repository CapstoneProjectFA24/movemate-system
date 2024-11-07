﻿using MoveMate.Domain.Models;
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

        public async Task<List<FeeSetting>> GetPercentFeeSettingsAsync()
        {
            return await _context.Set<FeeSetting>()
                .Where(f => f.Unit == UnitEnums.PERCENT.ToString() && f.IsActived == true)
                .ToListAsync();
        }
        
        public async Task<FeeSetting?> GetReviewerFeeSettingsAsync()
        {
            return await _context.Set<FeeSetting>()
                .Where(f => f.Type == TypeFeeEnums.REVIEWER.ToString() && f.IsActived == true)
                .FirstOrDefaultAsync();
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