﻿using Microsoft.EntityFrameworkCore;
using MoveMate.Domain.DBContext;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Repository.Repositories.Repository
{
    public class ScheduleWorkingRepository : GenericRepository<ScheduleWorking>, IScheduleWorkingRepository
    {
        public ScheduleWorkingRepository(MoveMateDbContext context) : base(context)
        {
        }

        public virtual async Task<ScheduleWorking?> GetByIdAsync(int id, string includeProperties = "")
        {
            IQueryable<ScheduleWorking> query = _dbSet;

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

        public async Task<ScheduleWorking?> GetScheduleByBookingAtAsync(DateTime bookingAt)
        {
            TimeOnly bookingTime = TimeOnly.FromDateTime(bookingAt);

            return await _dbSet
                .Where(s =>
                    (s.StartDate <= s.EndDate && s.StartDate <= bookingTime && bookingTime <= s.EndDate)
                    ||
                    (s.StartDate > s.EndDate && (bookingTime >= s.StartDate || bookingTime <= s.EndDate))
                )
                .FirstOrDefaultAsync();
        }
    }
}