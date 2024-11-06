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
        public AssignmentsRepository(MoveMateDbContext context) : base(context)
        {
        }

        public Assignment GetByStaffTypeAndBookingId(string staffType, int bookingId)
        {
            var assignment = Get(a => a.StaffType == staffType && a.BookingId == bookingId)
                .FirstOrDefault();
            return assignment;
        }

        public Assignment GetByStaffTypeAndIsResponsible(string staffType, int bookingId)
        {
            var assignment = Get(a => a.StaffType == staffType && a.BookingId == bookingId && a.IsResponsible == true)
                .FirstOrDefault();
            return assignment;
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

        public async Task<List<int>> GetAvailableWithOverlapAsync(
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
                            a.Truck.TruckCategoryId == truckCategoryId && a.ScheduleBookingId == scheduleBookingId)
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();

            var overlappingDrivers = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.ScheduleBookingId == scheduleBookingId &&
                            a.UserId.HasValue &&
                            driversWithCorrectTruck.Contains(a.UserId.Value) &&
                            ((a.EndDate > newStartTime && a.StartDate < newEndTime)))
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();

            var availableDrivers = driversWithCorrectTruck
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Except(overlappingDrivers.Where(id => id.HasValue).Select(id => id.Value))
                .ToList();

            return availableDrivers;
        }

        public async Task<List<int>> GetAvailableAsync(DateTime newStartTime, DateTime newEndTime,
            int scheduleBookingId, int truckCategoryId)
        {
            IQueryable<Assignment>
                query = _dbSet.Include(a => a.Truck); // Bao gồm Truck để truy xuất thông tin về xe tải

            var allDrivers = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId) // Lọc theo TruckCategoryId
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
        
        public async Task<List<int>> GetAvailableWithExtendedAsync(DateTime newStartTime, DateTime newEndTime,
            int scheduleBookingId, int truckCategoryId, double? useExtendedTime = 1)
        {
            var newStartTimeExtended = newStartTime.AddHours(-useExtendedTime!.Value);
            var newEndTimeExtended = newEndTime.AddHours(useExtendedTime!.Value);
            
            IQueryable<Assignment>
                query = _dbSet.Include(a => a.Truck); // Bao gồm Truck để truy xuất thông tin về xe tải

            var allDrivers = await query
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Truck.TruckCategoryId == truckCategoryId) // Lọc theo TruckCategoryId
                .Where(a =>
                    a.ScheduleBookingId == scheduleBookingId
                    && (((newStartTimeExtended <= a.EndDate && a.EndDate <= newStartTime) || (a.StartDate <= newEndTimeExtended && a.StartDate >= newEndTime))))
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();

            var availableDrivers = allDrivers
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();

            return availableDrivers;
        }
    }
}