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

namespace MoveMate.Repository.Repositories.Repository
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private MoveMateDbContext _dbContext;

        public TransactionRepository(MoveMateDbContext context) : base(context)
        {
            this._dbContext = context;
        }

        public async Task<Transaction> GetByTransactionCodeAsync(string transactionCode)
        {
            IQueryable<Transaction> query = _dbSet;
            return await query.FirstOrDefaultAsync(t => t.TransactionCode == transactionCode);
        }
    }
}