using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Repository.DBContext;
using Microsoft.EntityFrameworkCore;

namespace MoveMate.Repository.Repositories.Repository
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(MoveMateDbContext context) : base(context)
        {
        }
        public async Task<Payment> GetPaymentByBooingIdAsync(int bookingId)
        {
            IQueryable<Payment> query = _dbSet;
            return await query
                .Where(a => a.BookingId == bookingId)
                .FirstOrDefaultAsync();
        }
    }
}