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

        public async Task<Notification> GetUserDeviceAsync(string fcmToken)
        {
            try
            {
                return await this._dbContext.Notifications.SingleOrDefaultAsync(x => x.FCMToken.Equals(fcmToken));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteUserDevice(Notification userDevice)
        {
            try
            {
                this._dbContext.Notifications.Remove(userDevice);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateUserDeviceAsync(Notification userDevice)
        {
            try
            {
                await this._dbContext.Notifications.AddAsync(userDevice);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Notification> GetUserDeviceAsync(int id)
        {
            try
            {
                return await this._dbContext.Notifications.SingleOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}