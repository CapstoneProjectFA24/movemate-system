using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Google.Api;
using MoveMate.Repository.DBContext;
using MoveMate.Repository.Repositories.Dtos;

namespace MoveMate.Repository.Repositories.Repository
{
    public class PromotionCategoryRepository : GenericRepository<PromotionCategory>, IPromotionCategoryRepository
    {
        private readonly MoveMateDbContext _context;

        public PromotionCategoryRepository(MoveMateDbContext context) : base(context)
        {
            _context = context;
        }

        public virtual async Task<PromotionCategory?> GetByIdAsync(int id, string includeProperties = "")
        {
            IQueryable<PromotionCategory> query = _dbSet;

            // Apply includes
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            // Filter by ID
            query = query.Where(a => a.Id == id);

            // Execute the query and get the result
            var result = await query.FirstOrDefaultAsync();

            return result;
        }

        public async Task<int?> GetPromotionIdByServiceId(int serviceId)
        {
            IQueryable<PromotionCategory> query = _dbSet;

            var promotion = await query
                .Where(p => p.ServiceId == serviceId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            return promotion == 0 ? (int?)null : promotion;
        }

        public async Task<List<PromotionCategory>> GetPromotionByServiceIdAsync(int serviceId)
        {
            IQueryable<PromotionCategory> query = _dbSet;
            return await query
                .Where(p => p.ServiceId == serviceId)
                .ToListAsync();
        }

        public async Task<List<PromotionCategory>> GetPromotionsWithNoUserVoucherAsync(int userId, DateTime currentDate,
            string includeProperties = "")
        {
            IQueryable<PromotionCategory> query = _dbSet;

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            // Filter promotions where vouchers are either null or do not have the specified userId
            query = query.Where(pc =>
                !pc.Vouchers.Any(v => v.UserId == userId) && pc.IsDeleted == false && pc.EndDate >= currentDate);

            return await query.ToListAsync();
        }

        public async Task<List<PromotionCategory>> GetPromotionsWithUserVoucherAsync(int userId, DateTime currentDate,
            string includeProperties = "")
        {
            IQueryable<PromotionCategory> query = _dbSet;

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            // Filter promotions with at least one voucher linked to userId
            query = query.Where(pc =>
                pc.Vouchers.Any(v => v.UserId == userId) && pc.IsDeleted == false && pc.EndDate >= currentDate);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Retrieves detailed statistics for promotions, including active promotions, user participation, and voucher usage.
        /// </summary>
        /// <returns>
        /// A <see cref="PromotionStatisticsResponse"/> containing the following information:
        /// - Total number of promotions.
        /// - Number of active promotions.
        /// - Total value of running promotions.
        /// - Total number of users who have taken vouchers.
        /// - Total number of used vouchers.
        /// - Total value of used vouchers.
        /// - Detailed statistics for each promotion and active promotion.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Loads all promotion categories and their related vouchers.
        /// 2. Filters promotions based on their active status (current date is within the start and end dates).
        /// 3. Aggregates data for total promotions, active promotions, and voucher usage.
        /// 4. Provides detailed statistics for individual promotions and active promotions.
        /// </remarks>
        /// <example>
        /// Example usage:
        /// <code>
        /// var response = await GetPromotionStatisticsAsync();
        /// Console.WriteLine($"Total Promotions: {response.TotalPromotions}");
        /// Console.WriteLine($"Active Promotions: {response.ActivePromotions}");
        /// </code>
        /// </example>
        public async Task<PromotionStatisticsResponse> GetPromotionStatisticsAsync()
        {
            var promotions = await _context.PromotionCategories
                .Include(p => p.Vouchers) // Đảm bảo tải thông tin vouchers
                .ToListAsync();

            var currentDate = DateTime.Now;

            var totalPromotions = promotions.Count;

            var activePromotions =
                promotions.Where(p => p.StartDate <= currentDate && p.EndDate >= currentDate).ToList();
            var totalAmountRunningPromotions = promotions.Where(p => p.Amount.HasValue).Sum(p => p.Amount.Value);

            var totalUsersTakingPromotions = promotions
                .SelectMany(p => p.Vouchers)
                .Where(v => v.IsActived == true && v.UserId.HasValue && v.PromotionCategoryId.HasValue)
                .GroupBy(v => v.UserId)
                .Count();

            var totalUsedPromotions = promotions
                .SelectMany(p => p.Vouchers)
                .Where(v => v.BookingId.HasValue && v.PromotionCategoryId.HasValue)
                .Count();

            var totalAmountUsedPromotions = promotions
                .SelectMany(p => p.Vouchers)
                .Where(v => v.BookingId.HasValue && v.Price.HasValue)
                .Sum(v => v.Price.Value);

            var statisticsPerPromotion = new List<PromotionDetailStatistics>();

            foreach (var promotion in promotions)
            {
                var vouchers = promotion.Vouchers;

                // Thống kê cho từng promotion
                var totalUsersTakingPromotion = vouchers
                    .Where(v => v.IsActived == true && v.UserId.HasValue && v.PromotionCategoryId == promotion.Id)
                    .GroupBy(v => v.UserId)
                    .Count();

                var totalUsedPromotionDetails = vouchers
                    .Where(v => v.BookingId.HasValue && v.PromotionCategoryId == promotion.Id)
                    .Count();

                var totalAmountUsedPromotionDetails = vouchers
                    .Where(v => v.BookingId.HasValue && v.Price.HasValue && v.PromotionCategoryId == promotion.Id)
                    .Sum(v => v.Price.Value);

                var totalAmountRunningPromotion =
                    promotion.Amount.HasValue ? promotion.Amount.Value * vouchers.Count() : 0;

                statisticsPerPromotion.Add(new PromotionDetailStatistics
                {
                    PromotionId = promotion.Id,
                    PromotionName = promotion.Name,
                    Quantity = promotion.Quantity,
                    TotalUsersTakingVouchers = totalUsersTakingPromotion,
                    TotalUsedVouchers = totalUsedPromotionDetails,
                    TotalAmountUsedPromotions = totalAmountUsedPromotionDetails,
                    TotalAmountRunningPromotion = totalAmountRunningPromotion
                });
            }

            var statisticsPerPromotionActive = new List<PromotionDetailStatistics>();

            foreach (var promotion in activePromotions)
            {
                var vouchers = promotion.Vouchers;

                // Thống kê cho từng promotion
                var totalUsersTakingPromotion = vouchers
                    .Where(v => v.IsActived == true && v.UserId.HasValue && v.PromotionCategoryId == promotion.Id)
                    .GroupBy(v => v.UserId)
                    .Count();

                var totalUsedPromotionDetails = vouchers
                    .Where(v => v.BookingId.HasValue && v.PromotionCategoryId == promotion.Id)
                    .Count();

                var totalAmountUsedPromotionDetails = vouchers
                    .Where(v => v.BookingId.HasValue && v.Price.HasValue && v.PromotionCategoryId == promotion.Id)
                    .Sum(v => v.Price.Value);

                var totalAmountRunningPromotion =
                    promotion.Amount.HasValue ? promotion.Amount.Value * vouchers.Count() : 0;

                statisticsPerPromotionActive.Add(new PromotionDetailStatistics
                {
                    PromotionId = promotion.Id,
                    PromotionName = promotion.Name,
                    Quantity = promotion.Quantity,
                    TotalUsersTakingVouchers = totalUsersTakingPromotion,
                    TotalUsedVouchers = totalUsedPromotionDetails,
                    TotalAmountUsedPromotions = totalAmountUsedPromotionDetails,
                    TotalAmountRunningPromotion = totalAmountRunningPromotion
                });
            }

            return new PromotionStatisticsResponse
            {
                TotalPromotions = totalPromotions,
                ActivePromotions = activePromotions.Count(),
                TotalAmountRunningVouchers = totalAmountRunningPromotions,
                TotalUsersTakingVouchers = totalUsersTakingPromotions,
                TotalUsedVouchers = totalUsedPromotions,
                TotalAmountUsedPromotions = totalAmountUsedPromotions,
                PromotionDetails = statisticsPerPromotion,
                PromotionActiveDetails = statisticsPerPromotionActive,
                TotalActiveUsersTakingVouchers = statisticsPerPromotionActive.Sum(p => p.TotalUsersTakingVouchers),
                TotalActiveUsedVouchers = statisticsPerPromotionActive.Sum(p => p.TotalUsedVouchers),
                TotalActiveAmountRunningVouchers = statisticsPerPromotionActive.Sum(p => p.TotalAmountRunningPromotion),
                TotalActiveAmountUsedPromotions = statisticsPerPromotionActive.Sum(p => p.TotalAmountUsedPromotions)
            };
        }
    }
}