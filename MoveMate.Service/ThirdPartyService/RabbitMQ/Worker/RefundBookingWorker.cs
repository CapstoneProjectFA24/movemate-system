using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;
using MoveMate.Service.ThirdPartyService.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker
{
    public class RefundBookingWorker
    {
        private readonly ILogger<RefundBookingWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RefundBookingWorker(ILogger<RefundBookingWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        [Consumer("movemate.refund_booking")]
        public async Task HandleMessage(int message)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // var
                var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var firebaseServices = scope.ServiceProvider.GetRequiredService<IFirebaseServices>();
                var producer = scope.ServiceProvider.GetRequiredService<IMessageProducer>();
                // check booking
                var existingBooking = await unitOfWork.BookingRepository.GetByIdAsync(message);
                if (existingBooking == null)
                {
                    throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
                }

                existingBooking.RefundAt = DateTime.Now;
                if (existingBooking.IsDeposited == true)
                {
                    var timeDifference = (existingBooking.BookingAt.Value - existingBooking.RefundAt.Value).TotalHours;
                    double refundPercentage;
                    if (timeDifference >= 48)
                    {
                        refundPercentage = 1.0; // 100% refund
                    }
                    else if (timeDifference >= 24 && timeDifference < 48)
                    {
                        refundPercentage = 0.7; // 70% refund
                    }
                    else if (timeDifference >= 1 && timeDifference < 24)
                    {
                        refundPercentage = 0.5; // 50% refund
                    }
                    else
                    {
                        refundPercentage = 0.0; // No refund
                        existingBooking.RefundAt = null;
                    }
                    existingBooking.TotalRefund = (double)(existingBooking.Deposit * refundPercentage);
                    existingBooking.Status = BookingEnums.REFUNDING.ToString();
                    var tracker = new BookingTracker();
                    tracker.BookingId = existingBooking.Id;
                    tracker.Type = TrackerEnums.REFUND.ToString();
                    tracker.Status = StatusTrackerEnums.WAITING.ToString();
                    tracker.EstimatedAmount = existingBooking.TotalRefund;
                    tracker.Time = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");
                    await unitOfWork.BookingTrackerRepository.AddAsync(tracker);
                }

                if (existingBooking.TotalRefund == 0 || !existingBooking.TotalRefund.HasValue)
                {
                    existingBooking.Status = BookingEnums.COMPLETED.ToString();
                }

                await unitOfWork.BookingRepository.SaveOrUpdateAsync(existingBooking);
                await unitOfWork.SaveChangesAsync();
                producer.SendingMessage("movemate.push_to_firebase", existingBooking.Id);
                Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
