using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoveMate.Repository.DBContext;

namespace MoveMate.Repository.Repositories.Repository
{
    public class TruckImgRepository : GenericRepository<TruckImg>, ITruckImgRepository
    {
        public TruckImgRepository(MoveMateDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TruckImg>> GetByTruckIdAsync(int truckId)
        {
            IQueryable<TruckImg> query = _dbSet;
            return await query
                .Where(img => img.TruckId == truckId)
                .ToListAsync();
        }
    }
}