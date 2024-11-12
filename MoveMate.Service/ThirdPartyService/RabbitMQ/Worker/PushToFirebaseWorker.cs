using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
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
    public class PushToFirebaseWorker
    {
        private readonly ILogger<PushToFirebaseWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PushToFirebaseWorker(ILogger<PushToFirebaseWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        [Consumer("movemate.push_to_firebase_local")]
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


                    var booking = await unitOfWork.BookingRepository.GetByIdAsyncV1(message,
                    includeProperties:
                   "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments");
                    await firebaseServices.SaveBooking(booking, booking.Id, "bookings");
                }



            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing booking review for message {Message}", message);
                throw;
            }

            Console.WriteLine($"movemate.push_to_firebase_local: {message}");

        }
    }
}
