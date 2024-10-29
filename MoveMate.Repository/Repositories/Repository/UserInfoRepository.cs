﻿using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.DBContext;

namespace MoveMate.Repository.Repositories.Repository
{
    public class UserInfoRepository : GenericRepository<UserInfo>, IUserInfoRepository
    {
        public UserInfoRepository(MoveMateDbContext context) : base(context)
        {
        }

        public async Task<List<UserInfo>> GetUserInfoByUserIdAsync(int accountId)
        {
            return await _dbSet
                .Include(ui => ui.User)
                .Where(a => a.UserId == accountId)
                .ToListAsync();
        }

    }
}