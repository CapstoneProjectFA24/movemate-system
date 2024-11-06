using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.Utils;
using static Grpc.Core.Metadata;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker;

public class AssignReviewWorker
{
    private readonly ILogger<AssignReviewWorker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    /*private readonly IFirebaseServices _firebaseServices;*/

    public AssignReviewWorker(ILogger<AssignReviewWorker> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

    }

    [Consumer("movemate.booking_assign_review")]
    public async Task HandleMessage(int message)
    {
        //await Task.Delay(TimeSpan.FromSeconds(3));
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                var firebaseServices = scope.ServiceProvider.GetRequiredService<IFirebaseServices>();
                
                var booking = await unitOfWork.BookingRepository.GetByIdAsync(message);

                string redisKey = DateUtil.GetKeyReview();
                var reviewerId = await redisService.DequeueAsync<int>(redisKey);
                var date = DateUtil.GetShard(booking!.BookingAt);
                if (reviewerId == 0)
                {
                    var checkExistQueue = await redisService.KeyExistsQueueAsync(redisKey);
                    if (checkExistQueue)
                    {
                        throw new Exception($"Queue {redisKey} already exists but is empty");
                    }
                    else
                    {
                        List<int> listReviewer = await unitOfWork.UserRepository.FindAllUserByRoleIdAsync(2);
                        await redisService.EnqueueMultipleAsync(redisKey, listReviewer);

                        throw new Exception($"Queue {redisKey} has been added");
                    }
                }

                var startDate = booking!.BookingAt;
                var isResponsible = false;
                if (booking.IsReviewOnline == false)
                {
                    startDate = booking!.ReviewAt;
                    isResponsible = true;
                }
                
                var reviewer = new Assignment()
                {
                    BookingId = message,
                    Status = AssignmentStatusEnums.ASSIGNED.ToString(),
                    UserId = reviewerId,
                    StaffType = RoleEnums.REVIEWER.ToString(),
                    IsResponsible = isResponsible,
                    StartDate = startDate,
                };
                var scheduleBooking = await unitOfWork.ScheduleBookingRepository.GetByShard(date);

                if (scheduleBooking == null)
                {
                    scheduleBooking = new ScheduleBooking()
                    {
                        Shard = date,
                        IsActived = true,
                         
                    };
                    await unitOfWork.ScheduleBookingRepository.SaveOrUpdateAsync(scheduleBooking);
                    
                }
                scheduleBooking!.Assignments.Add(reviewer);
                
                booking.Assignments.Add(reviewer);

                booking.Status = AssignmentStatusEnums.ASSIGNED.ToString();
                await unitOfWork.BookingRepository.SaveOrUpdateAsync(booking);

                unitOfWork.Save();
                firebaseServices.SaveBooking(booking, booking.Id, "bookings");
                redisService.EnqueueAsync(redisKey, reviewerId);

                Console.WriteLine($"Booking info: {booking}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing booking review for message {Message}", message);
            throw;
        }

        Console.WriteLine($"Received message: {message}");
    }
}