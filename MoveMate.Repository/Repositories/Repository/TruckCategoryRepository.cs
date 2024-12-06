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
using MoveMate.Repository.Repositories.Dtos;

namespace MoveMate.Repository.Repositories.Repository
{
    public class TruckCategoryRepository : GenericRepository<TruckCategory>, ITruckCategoryRepository
    {
        private MoveMateDbContext _dbContext;

        public TruckCategoryRepository(MoveMateDbContext context) : base(context)
        {
            this._dbContext = context;
        }

        public async Task<List<string>> GetFirstTruckImagesByCategoryIdAsync(int truckCategoryId)
        {
            // Truy vấn lấy xe tải đầu tiên thuộc TruckCategory đó
            var truck = await _dbContext.Trucks
                .Where(t => t.TruckCategoryId == truckCategoryId)
                .Include(t => t.TruckImgs) // Include các ảnh liên quan
                .FirstOrDefaultAsync();

            // Nếu không có xe tải nào thuộc danh mục này
            if (truck == null)
            {
                return new List<string>();
            }

            // Lấy danh sách các link ảnh của xe tải
            var truckImages = truck.TruckImgs.Select(img => img.ImageUrl).ToList();

            return truckImages;
        }

        public async Task<StatisticTruckCategoryResult> GetTruckCategorySummaryAsync()
        {
            var truckCategories = await _dbContext.TruckCategories
                .Where(tc => tc.IsDeleted != true)
                .Select(tc => new StatisticTruckCategorySummary
                {
                    TruckCategoryId = tc.Id,
                    TruckCategoryName = tc.CategoryName!,
                    TotalTrucks = tc.Trucks.Count,
                    TotalBookings = tc.Trucks
                        .Where(t => t.IsDeleted != true)
                        .SelectMany(t => t.Assignments)
                        .Select(a => a.BookingId)
                        .Count()
                })
                .ToListAsync();

            var result = new StatisticTruckCategoryResult
            {
                TotalTruckCategories = truckCategories.Count,
                TruckCategories = truckCategories
            };

            return result;
        }
    }
}