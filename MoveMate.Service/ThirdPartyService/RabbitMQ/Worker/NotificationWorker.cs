using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker
{
    public class NotificationWorker
    {
        private UnitOfWork _unitOfWork;
        private readonly ILogger<NotificationWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IFirebaseServices _firebaseServices;

        public NotificationWorker(IUnitOfWork unitOfWork, ILogger<NotificationWorker> logger, IServiceScopeFactory serviceScopeFactory, IFirebaseServices firebaseServices)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _firebaseServices = firebaseServices;
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        [Consumer("movemate.notification_update_booking")]
        public async Task HandleMessage(int message)
        {

            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(message);
            if (booking == null)
            {
                throw new Exception($"Booking with Id {message} not found");
            }


            var notificationUser = await _unitOfWork.NotificationRepository.GetByUserIdAsync((int)booking.UserId);
            if (notificationUser == null)
            {
                throw new Exception($"Can't send notification to user with Id {booking.UserId}");
            }

            var assignments = await _unitOfWork.AssignmentsRepository.GetByBookingId(booking.Id);
            if (assignments == null)
            {
                throw new Exception($"Not found assignment with booking Id {booking.UserId}");
            }

            // Define title, body, and data for the notification
            var title = "Change Status Booking";
            var body = $"Your booking with ID {notificationUser.Id} has been changed.";
            var fcmToken = notificationUser.FcmToken;
            var data = new Dictionary<string, string>
        {
            { "bookingId", booking.Id.ToString() },
            { "status", booking.Status.ToString() },
            { "message", "The booking has been change status successfully." }
        };

            // Send notification to Firebase
            await _firebaseServices.SendNotificationAsync(title, body, fcmToken, data);

            foreach (var assignment in assignments)
            {
                // Retrieve the staff's notification token
                var notificationStaff = await _unitOfWork.NotificationRepository.GetByUserIdAsync((int)assignment.UserId);
                if (notificationStaff != null && !string.IsNullOrEmpty(notificationStaff.FcmToken))
                {
                    var titleAssignment = "Change Status Booking";
                    var bodyAssignment = $"Your assignment with ID {assignment.Id} has been changed.";
                    var fcmTokenAssignment = notificationStaff.FcmToken;
                    var dataAssignment = new Dictionary<string, string>
            {
                { "bookingId", booking.Id.ToString() },
                { "status", booking.Status.ToString() },
                { "message", "The booking has been changed successfully." }
            };

                    // Send notification for each assignment to staff
                    await _firebaseServices.SendNotificationAsync(titleAssignment, bodyAssignment, fcmTokenAssignment, dataAssignment);
                }
            }
        }
    }
}
