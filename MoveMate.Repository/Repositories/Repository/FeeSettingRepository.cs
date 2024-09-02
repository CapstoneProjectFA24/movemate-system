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
    public class FeeSettingRepository : GenericRepository<FeeSetting>, IFeeSettingRepository
    {
        public FeeSettingRepository(TruckRentalContext context) : base(context)
        {
        }
    }
}
