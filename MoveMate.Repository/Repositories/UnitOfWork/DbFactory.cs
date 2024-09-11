
using Microsoft.Extensions.Configuration;
using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoveMate.API.Utils;
using MoveMate.Domain.DBContext;

namespace MoveMate.Repository.Repositories.UnitOfWork
{
    public class DbFactory : Disposable, IDbFactory
    {
        private MoveMateDbContext _dbContext;
      //  private RedisConnectionProvider _redisConnectionProvider;
        public DbFactory()
        {
            
        }

        public MoveMateDbContext InitDbContext()
        {
            if (_dbContext == null)
            {

                string connectionString = DbUtil.getConnectString();
                
                var optionsBuilder = new DbContextOptionsBuilder<MoveMateDbContext>();
                optionsBuilder.UseSqlServer(connectionString);
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        
                _dbContext = new MoveMateDbContext(optionsBuilder.Options);
            }
            return _dbContext;
        }

        //public async Task<RedisConnectionProvider> InitRedisConnectionProvider()
        //{
        //    if (this._redisConnectionProvider == null)
        //    {
        //        var builder = new ConfigurationBuilder()
        //                          .SetBasePath(Directory.GetCurrentDirectory())
        //                          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //        IConfigurationRoot configuration = builder.Build();
        //        this._redisConnectionProvider = new RedisConnectionProvider(configuration.GetConnectionString("RedisDbStore"));
        //        await this._redisConnectionProvider.Connection.CreateIndexAsync(typeof(AccountToken));
        //        await this._redisConnectionProvider.Connection.CreateIndexAsync(typeof(EmailVerification));
        //    }
        //    return this._redisConnectionProvider;
        //}

        protected override void DisposeCore()
        {
            if (this._dbContext != null)
            {
                this._dbContext.Dispose();
            }
        }
    }
}
