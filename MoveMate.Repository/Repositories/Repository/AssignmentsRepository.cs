using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.DBContext;
using Microsoft.EntityFrameworkCore;

namespace MoveMate.Repository.Repositories.Repository
{
    public class AssignmentsRepository : GenericRepository<Assignment>, IAssignmentsRepository
    {
        private readonly MoveMateDbContext _dbContext;
        public AssignmentsRepository(MoveMateDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public Assignment GetByStaffTypeAndBookingId(string staffType, int bookingId)
        {
            var assignment = Get(a => a.StaffType == staffType && a.BookingId == bookingId)
                                .FirstOrDefault();
            return assignment;
        }

        public async Task<List<Assignment>> GetByBookingId(int bookingId)
        {
            return await _dbContext.Assignments
                .Where(p => p.BookingId == bookingId).ToListAsync();

        }

      

        public Assignment GetByStaffTypeAndIsResponsible(string staffType, int bookingId)
        {
            var assignment = Get(a => a.StaffType == staffType && a.BookingId == bookingId && a.IsResponsible == true)
                                .FirstOrDefault();
            return assignment;
        }

    }
}