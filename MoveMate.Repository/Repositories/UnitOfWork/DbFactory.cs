
using Microsoft.Extensions.Configuration;
using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MoveMate.Repository.Repositories.UnitOfWork
{
    public class DbFactory : Disposable, IDbFactory
    {
        private TruckRentalContext _dbContext;
      //  private RedisConnectionProvider _redisConnectionProvider;
        public DbFactory()
        {
            DotNetEnv.Env.Load();
        }

        public TruckRentalContext InitDbContext()
        {
            if (_dbContext == null)
            {
            
                var cnn = Environment.GetEnvironmentVariable("MDB");
                var finalConnectionString = "";

                if (!string.IsNullOrEmpty(cnn))
                {
                    finalConnectionString = cnn;
                }
                else
                {
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                
                    IConfigurationRoot configuration = builder.Build();
                    finalConnectionString = configuration.GetConnectionString("MyDB");
                }
                
                var optionsBuilder = new DbContextOptionsBuilder<TruckRentalContext>();
                optionsBuilder.UseSqlServer(finalConnectionString);
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        
                _dbContext = new TruckRentalContext(optionsBuilder.Options);
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
