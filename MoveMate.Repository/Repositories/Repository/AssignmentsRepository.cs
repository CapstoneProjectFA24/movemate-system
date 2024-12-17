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

namespace MoveMate.Repository.Repositories.Repository
{
    public class AssignmentsRepository : GenericRepository<Assignment>, IAssignmentsRepository
    {
        public AssignmentsRepository(MoveMateDbContext context) : base(context)
        {
        }
        public virtual async Task<Assignment?> GetByIdAsync(int id, string includeProperties = "")
        {
            IQueryable<Assignment> query = _dbSet;

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
        public Assignment GetByStaffTypeAndBookingId(string staffType, int bookingId)
        {
            IQueryable<Assignment> query = _dbSet;
            return query
                .Where(a => a.StaffType == staffType && a.BookingId == bookingId)
                .FirstOrDefault();
        }
        public async Task<List<Assignment>> GetAssignmentByBookingDetailIdAndStatusAsync(int bookingDetailId, string status)
        {
            IQueryable<Assignment> query = _dbSet;

            var assignments = await query
                .Where(a => a.BookingDetailsId == bookingDetailId && a.Status == status)
                .ToListAsync(); // Execute and return the list of assignments

            return assignments;
        }
        public async Task<List<Assignment>> GetAssignmentByUserIdAndFCMTokenAsync(int bookingId)
        {
            IQueryable<Assignment> query = _dbSet
                .Include(a => a.User)
                .ThenInclude(u => u.Notifications);

            var assignments = await query              
                .Where(a => a.BookingId == bookingId && a.Status != AssignmentStatusEnums.FAILED.ToString() && a.User.Notifications.Any(n => n.FcmToken != null))
                .ToListAsync(); // Execute and return the list of assignments

            return assignments;
        }
        public async Task<Assignment> GetAssignmentByBookingIdAndStaffTypeAndFCMTokenAsync(int bookingId, string staffType)
        {
            IQueryable<Assignment> query = _dbSet
                .Include(a => a.User)
                .ThenInclude(u => u.Notifications);

            var assignments = await query
                .Where(a => a.BookingId == bookingId && a.IsResponsible == true && a.StaffType == staffType && a.Status != AssignmentStatusEnums.FAILED.ToString() && a.User.Notifications.Any(n => n.FcmToken != null))
                .FirstOrDefaultAsync(); // Execute and return the list of assignments

            return assignments;
        }

        public async Task<List<Assignment>> GetByBookingId(int bookingId)
        {
            IQueryable<Assignment> query = _dbSet;
            return await query
                .Where(p => p.BookingId == bookingId)
                .ToListAsync();
        }

        public async Task<List<Assignment>> GetByBookingIdAndIsReposibility(int bookingId)
        {
            IQueryable<Assignment> query = _dbSet;
            return await query
                .Where(p => p.BookingId == bookingId && p.IsResponsible == true)
                .ToListAsync();
        }

        public Assignment GetByStaffTypeAndIsResponsible(string staffType, int bookingId)
        {
            IQueryable<Assignment> query = _dbSet;
            return query
                .Where(a => a.StaffType == staffType && a.BookingId == bookingId && a.Status != AssignmentStatusEnums.FAILED.ToString() && a.IsResponsible == true)
                .FirstOrDefault();
        }

        public async Task<List<Assignment>> GetByStaffTypeAsync(string staffType, int bookingId, int assignmentId)
        {
            IQueryable<Assignment> query = _dbSet;
            return await query
                .Where(a => a.StaffType == staffType && a.BookingId == bookingId && a.IsResponsible == false &&
                            a.Id != assignmentId && a.Status != AssignmentStatusEnums.FAILED.ToString())
                .ToListAsync();
        }

        public async Task<List<Assignment>> GetAllByStaffType(string staffType, int bookingId)
        {
            IQueryable<Assignment> query = _dbSet;
            return await query
                .Where(a => a.BookingId == bookingId && a.StaffType == staffType && a.Status != AssignmentStatusEnums.FAILED.ToString() && a.IsResponsible == false )
                .ToListAsync();
        }

        public async Task<Assignment> GetByUserIdAndStaffTypeAndIsResponsible(int userId, string staffType,
            int bookingId)
        {
            IQueryable<Assignment> query = _dbSet;
            return await query
                .Where(a => a.UserId == userId
                            && a.StaffType == staffType
                            && a.BookingId == bookingId
                            && a.IsResponsible == true)
                .FirstOrDefaultAsync();
        }

        public async Task<Assignment> GetByUserIdAndIsResponsible(int userId, int bookingId)
        {
            IQueryable<Assignment> query = _dbSet;
            return await query
                .Where(a => a.UserId == userId
                            && a.BookingId == bookingId
                            && a.IsResponsible == true)
                .FirstOrDefaultAsync();
        }

        public async Task<Assignment> GetByUserIdAndStaffType(int userId, string staffType, int bookingId)
        {
            IQueryable<Assignment> query = _dbSet;
            return await query
                .Where(a => a.UserId == userId && a.StaffType == staffType && a.BookingId == bookingId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Assignment>> GetByUserId(int userId)
        {
            IQueryable<Assignment> query = _dbSet;
            return await query
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Assignment>> GetDriverAvailableWithOverlapAsync(
            DateTime newStartTime,
            DateTime newEndTime,
            int scheduleBookingId,
            int? truckCategoryId,
            double? useExtendedTime = 0)
        {
            IQueryable<Assignment> query = _dbSet
                .Include(a => a.Truck)
                .Include(a => a.Booking);

            newStartTime = newStartTime.AddHours(-(0.5 + useExtendedTime!.Value));
            newEndTime = newEndTime.AddHours((0.5 + useExtendedTime!.Value));


            var driversWithCorrectTruck = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId &&
                            a.ScheduleBookingId == scheduleBookingId)
                .Distinct()
                .ToListAsync();


            var overlappingAssignments = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.ScheduleBookingId == scheduleBookingId &&
                            a.UserId.HasValue &&
                            driversWithCorrectTruck.Select(d => d.UserId).Contains(a.UserId) &&
                            ((a.EndDate > newStartTime && a.StartDate < newEndTime)))
                .Distinct()
                .ToListAsync();


            var availableAssignments = driversWithCorrectTruck
                .Where(a => a.UserId.HasValue &&
                            !overlappingAssignments.Any(o => o.UserId == a.UserId))
                .ToList();

            return availableAssignments;
        }

        public async Task<List<Assignment>> GetDriverByGroupAvailableWithOverlapAsync(
            DateTime newStartTime,
            DateTime newEndTime,
            int scheduleBookingId,
            int? truckCategoryId,
            int? groupId,
            double? useExtendedTime = 0)
        {
            IQueryable<Assignment> query = _dbSet
                .Include(a => a.Truck)
                .Include(a => a.Booking)
                .Include(a => a.User);


            newStartTime = newStartTime.AddHours(-(0.5 + useExtendedTime!.Value));
            newEndTime = newEndTime.AddHours((0.5 + useExtendedTime!.Value));


            var driversWithCorrectTruck = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId &&
                            a.ScheduleBookingId == scheduleBookingId &&
                            a.User.GroupId == groupId)
                .Distinct()
                .ToListAsync();


            var overlappingAssignments = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.ScheduleBookingId == scheduleBookingId &&
                            a.Truck.TruckCategoryId == truckCategoryId &&
                            a.User.GroupId == groupId &&
                            a.UserId.HasValue &&
                            driversWithCorrectTruck.Select(d => d.UserId).Contains(a.UserId) &&
                            ((a.EndDate > newStartTime && a.StartDate < newEndTime)))
                .Distinct()
                .ToListAsync();


            var availableAssignments = driversWithCorrectTruck
                .Where(a => a.UserId.HasValue &&
                            !overlappingAssignments.Any(o => o.UserId == a.UserId))
                .ToList();

            return availableAssignments;
        }

        public async Task<List<Assignment>> GetPorterAvailableWithOverlapAsync(
            DateTime newStartTime,
            DateTime newEndTime,
            int scheduleBookingId,
            double? useExtendedTime = 0)
        {
            IQueryable<Assignment> query = _dbSet
                .Include(a => a.Booking);

            newStartTime = newStartTime.AddHours(-(0.5 + useExtendedTime!.Value));
            newEndTime = newEndTime.AddHours((0.5 + useExtendedTime!.Value));


            var porters = await query
                .Where(a => a.StaffType == RoleEnums.PORTER.ToString() &&
                            a.ScheduleBookingId == scheduleBookingId)
                .Distinct()
                .ToListAsync();

            var overlappingAssignments = await query
                .Where(a => a.StaffType == RoleEnums.PORTER.ToString() &&
                            a.ScheduleBookingId == scheduleBookingId &&
                            a.UserId.HasValue &&
                            porters.Select(d => d.UserId).Contains(a.UserId) &&
                            ((a.EndDate > newStartTime && a.StartDate < newEndTime)))
                .Distinct()
                .ToListAsync();


            var availableAssignments = porters
                .Where(a => a.UserId.HasValue &&
                            !overlappingAssignments.Any(o => o.UserId == a.UserId))
                .ToList();

            return availableAssignments;
        }

        public async Task<List<Assignment>> GetPorterByGroupAvailableWithOverlapAsync(
            DateTime newStartTime,
            DateTime newEndTime,
            int scheduleBookingId,
            int? groupId,
            double? useExtendedTime = 0
        )
        {
            IQueryable<Assignment> query = _dbSet
                .Include(a => a.Booking)
                .Include(a => a.User);

            newStartTime = newStartTime.AddHours(-(0.5 + useExtendedTime!.Value));
            newEndTime = newEndTime.AddHours((0.5 + useExtendedTime!.Value));


            var porters = await query
                .Where(a => a.StaffType == RoleEnums.PORTER.ToString() &&
                            a.ScheduleBookingId == scheduleBookingId &&
                            a.User.GroupId == groupId)
                .Distinct()
                .ToListAsync();

            var overlappingAssignments = await query
                .Where(a => a.StaffType == RoleEnums.PORTER.ToString() &&
                            a.ScheduleBookingId == scheduleBookingId &&
                            a.User.GroupId == groupId &&
                            a.UserId.HasValue &&
                            porters.Select(d => d.UserId).Contains(a.UserId) &&
                            ((a.EndDate > newStartTime && a.StartDate < newEndTime)))
                .Distinct()
                .ToListAsync();


            var availableAssignments = porters
                .Where(a => a.UserId.HasValue &&
                            !overlappingAssignments.Any(o => o.UserId == a.UserId))
                .ToList();

            return availableAssignments;
        }

        public async Task<List<Assignment>> GetDriverAvailableWithExtendedAsync(DateTime newStartTime,
            DateTime newEndTime,
            int scheduleBookingId, int truckCategoryId, double? useExtendedTime = 1, double? useBufferTime = 0)
        {
            newStartTime = newStartTime.AddHours(-(0.5 + useBufferTime!.Value));
            newEndTime = newEndTime.AddHours((0.5 + useBufferTime!.Value));

            var newStartTimeExtended = newStartTime.AddHours(-useExtendedTime!.Value);
            var newEndTimeExtended = newEndTime.AddHours(useExtendedTime!.Value);

            IQueryable<Assignment> query = _dbSet
                .Include(a => a.Truck)
                .Include(a => a.Booking);

            var conflictedDrivers = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId)
                .Where(a => a.StartDate < newEndTime && a.EndDate > newStartTime)
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();

            var availableAssignments = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId)
                .Where(a => a.ScheduleBookingId == scheduleBookingId
                            && ((newStartTimeExtended <= a.EndDate && a.EndDate <= newStartTime) ||
                                (a.StartDate <= newEndTimeExtended && a.StartDate >= newEndTime)))
                .Where(a => !conflictedDrivers.Contains(a.UserId))
                .GroupBy(a => a.UserId)
                .Select(g => g.First())
                .ToListAsync();

            return availableAssignments;
        }

        public async Task<List<Assignment>> GetDriverByGroupAvailableWithExtendedAsync(DateTime newStartTime,
            DateTime newEndTime,
            int scheduleBookingId, int truckCategoryId,
            int? groupId,
            double? useExtendedTime = 1, double? useBufferTime = 0)
        {
            newStartTime = newStartTime.AddHours(-(0.5 + useBufferTime!.Value));
            newEndTime = newEndTime.AddHours((0.5 + useBufferTime!.Value));

            var newStartTimeExtended = newStartTime.AddHours(-useExtendedTime!.Value);
            var newEndTimeExtended = newEndTime.AddHours(useExtendedTime!.Value);

            IQueryable<Assignment> query = _dbSet
                .Include(a => a.Truck)
                .Include(a => a.Booking)
                .Include(a => a.User);

            var conflictedDrivers = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId && a.User.GroupId == groupId)
                .Where(a => a.StartDate < newEndTime && a.EndDate > newStartTime)
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();

            var availableAssignments = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId && a.User.GroupId == groupId)
                .Where(a => a.ScheduleBookingId == scheduleBookingId
                            && ((newStartTimeExtended <= a.EndDate && a.EndDate <= newStartTime) ||
                                (a.StartDate <= newEndTimeExtended && a.StartDate >= newEndTime)))
                .Where(a => !conflictedDrivers.Contains(a.UserId))
                .GroupBy(a => a.UserId)
                .Select(g => g.First())
                .ToListAsync();

            return availableAssignments;
        }

        public async Task<List<Assignment>> GetPortersAvailableWithExtendedAsync(DateTime newStartTime,
            DateTime newEndTime,
            int scheduleBookingId, double? useExtendedTime = 1, double? useBufferTime = 0)
        {
            newStartTime = newStartTime.AddHours(-(0.5 + useBufferTime!.Value));
            newEndTime = newEndTime.AddHours((0.5 + useBufferTime!.Value));

            var newStartTimeExtended = newStartTime.AddHours(-useExtendedTime!.Value);
            var newEndTimeExtended = newEndTime.AddHours(useExtendedTime!.Value);

            IQueryable<Assignment> query = _dbSet
                .Include(a => a.Booking);

            var conflictedDrivers = await query
                .Where(a => a.StaffType == RoleEnums.PORTER.ToString())
                .Where(a => a.StartDate < newEndTime && a.EndDate > newStartTime)
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();

            var availableAssignments = await query
                .Where(a => a.StaffType == RoleEnums.PORTER.ToString())
                .Where(a => a.ScheduleBookingId == scheduleBookingId
                            && ((newStartTimeExtended <= a.EndDate && a.EndDate <= newStartTime) ||
                                (a.StartDate <= newEndTimeExtended && a.StartDate >= newEndTime)))
                .Where(a => !conflictedDrivers.Contains(a.UserId))
                .GroupBy(a => a.UserId)
                .Select(g => g.First())
                .ToListAsync();


            return availableAssignments;
        }

        public async Task<List<Assignment>> GetPortersByGroupAvailableWithExtendedAsync(DateTime newStartTime,
            DateTime newEndTime,
            int scheduleBookingId,
            int? groupId,
            double? useExtendedTime = 1, double? useBufferTime = 0)
        {
            newStartTime = newStartTime.AddHours(-(0.5 + useBufferTime!.Value));
            newEndTime = newEndTime.AddHours((0.5 + useBufferTime!.Value));

            var newStartTimeExtended = newStartTime.AddHours(-useExtendedTime!.Value);
            var newEndTimeExtended = newEndTime.AddHours(useExtendedTime!.Value);

            IQueryable<Assignment> query = _dbSet
                .Include(a => a.Booking);

            var conflictedDrivers = await query
                .Where(a => a.StaffType == RoleEnums.PORTER.ToString() && a.User.GroupId == groupId)
                .Where(a => a.StartDate < newEndTime && a.EndDate > newStartTime)
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();

            var availableAssignments = await query
                .Where(a => a.StaffType == RoleEnums.PORTER.ToString() && a.User.GroupId == groupId)
                .Where(a => a.ScheduleBookingId == scheduleBookingId
                            && ((newStartTimeExtended <= a.EndDate && a.EndDate <= newStartTime) ||
                                (a.StartDate <= newEndTimeExtended && a.StartDate >= newEndTime)))
                .Where(a => !conflictedDrivers.Contains(a.UserId))
                .GroupBy(a => a.UserId)
                .Select(g => g.First())
                .ToListAsync();


            return availableAssignments;
        }
    }
}