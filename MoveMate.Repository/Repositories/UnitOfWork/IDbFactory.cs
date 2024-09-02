

using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Repository.Repositories.UnitOfWork
{
    public interface IDbFactory : IDisposable
    {
        public TruckRentalContext InitDbContext();
      //  public Task<RedisConnectionProvider> InitRedisConnectionProvider();
    }
}
