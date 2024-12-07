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
    public class UserInfoRepository : GenericRepository<UserInfo>, IUserInfoRepository
    {
        public UserInfoRepository(MoveMateDbContext context) : base(context)
        {
        }

        public async Task<UserInfo?> GetUserInfoByUserIdAndTypeAsync(int userId, string type)
        {
            IQueryable<UserInfo> query = _dbSet;
            return await query
                .Include(ui => ui.User)
                .FirstOrDefaultAsync(ui => ui.UserId == userId && ui.Type == type);
        }



    }
}