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
    public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
    {
        private readonly MoveMateDbContext _context;
        public VoucherRepository(MoveMateDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<List<Voucher>> GetAvailableVouchersByPromotionId(int promotionId)
        {
            return await _context.Vouchers
                .Where(v => v.PromotionCategoryId == promotionId && !v.UserId.HasValue)
                .OrderByDescending(v => v.Price) 
                .ToListAsync();
        }
        public async Task<bool> UserHasVoucherForPromotionAsync(int promotionId, int userId)
        {
            return await _context.Vouchers
                .AnyAsync(v => v.PromotionCategoryId == promotionId && v.UserId == userId);
        }

    }
}