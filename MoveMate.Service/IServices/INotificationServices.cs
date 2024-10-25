using MoveMate.Service.ViewModels.ModelRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface INotificationServices
    {
        public Task CreateUserDeviceAsync(CreateNotificationRequest request, IEnumerable<Claim> claims);
        public Task DeleteUserDeviceAsync(int userDeviceId);
    }
}
