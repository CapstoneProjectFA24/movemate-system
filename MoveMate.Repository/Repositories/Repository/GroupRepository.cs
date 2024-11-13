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
    }
}