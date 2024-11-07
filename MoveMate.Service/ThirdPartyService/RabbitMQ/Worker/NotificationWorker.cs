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
        public async Task HandleMessage(int bookingId)
        {
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
                if (booking == null)
                {
                    throw new Exception($"Booking with ID {bookingId} not found.");               
                }

                var notificationUser = await _unitOfWork.NotificationRepository.GetByUserIdAsync(booking.UserId ?? 0);
                if (notificationUser?.FcmToken == null)
                {
                    throw new Exception($"Notification token for user with ID {booking.UserId} not found.");
                }

                // Send notification to the booking user
                await SendNotificationAsync(
                    "Change Status Booking",
                    $"Your booking with ID {booking.Id} has been updated.",
                    notificationUser.FcmToken,
                    booking.Id,
                    booking.Status
                );

                // Notify each assignment related to the booking
                var assignments = await _unitOfWork.AssignmentsRepository.GetByBookingId(booking.Id);
                foreach (var assignment in assignments)
                {
                    var notificationStaff = await _unitOfWork.NotificationRepository.GetByUserIdAsync((int)assignment.UserId);
                    if (notificationStaff?.FcmToken != null)
                    {
                        await SendNotificationAsync(
                            "Change Status Booking",
                            $"Your assignment with ID {assignment.Id} has been updated.",
                            notificationStaff.FcmToken,
                            booking.Id,
                            booking.Status
                        );
                    }
                }
            }
            catch (Exception ex)
            {
               throw new Exception($"Error handling notification for booking ID {bookingId}: {ex.Message}", ex);
            }
        }

        private async Task SendNotificationAsync(string title, string body, string fcmToken, int bookingId, string status)
        {
            var data = new Dictionary<string, string>
            {
                { "bookingId", bookingId.ToString() },
                { "status", status.ToString() },
                { "message", "The booking status has been updated successfully." }
            };

            await _firebaseServices.SendNotificationAsync(title, body, fcmToken, data);
        }
    }
}
