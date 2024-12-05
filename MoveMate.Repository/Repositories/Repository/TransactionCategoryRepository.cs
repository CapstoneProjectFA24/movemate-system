using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.DBContext;
using MoveMate.Domain.Enums;
using MoveMate.Repository.Repositories.Dtos;
using Transaction = MoveMate.Domain.Models.Transaction;

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
        
        public async Task<CalculateStatisticTransactionDto> CalculateStatisticTransactionsAsync(string shardPrefix)
        {
            var transactions = await _dbContext.Transactions
                .Where(t => t.Shard.StartsWith(shardPrefix) && t.IsDeleted == false && t.Status == PaymentEnum.SUCCESS.ToString())
                .ToListAsync();

            var totalIncome = transactions
                .Where(t => t.WalletId == 4 && t.IsCredit == true)
                .Sum(t => t.Amount ?? 0);

            var totalCompensation = transactions
                .Where(t => t.WalletId == 4 && t.IsCredit == false)
                .Sum(t => t.Amount ?? 0);
            
            return new CalculateStatisticTransactionDto
            {
                Shard = shardPrefix,
                TotalIncome = totalIncome,
                TotalCompensation = totalCompensation
            };
        }

    }
}