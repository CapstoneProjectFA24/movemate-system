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
    public class FeeSettingRepository : GenericRepository<FeeSetting>, IFeeSettingRepository
    {
        public FeeSettingRepository(MoveMateDbContext context) : base(context)
        {
        }
        
        public async Task<List<FeeSetting>> GetCommonFeeSettingsAsync()
        {
            return await _context.Set<FeeSetting>()
                .Where(f => f.Type == TypeServiceEnums.COMMON.ToString() && f.IsActived == true)
                .ToListAsync();
        }
    }
}
