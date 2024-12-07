using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.Enums;
using MoveMate.Repository.DBContext;
using MoveMate.Repository.Repositories.Dtos;

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

        /// <summary>
        /// Retrieves statistics of users grouped by their roles within each group.
        /// </summary>
        /// <remarks>
        /// This method returns the total number of groups and for each group, it provides the count of users 
        /// and the breakdown of users by their roles (e.g., Admin, User, etc.). 
        /// The total number of groups is also included in the response.
        /// </remarks>
        /// <returns>
        /// Returns a <see cref="GroupUserRoleStatisticsResponse"/> object which contains:
        /// - TotalGroups: The total number of groups.
        /// - Groups: A list of group statistics where each group contains:
        ///   - GroupName: The name of the group.
        ///   - TotalUsers: The total number of users in the group.
        ///   - UsersByRole: A list of roles and the count of users assigned to each role.
        /// </returns>
        public async Task<GroupUserRoleStatisticsResponse> GetGroupUserRoleStatistics()
        {
            var groups = await _dbSet
                .Select(group => new GroupUserRoleStatisticDto
                {
                    GroupId = group.Id,
                    GroupName = group.Name,
                    TotalUsers = group.Users.Count,
                    UsersByRole = group.Users
                        .GroupBy(user => user.Role.Name)
                        .Select(roleGroup => new RoleUserCount
                        {
                            RoleName = roleGroup.Key,
                            UserCount = roleGroup.Count()
                        }).ToList()
                })
                .ToListAsync();

            return new GroupUserRoleStatisticsResponse
            {
                TotalGroups = groups.Count,
                Groups = groups
            };
        }


    }
}