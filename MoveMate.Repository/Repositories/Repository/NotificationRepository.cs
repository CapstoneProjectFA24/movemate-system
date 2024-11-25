using MoveMate.Domain.Models;
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
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly MoveMateDbContext _dbContext;

        public NotificationRepository(MoveMateDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<Notification?> FirstOrDefaultAsync(int accountId, string deviceId)
        {
            try
            {
                IQueryable<Notification> query = _dbSet;
                return await query
                        .FirstOrDefaultAsync(n => n.UserId == accountId && n.DeviceId == deviceId);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching the notification: {ex.Message}");
            }
        }



        public async Task<List<Notification>> GetNotiAsync(string fcmToken)
        {
            IQueryable<Notification> query = _dbSet;
            return await query
                           .Where(p => p.FcmToken == fcmToken).ToListAsync();
                
        }

        public async Task<Notification> GetByUserIdAsync(int userId)
        {
            IQueryable<Notification> query = _dbSet;
            return await query
                               .FirstOrDefaultAsync(x => x.UserId == userId && x.FcmToken != null);          
        }

    }
}