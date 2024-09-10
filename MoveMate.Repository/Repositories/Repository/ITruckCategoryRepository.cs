using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MoveMate.Repository.Repositories.Repository
{
    public class TruckCategoryRepository : GenericRepository<TruckCategory>, ITruckCategoryRepository
    {
        private TruckRentalContext _dbContext;
        public TruckCategoryRepository(TruckRentalContext context) : base(context)
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
    }
}
