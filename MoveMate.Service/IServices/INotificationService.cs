using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface INotificationService
    {
        public Task<OperationResult<bool>> ManagerReadMail(int userId, int notiId);
        public Task<OperationResult<List<NotificationResponse>>> GetAll(GetAllNotificationRequest request);
        public Task<OperationResult<List<BookingTrackerResponse>>> GetAllBookingTracker(GetAllBookingTrackerReport request);
    }
}
