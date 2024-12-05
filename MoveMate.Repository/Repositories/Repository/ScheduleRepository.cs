using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.DBContext;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Repository.Repositories.Repository
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(MoveMateDbContext context) : base(context)
        {
        }

        public Schedule GetByDate(string date)
        {
            // Chuyển đổi chuỗi date thành kiểu DateOnly với định dạng MM/dd/yyyy
            if (!DateOnly.TryParseExact(date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                throw new ArgumentException("Invalid date format. Expected format: MM/dd/yyyy", nameof(date));
            }

            IQueryable<Schedule> query = _dbSet;
            return query
                .Where(a => a.Date == parsedDate && a.IsDeleted == false)
                .FirstOrDefault();
        }
        public virtual async Task<Schedule?> GetByIdAsync(int id, string includeProperties = "")
        {
            IQueryable<Schedule> query = _dbSet;
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }
            query = query.Where(a => a.Id == id);

            var result = await query.FirstOrDefaultAsync();
            return result;
        }
    }
}
