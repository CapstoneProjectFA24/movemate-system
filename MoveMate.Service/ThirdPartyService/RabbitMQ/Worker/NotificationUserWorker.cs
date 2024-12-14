using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;
using MoveMate.Service.ThirdPartyService.RabbitMQ.DTO;
using MoveMate.Service.ThirdPartyService.Redis;
using Parlot.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker
{
    public class NotificationUserWorker
    {
        private readonly ILogger<NotificationUserWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public NotificationUserWorker(ILogger<NotificationUserWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        [Consumer("movemate.notification_user")]
        public async Task HandleMessage(NotiListDto message)
        {
            await Task.Delay(100);
            Console.WriteLine("movemate.notification_user");
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    // var
                    var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                    var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                    var firebaseServices = scope.ServiceProvider.GetRequiredService<IFirebaseServices>();

                    {
                        var booking = await unitOfWork.BookingRepository.GetByIdAsync(message.BookingId, includeProperties: "BookingTrackers");
                        switch (message.Type)
                        {
                            case var type when type == NotificationEnums.CUSTOMER_REPORT.ToString():
                                var assignmentPorter = await unitOfWork.AssignmentsRepository.GetAssignmentByBookingIdAndStaffTypeAndFCMTokenAsync(message.BookingId, message.StaffType);
                                var notificationStaffPorter = assignmentPorter.User.Notifications.FirstOrDefault(n => n.FcmToken != null);
                                if (notificationStaffPorter != null && !string.IsNullOrEmpty(notificationStaffPorter.FcmToken))
                                {
                                    var titleAssignment = "Khách hàng báo cáo sự cố";
                                    var bodyAssignment = $"Khách hàng đã báo cáo đồ đạc bị hư hỏng cho mã đơn {booking.Id}.";
                                    var fcmTokenAssignment = notificationStaffPorter.FcmToken;
                                    var dataAssignment = new Dictionary<string, string>
                                    {
                                        { "bookingId", booking.Id.ToString() },
                                        { "status", booking.Status.ToString() },
                                        { "message", "khách hàng đã báo cáo một vấn đề với đơn hàng" },
                                        { "bookingTracker", booking.BookingTrackers.Count(b => b.Type == TrackerEnums.MONETARY.ToString()).ToString()}
                                    };

                                    // Send notification for each assignment to staff
                                    await firebaseServices.SendNotificationAsync(titleAssignment, bodyAssignment,
                                        fcmTokenAssignment,
                                        dataAssignment);
                                }

                                break;

                            case var type when type == NotificationEnums.ASSIGNED_LEADER.ToString():
                                var assignment = await unitOfWork.AssignmentsRepository.GetAssignmentByBookingIdAndStaffTypeAndFCMTokenAsync(message.BookingId, message.StaffType);
                                var notificationStaff = assignment.User.Notifications.FirstOrDefault(n => n.FcmToken != null);
                                if (notificationStaff != null && !string.IsNullOrEmpty(notificationStaff.FcmToken))
                                {
                                    // Define title, body, and data for the notification
                                    var title = "Nhân viên chịu trách nhiệm";
                                    var body = $"Bạn đã được chọn ngẫu nhiên làm người chịu trách nhiệm cho mã đơn {assignment.BookingId}.";
                                    var fcmToken = notificationStaff.FcmToken;
                                    var data = new Dictionary<string, string>
                                    {
                                        { "bookingId", booking.Id.ToString() },
                                        { "status", booking.Status.ToString() },
                                        { "message", "Bạn đã được chọn là người chọn trách nhiệm." },
                                        { "user", assignment.UserId.ToString() }
                                    };

                                    // Send notification to Firebase
                                    await firebaseServices.SendNotificationAsync(title, body, fcmToken, data);
                                }

                                break;

                            case var type when type == NotificationEnums.PAYMENT_BY_CASH.ToString():
                                var assignmentDriver = await unitOfWork.AssignmentsRepository.GetAssignmentByBookingIdAndStaffTypeAndFCMTokenAsync(message.BookingId, message.StaffType);
                                var notificationStaffDriver = assignmentDriver.User.Notifications.FirstOrDefault(n => n.FcmToken != null);
                                if (notificationStaffDriver != null && !string.IsNullOrEmpty(notificationStaffDriver.FcmToken))
                                {
                                    var title = "Khách hàng yêu cầu thanh toán bằng tiền mặt";
                                    var body = $"Người dùng đã chọn thanh toán bằng tiền mặt cho đơn hàng {booking.Id}.";
                                    var fcmToken = notificationStaffDriver.FcmToken;
                                    var data = new Dictionary<string, string>
                                    {
                                        { "bookingId", booking.Id.ToString() },
                                        { "status", booking.Status.ToString() },
                                        { "message", "Người dùng đã chọn thanh toán bằng tiền mặt." }
                                    };

                                    // Send notification to Firebase
                                    await firebaseServices.SendNotificationAsync(title, body, fcmToken, data);
                                }
                                break;

                            default:
                                Console.WriteLine($"Unknown notification type: {message.Type}");
                                break;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error processing booking notification for message {e.Message}", message);
                throw;
            }
            Console.WriteLine($"Received message to queue movemate.notification_update_booking: {message}");
        }
    }
}
