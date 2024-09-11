using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.GenericRepository;
using MoveMate.Repository.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoveMate.Domain.DBContext;
using System.Threading.Tasks;

namespace MoveMate.Repository.Repositories.Repository
{
    public class AchievementRepository : GenericRepository<Achievement>, IAchievementRepository
    {
        public AchievementRepository(MoveMateDbContext context) : base(context)
        {
        }
    }
}
