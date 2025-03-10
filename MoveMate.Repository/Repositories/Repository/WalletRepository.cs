﻿using MoveMate.Domain.Models;
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
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(MoveMateDbContext context) : base(context)
        {
        }

        public async Task<Wallet> GetWalletByAccountIdAsync(int accountId)
        {
            IQueryable<Wallet> query = _dbSet;
            return await query
                .Where(a => a.UserId == accountId)
                .FirstOrDefaultAsync();
        }

        public void Detach(Wallet entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
    }
}