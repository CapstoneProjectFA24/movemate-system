using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.DBContext;

namespace MoveMate.Repository.Repositories.Repository
{
    public class ScheduleDetailRepository : GenericRepository<ScheduleDetail>, IScheduleDetailRepository
    {
        public ScheduleDetailRepository(MoveMateDbContext context) : base(context)
        {
        }

        public List<ScheduleDetail> GetScheduleDetailsByDate(DateTime specificDate)
        {
            IQueryable<ScheduleDetail> query = _dbSet;
            var scheduleDetails = query
                .Where(sd => sd.WorkingDays.HasValue && sd.WorkingDays.Value.Date == specificDate.Date)
                .OrderBy(sd => sd.EndDate) 
                .ToList();

            return scheduleDetails;
        }

    }
}