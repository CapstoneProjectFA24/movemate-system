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
        //await Task.Delay(TimeSpan.FromSeconds(1));
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                var firebaseServices = scope.ServiceProvider.GetRequiredService<IFirebaseServices>();
                var producer = scope.ServiceProvider.GetRequiredService<IMessageProducer>();

                var booking = await unitOfWork.BookingRepository.GetByIdAsync(message);
                
                var schedule = await unitOfWork.ScheduleWorkingRepository.GetScheduleByBookingAtAsync(booking.BookingAt.Value);
                
                string redisKey = DateUtil.GetKeyReview(schedule.GroupId.Value, schedule.Id);
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
                        List<int> listReviewer = await unitOfWork.UserRepository.FindAllUserByRoleIdAndGroupIdAsync(2,  schedule.GroupId.Value);
                        if (listReviewer.Count() <= 0)
                        {
                            listReviewer.Add(2);
                        }
                        await redisService.EnqueueMultipleAsync(redisKey, listReviewer);

                        throw new Exception($"Queue {redisKey} has been added");
                    }
                }

                var startDate = booking!.BookingAt;
                var isResponsible = true;
                
                var reviewer = new Assignment()
                {
                    BookingId = message,
                    Status = AssignmentStatusEnums.ASSIGNED.ToString(),
                    UserId = reviewerId,
                    StaffType = RoleEnums.REVIEWER.ToString(),
                    IsResponsible = isResponsible,
                    StartDate = startDate,
                };

                await unitOfWork.AssignmentsRepository.SaveOrUpdateAsync(reviewer);
                
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

                booking = await unitOfWork.BookingRepository.GetByIdAsyncV1(message,
                    includeProperties:
                    "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments");
                await firebaseServices.SaveBooking(booking, message, "bookings");

                producer.SendingMessage("movemate.push_to_firebase_local", booking.Id);
                
                //booking = await unitOfWork.BookingRepository.GetByIdAsyncV1(booking.Id,
                //    includeProperties:
                //    "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments");
                
                //firebaseServices.SaveBooking(booking, booking.Id, "bookings");
                redisService.EnqueueAsync(redisKey, reviewerId);

                Console.WriteLine($"Booking info: {booking.Id}");
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