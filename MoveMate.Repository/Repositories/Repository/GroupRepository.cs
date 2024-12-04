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
using MoveMate.Domain.Enums;

namespace MoveMate.Repository.Repositories.Repository
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        public GroupRepository(MoveMateDbContext context) : base(context)
        {
        }

        //public virtual async Task<List<Group>> GetStaffActiveNowBookingStaffDailies(int roleId)
        //{
        //    IQueryable<Group> query = _dbSet;

        //    var activeBookingStaffDailies = await query
        //        .Where(bsd => bsd.IsActived == true && bsd.User != null && bsd.User.RoleId == roleId && bsd.Status == GroupEnums.NOW.ToString())
        //        .ToListAsync();

        //    return activeBookingStaffDailies;
        //}

        //public virtual async Task<List<Group>> GetBookingStaffDailiesNow(int roleId)
        //{
        //    IQueryable<Group> query = _dbSet;

        //    var activeBookingStaffDailies = await query
        //        .Where(bsd => bsd.Status == GroupEnums.NOW.ToString() && bsd.User != null && bsd.User.RoleId == roleId)
        //        .ToListAsync();

        //    return activeBookingStaffDailies;
        //}


        public virtual async Task<Group?> GetByIdAsyncV1(int id, string includeProperties = "")
        {
            IQueryable<Group> query = _dbSet;
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            // Include BookingTrackers and their related TrackerResources (fix typo here)
            //query = query
            //    .Include(b => b.BookingDetails)
            //    .Include(b => b.Assignments)
            //    .Include(b => b.FeeDetails)
            //    .Include(b => b.BookingTrackers)              
            //    .ThenInclude(bt => bt.TrackerSources); // Use 'TrackerResources' instead of 'TrackerSources'

            query = query.Where(a => a.Id == id);

            var result = await query.FirstOrDefaultAsync();
            return result;
        }


    }
}