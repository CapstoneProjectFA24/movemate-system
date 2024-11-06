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
    public class AssignmentsRepository : GenericRepository<Assignment>, IAssignmentsRepository
    {
        private readonly MoveMateDbContext _context;

        public AssignmentsRepository(MoveMateDbContext context) : base(context)
        {
            _context = context;
        }

        public Assignment GetByStaffTypeAndBookingId(string staffType, int bookingId)
        {
            var assignment = Get(a => a.StaffType == staffType && a.BookingId == bookingId)
                .FirstOrDefault();
            return assignment;
        }

        public async Task<List<Assignment>> GetByBookingId(int bookingId)
        {
            return await _context.Assignments
                .Where(p => p.BookingId == bookingId).ToListAsync();
        }


        public Assignment GetByStaffTypeAndIsResponsible(string staffType, int bookingId)
        {
            var assignment = Get(a => a.StaffType == staffType && a.BookingId == bookingId && a.IsResponsible == true)
                .FirstOrDefault();
            return assignment;
        }


        public async Task<Assignment> GetByUserIdAndStaffTypeAndIsResponsible(int userId, string staffType,
            int bookingId)
        {
            return await _context.Assignments
                .Where(a => a.UserId == userId && a.StaffType == staffType && a.BookingId == bookingId &&
                            a.IsResponsible == true)
                .FirstOrDefaultAsync();
        }

        public async Task<Assignment> GetByUserIdAndStaffType(int userId, string staffType, int bookingId)
        {
            return await _context.Assignments
                .Where(a => a.UserId == userId && a.StaffType == staffType && a.BookingId == bookingId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<int>> GetAvailableAsync(DateTime newStartTime, DateTime newEndTime,
            int scheduleBookingId, string staffType)
        {
            IQueryable<Assignment> query = _dbSet;

            var allDrivers = await query
                .Where(a => a.StaffType == staffType)
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();

            var overlappingDrivers = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString()
                            && a.ScheduleBookingId == scheduleBookingId
                            && ((a.EndDate > newStartTime && a.StartDate < newEndTime)))
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();

            var availableDrivers = allDrivers
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Except(overlappingDrivers.Where(id => id.HasValue).Select(id => id.Value)) // Xử lý overlappingDrivers
                .ToList();

            return availableDrivers;
        }

        public async Task<List<Assignment>> GetAvailableWithOverlapAsync(
            DateTime newStartTime,
            DateTime newEndTime,
            int scheduleBookingId,
            int? truckCategoryId,
            double? useExtendedTime = 0)
        {
            IQueryable<Assignment> query = _dbSet.Include(a => a.Truck);

            newStartTime = newStartTime.AddHours(-useExtendedTime!.Value);
            newEndTime = newEndTime.AddHours(useExtendedTime!.Value);


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


        public async Task<List<int>> GetAvailableAsync(DateTime newStartTime, DateTime newEndTime,
            int scheduleBookingId, int truckCategoryId)
        {
            IQueryable<Assignment>
                query = _dbSet.Include(a => a.Truck); 
            var allDrivers = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId) 
                .Where(a =>
                    a.ScheduleBookingId == scheduleBookingId
                    && ((a.EndDate <= newStartTime || a.StartDate >= newEndTime)))
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();

            var availableDrivers = allDrivers
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            return availableDrivers;
        }

        public async Task<List<int>> GetListDriverIdAvailableWithExtendedAsync(DateTime newStartTime,
            DateTime newEndTime,
            int scheduleBookingId, int truckCategoryId, double? useExtendedTime = 1, double? useBufferTime = 0)
        {
            newStartTime = newStartTime.AddMinutes(-(30 + useBufferTime!.Value));
            newEndTime = newEndTime.AddMinutes((30 + useBufferTime!.Value));

            var newStartTimeExtended = newStartTime.AddHours(-useExtendedTime!.Value);
            var newEndTimeExtended = newEndTime.AddHours(useExtendedTime!.Value);

            IQueryable<Assignment>
                query = _dbSet.Include(a => a.Truck);

            var conflictedDrivers = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId)
                .Where(a =>
                    (a.StartDate < newEndTime && a.EndDate > newStartTime))
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();

            var availableDrivers = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId)
                .Where(a =>
                    a.ScheduleBookingId == scheduleBookingId
                    && (((newStartTimeExtended <= a.EndDate && a.EndDate <= newStartTime) ||
                         (a.StartDate <= newEndTimeExtended && a.StartDate >= newEndTime))))
                .Where(a => !conflictedDrivers.Contains(a.UserId))
                .Select(a => a.UserId)
                .Distinct()
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToListAsync();

            return availableDrivers;
        }

        public async Task<List<Assignment>> GetDriverAvailableWithExtendedAsync(DateTime newStartTime,
            DateTime newEndTime,
            int scheduleBookingId, int truckCategoryId, double? useExtendedTime = 1, double? useBufferTime = 0)
        {
            newStartTime = newStartTime.AddMinutes(-(30 + useBufferTime!.Value));
            newEndTime = newEndTime.AddMinutes((30 + useBufferTime!.Value));

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

            // Tìm danh sách Assignment cho các tài xế rảnh
            var availableAssignments = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId)
                .Where(a => a.ScheduleBookingId == scheduleBookingId
                            && ((newStartTimeExtended <= a.EndDate && a.EndDate <= newStartTime) ||
                                (a.StartDate <= newEndTimeExtended && a.StartDate >= newEndTime)))
                .Where(a => !conflictedDrivers.Contains(a.UserId))
                .Distinct()
                .ToListAsync();

            return availableAssignments;
        }

        public async Task<List<Assignment>> GetDriverAvailableAsync(DateTime newStartTime, DateTime newEndTime,
            int scheduleBookingId, int truckCategoryId, double? useBufferTime = 0)
        {
            newStartTime = newStartTime.AddMinutes(-(30 + useBufferTime!.Value));
            newEndTime = newEndTime.AddMinutes((30 + useBufferTime!.Value));

            IQueryable<Assignment> query = _dbSet
                .Include(a => a.Truck)
                .Include(a => a.Booking);

            var Drivers = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId)
                .Where(a => a.StartDate > newEndTime && a.EndDate < newStartTime)
                .Distinct()
                .ToListAsync();


            return Drivers;
        }
    }
}