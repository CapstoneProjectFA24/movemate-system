using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using MoveMate.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.DBContext;

namespace MoveMate.Repository.Repositories.Repository
{
    public class TruckRepository : GenericRepository<Truck>, ITruckRepository
    {
        public TruckRepository(MoveMateDbContext context) : base(context)
        {
        }
    }
}