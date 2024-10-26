﻿using MoveMate.Domain.Models;
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
    }
}