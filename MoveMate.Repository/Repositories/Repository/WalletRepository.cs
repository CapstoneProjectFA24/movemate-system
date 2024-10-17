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
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(MoveMateDbContext context) : base(context)
        {
        }

        public async Task<Wallet> GetWalletByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Where(a => a.UserId == accountId)
                .FirstOrDefaultAsync();
        }

        public void Detach(Wallet entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
    }
}