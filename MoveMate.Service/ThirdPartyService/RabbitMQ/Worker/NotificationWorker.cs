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
using FirebaseAdmin.Messaging;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker
{
    public class NotificationWorker
    {
        private readonly ILogger<NotificationWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private static readonly Dictionary<string, string> BookingStatusNames = new Dictionary<string, string>
    {
        { BookingEnums.PENDING.ToString(), "Đang xử lý yêu cầu" },
        { BookingEnums.DEPOSITING.ToString(), "Vui lòng thanh toán để tiến hành dịch vụ" },
        { BookingEnums.ASSIGNED.ToString(), "Nhân viên đang xem xét yêu cầu của bạn" },
        { BookingEnums.REVIEWING.ToString(), "Đã có đề xuất dịch vụ mới" },
        { BookingEnums.REVIEWED.ToString(), "Vui lòng xác nhận đề xuất dịch vụ" },
        { BookingEnums.COMING.ToString(), "Đội ngũ vận chuyển đang trên đường đến" },
        { BookingEnums.WAITING.ToString(), "Vui lòng xác nhận lịch khảo sát" },
        { BookingEnums.IN_PROGRESS.ToString(), "Đang thực hiện vận chuyển" },
        { BookingEnums.COMPLETED.ToString(), "Dịch vụ đã hoàn thành" },
        { BookingEnums.PAUSED.ToString(), "Đã có đề xuất dịch vụ mới" },
        { BookingEnums.CANCEL.ToString(), "Đơn hàng đã bị hủy" },
        { BookingEnums.REFUNDING.ToString(), "Đang chờ hoàn tiền" }
    };
        public NotificationWorker(ILogger<NotificationWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        private string GetBookingStatusName(string status)
        {
            if (BookingStatusNames.ContainsKey(status))
            {
                return BookingStatusNames[status];
            }
            return "Trạng thái không xác định"; 
        }
        [Consumer("movemate.notification_update_booking_local")]
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
                    var statusName = GetBookingStatusName(booking.Status);

                    var notificationUser =
                        await unitOfWork.NotificationRepository.GetByUserIdAsync((int)booking.UserId);
                    if (notificationUser == null)
                    {
                        throw new Exception($"Can't send notification to user with Id {booking.UserId} - {booking.Status}");
                    }

                    //if (booking.Status == BookingEnums.)

                    var assignments = await unitOfWork.AssignmentsRepository.GetAssignmentByUserIdAndFCMTokenAsync(booking.Id);
                    if (assignments == null)
                    {
                        throw new Exception($"Not found assignment with booking Id {booking.UserId} - {booking.Status}");
                    }

                    if (!string.IsNullOrEmpty(notificationUser.FcmToken))
                    {
                        // Define title, body, and data for the notification

                        var redisKey = message + "-" + "user" + "-" + booking.UserId + "-" + booking.Status;

                        var isExistQueue = await redisService.KeyExistsAsync(redisKey);

                        if (!isExistQueue)
                        {
                            redisService.SetData(redisKey, message);

                            var title = $"Đơn hàng đã được cập nhật - {statusName}.";
                            var body = $"Đơn hàng với mã đơn {booking.Id} đã cập nhật trạng thái mới - {statusName}.";
                            var fcmToken = notificationUser.FcmToken;
                            var data = new Dictionary<string, string>
                            {
                                { "bookingId", booking.Id.ToString() },
                                { "status", booking.Status.ToString()  },
                                { "message", "Đơn hàng đã được thay đổi trạng thái thành công." }
                            };

                            // Send notification to Firebase
                            await firebaseServices.SendNotificationAsync(title, body, fcmToken, data);
                        }


                    }
                    foreach (var assignment in assignments)
                    {

                        var redisKey = message + "-" + "staff" + "-" + assignment.Id + "-" + booking.Status;

                        var isExistQueue = await redisService.KeyExistsAsync(redisKey);

                        if (!isExistQueue)
                        {
                            redisService.SetData(redisKey, message);
                            // Retrieve the staff's notification token
                            var notificationStaff =
                               assignment.User.Notifications.FirstOrDefault(n => n.FcmToken != null);

                            if (notificationStaff != null && !string.IsNullOrEmpty(notificationStaff.FcmToken))
                            {
                                var titleAssignment = $"Đơn hàng đã được cập nhật - {statusName}.";
                                var bodyAssignment = $"Đơn hàng bạn phụ trách với mã đơn {booking.Id} đã cập nhật - {statusName}.";
                                var fcmTokenAssignment = notificationStaff.FcmToken;
                                var dataAssignment = new Dictionary<string, string>
                                {
                                    { "bookingId", booking.Id.ToString() },
                                    { "status", booking.Status.ToString() },
                                    { "message", "Đơn hàng đã được thay đổi trạng thái thành công." }
                                };

                                // Send notification for each assignment to staff
                                await firebaseServices.SendNotificationAsync(titleAssignment, bodyAssignment,
                                    fcmTokenAssignment,
                                    dataAssignment);
                            }

                        }
                    }
                }
            }
            /* catch (FirebaseMessagingException ex)
             {
                 _logger.LogError(ex, "FirebaseMessagingException processing booking notification for message {Message}", message);

                 return;
             }*/
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing booking notification for message {Message}", message);
                throw;
            }

            Console.WriteLine($"Received message to queue movemate.notification_update_booking: {message}");
        }
    }
}