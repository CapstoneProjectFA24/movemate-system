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
        }

        public async Task<Notification?> FirstOrDefaultAsync(int accountId, string deviceId)
        {
            try
            {
                return await _dbContext.Notifications
                    .FirstOrDefaultAsync(n => n.UserId == accountId && n.DeviceId == deviceId);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching the notification: {ex.Message}");
            }
        }
    }
}