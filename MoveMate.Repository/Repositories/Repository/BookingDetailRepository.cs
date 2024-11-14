﻿using MoveMate.Domain.Models;
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
    public class BookingDetailRepository : GenericRepository<BookingDetail>, IBookingDetailRepository
    {
        public BookingDetailRepository(MoveMateDbContext context) : base(context)
        {
        }

        //public async Task<BookingDetail> GetBookingDetailAsyncByBookingId(int bookingId)
        //{
        //    return await _context.Set<BookingDetail>()
        //        .Include(b => b.User)
        //        .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        //}

        public async Task<BookingDetail?> GetAsyncByServiceIdAndBookingId(int serviceId, int bookingId)
        {
            IQueryable<BookingDetail> query = _dbSet;
            return await query
                .Where(b => b.ServiceId == serviceId && b.BookingId == bookingId)
                .FirstOrDefaultAsync();
        }
        
        public async Task<BookingDetail?> GetAsyncByTypeAndBookingId(string type, int bookingId)
        {
            IQueryable<BookingDetail> query = _dbSet;
            return await query
                .Where(b => b.Type == type && b.BookingId == bookingId)
                .OrderBy(b => b.ServiceId)
                .FirstOrDefaultAsync();
        }

    }
}