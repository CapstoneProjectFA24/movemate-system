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
using AutoMapper;
using MoveMate.Service.ThirdPartyService.Redis;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker
{
    public class NotificationWorker
    {
        private readonly ILogger<NotificationWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public NotificationWorker(ILogger<NotificationWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        [Consumer("movemate.notification_update_booking")]
        public async Task HandleMessage(int message)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                    var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                    var firebaseServices = scope.ServiceProvider.GetRequiredService<IFirebaseServices>();

                    var booking = await unitOfWork.BookingRepository.GetByIdAsync(message);
                    if (booking == null)
                    {
                        throw new Exception($"Booking with Id {message} not found");
                    }


                    var notificationUser =
                        await unitOfWork.NotificationRepository.GetByUserIdAsync((int)booking.UserId);
                    if (notificationUser == null)
                    {
                        throw new Exception($"Can't send notification to user with Id {booking.UserId}");
                    }

                    var assignments = await unitOfWork.AssignmentsRepository.GetByBookingId(booking.Id);
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
                    await firebaseServices.SendNotificationAsync(title, body, fcmToken, data);

                    foreach (var assignment in assignments)
                    {
                        // Retrieve the staff's notification token
                        var notificationStaff =
                            await unitOfWork.NotificationRepository.GetByUserIdAsync((int)assignment.UserId);
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
                            await firebaseServices.SendNotificationAsync(titleAssignment, bodyAssignment,
                                fcmTokenAssignment,
                                dataAssignment);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing booking notification for message {Message}", message);
                throw;
            }
            
            Console.WriteLine($"Received message to queue movemate.notification_update_booking: {message}");
        }
    }
}