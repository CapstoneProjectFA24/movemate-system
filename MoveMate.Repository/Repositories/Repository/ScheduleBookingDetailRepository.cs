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
    public class ScheduleBookingDetailRepository : GenericRepository<ScheduleBookingDetail>, IScheduleBookingDetailRepository
    {
        public ScheduleBookingDetailRepository(MoveMateDbContext context) : base(context)
        {
        }

        //public List<ScheduleBookingDetail> GetScheduleDetailsByDate(DateTime specificDate)
        //{
        //    IQueryable<ScheduleBookingDetail> query = _dbSet;
        //    var scheduleDetails = query
        //        .Where(sd => sd.WorkingDays.HasValue && sd.WorkingDays.Value.Date == specificDate.Date)
        //        .OrderBy(sd => sd.EndDate) 
        //        .ToList();

        //    return scheduleDetails;
        //}

    }
}