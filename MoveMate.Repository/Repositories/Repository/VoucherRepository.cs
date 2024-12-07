using MoveMate.Domain.Models;
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
    public class VoucherRepository : GenericRepository<Voucher>, IVoucherRepository
    {
        private readonly MoveMateDbContext _context;
        public VoucherRepository(MoveMateDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<List<Voucher>> GetAvailableVouchersByPromotionId(int promotionId)
        {
            IQueryable<Voucher> query = _dbSet;
            return await query
            .Where(v => v.PromotionCategoryId == promotionId && !v.UserId.HasValue)
                .OrderByDescending(v => v.Price) 
                .ToListAsync();
        }
        public async Task<Voucher> GetVoucherWithHighestPriceByPromotionIdAsync(int promotionId)
        {
            IQueryable<Voucher> query = _dbSet.AsNoTracking();

            return await query
                .Where(v => v.PromotionCategoryId == promotionId)
                .OrderByDescending(v => v.Price)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Voucher>> GetUserVouchersAsync(int userId)
        {
            IQueryable<Voucher> query = _dbSet;
            return await query
                .Include(v => v.PromotionCategory) // Include the PromotionCategory to access its properties
                .Where(v => v.UserId == userId &&
                     v.IsActived == true &&
                     v.PromotionCategory != null && 
                     v.PromotionCategory.IsPublic == true && 
                     v.PromotionCategory.StartDate <= DateTime.Now && 
                     v.PromotionCategory.EndDate >= DateTime.Now) 
                    .ToListAsync();
        }


        public async Task<List<Voucher>> GetUserVouchersByBookingIdAsync(int userId, int bookingId)
        {
            IQueryable<Voucher> query = _dbSet;
            return await query
                .Include(v => v.PromotionCategory) // Include the PromotionCategory to access its properties
                .Where(v => v.UserId == userId &&
                     v.BookingId == bookingId &&
                     v.IsActived == false &&
                     v.PromotionCategory != null &&
                     v.PromotionCategory.IsPublic == true &&
                     v.PromotionCategory.StartDate <= DateTime.Now &&
                     v.PromotionCategory.EndDate >= DateTime.Now)
                    .ToListAsync();
        }
        public async Task<bool> HasDuplicatePromotionCategoryIdsAsync(List<Voucher> vouchers)
        {
            var duplicateIds = vouchers
                .GroupBy(v => v.PromotionCategoryId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            return duplicateIds.Any();
        }


        public async Task<bool> UserHasVoucherForPromotionAsync(int promotionId, int userId)
        {
            IQueryable<Voucher> query = _dbSet;
            return await query
                .AnyAsync(v => v.PromotionCategoryId == promotionId && v.UserId == userId);
        }

    }
}