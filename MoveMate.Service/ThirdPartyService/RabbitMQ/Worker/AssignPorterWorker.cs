using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.GoongMap;
using MoveMate.Service.ThirdPartyService.GoongMap.Models;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.Utils;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker;

public class AssignPorterWorker
{
    private readonly ILogger<AssignPorterWorker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AssignPorterWorker(IServiceScopeFactory serviceScopeFactory, ILogger<AssignPorterWorker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    [Consumer("movemate.booking_assign_porter_local")]
    public async Task HandleMessage(int message)
    {
        Console.WriteLine("movemate.booking_assign_porter_local");
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // var
                var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                var firebaseServices = scope.ServiceProvider.GetRequiredService<IFirebaseServices>();

                // check booking
                var existingBooking = await unitOfWork.BookingRepository.GetByIdAsync(message);
                if (existingBooking == null)
                {
                    throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
                }
                
                var bookingDetail =
                    await unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(
                        TypeServiceEnums.PORTER.ToString(), message);

                if (bookingDetail == null)
                {
                    throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
                }

                var endTime = existingBooking.BookingAt!.Value.AddHours(existingBooking.EstimatedDeliveryTime!.Value);

                var date = DateUtil.GetShard(existingBooking.BookingAt);

                var redisKey = DateUtil.GetKeyPorter(existingBooking.BookingAt);
                var redisKeyV2 = DateUtil.GetKeyPorterV2(existingBooking.BookingAt);

                var checkExistQueue = await redisService.KeyExistsQueueAsync(redisKeyV2);

                var scheduleBooking = await unitOfWork.ScheduleBookingRepository.GetByShard(date);
                var listAssignments = new List<Assignment>();

                var keyAssigned = DateUtil.GetKeyPorterBooking(existingBooking.BookingAt, existingBooking.Id);
                var timeExpiryRedisQueue = DateUtil.TimeUntilEndOfDay(existingBooking.BookingAt!.Value);

                redisService.SetData(keyAssigned, existingBooking.Id, timeExpiryRedisQueue);
                
                if (checkExistQueue == false)
                {
                    //var timeExpiryRedisQueue = DateUtil.TimeUntilEndOfDay(existingBooking.BookingAt!.Value);

                    var porterIds =
                        await unitOfWork.UserRepository.FindAllUserByRoleIdAsync(5);

                    await redisService.EnqueueMultipleAsync(redisKey, porterIds, timeExpiryRedisQueue);
                    await redisService.EnqueueMultipleAsync(redisKeyV2, porterIds, timeExpiryRedisQueue);

                    // check StaffNumber in booking and StaffNumber in system
                    var porterNumberBooking = existingBooking.PorterNumber!.Value;
                    if (porterNumberBooking > porterIds.Count)
                    {
                        var user = await unitOfWork.UserRepository.GetManagerAsync();
                        bookingDetail.Status = BookingDetailStatusEnums.WAITING.ToString();
                        await unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetail);

                        // danh tag failedm, noti to manager
                        var notification = new Notification
                        {
                            UserId = user.Id,
                            SentFrom = "System",
                            Receive = user.Name,
                            Name = "porter Shortage Notification",
                            Description =
                                $"Only {porterIds.Count} porters available for {porterNumberBooking} bookings.",
                            Topic = "porterAssignment",
                            IsRead = false
                        };

                        // Save the notification to Firestore
                        await firebaseServices.SaveMailManager(notification, notification.Id, "reports");
                    }

                    await AssignPortersToBooking(
                        message,
                        redisKey,
                        existingBooking.BookingAt.Value,
                        existingBooking.PorterNumber!.Value,
                        existingBooking.EstimatedDeliveryTime ?? 3,
                        listAssignments,
                        redisService,
                        scheduleBooking!.Id,
                        bookingDetail
                    );

                    await unitOfWork.AssignmentsRepository.SaveOrUpdateRangeAsync(listAssignments);
                    unitOfWork.Save();

                    var bookingFirebase = await unitOfWork.BookingRepository.GetByIdAsyncV1(existingBooking.Id,
                        includeProperties:
                        "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments");

                    await firebaseServices.SaveBooking(bookingFirebase, bookingFirebase.Id, "bookings");
                }
                else
                {
                    int countporterNumberBooking = existingBooking.PorterNumber!.Value;
                    var countporter = await redisService.CheckQueueCountAsync(redisKey);
                    if (countporter > countporterNumberBooking)
                    {
                        await AssignPortersToBooking(
                            message,
                            redisKey,
                            existingBooking.BookingAt!.Value,
                            existingBooking.PorterNumber.Value,
                            existingBooking.EstimatedDeliveryTime ?? 3,
                            listAssignments,
                            redisService,
                            scheduleBooking!.Id,
                            bookingDetail
                        );
                        await unitOfWork.AssignmentsRepository.SaveOrUpdateRangeAsync(listAssignments);
                        unitOfWork.Save();

                        var bookingFirebase = await unitOfWork.BookingRepository.GetByIdAsyncV1(existingBooking.Id,
                            includeProperties:
                            "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments");

                        await firebaseServices.SaveBooking(bookingFirebase, bookingFirebase.Id, "bookings");
                    }
                    else
                    {
                        // nếu mà list countporter < existingBooking.porterNumber thì
                        //  
                        // 1H
                        var assignedPortersAvailable1Hours =
                            await unitOfWork.AssignmentsRepository.GetPortersAvailableWithExtendedAsync(
                                existingBooking.BookingAt.Value, endTime, scheduleBooking.Id);

                        // 2H
                        var assignedPortersAvailable2Hours =
                            await unitOfWork.AssignmentsRepository.GetPortersAvailableWithExtendedAsync(
                                existingBooking.BookingAt.Value, endTime, scheduleBooking.Id, 1, 1);
                        // OTHER
                        var assignedPortersAvailableOther =
                            await unitOfWork.AssignmentsRepository.GetPorterAvailableWithOverlapAsync(
                                existingBooking.BookingAt.Value, endTime, scheduleBooking.Id, 2);

                        var countRemaining = (int)countporter + assignedPortersAvailable1Hours.Count() +
                                             assignedPortersAvailable2Hours.Count() +
                                             assignedPortersAvailableOther.Count();
                        if (countRemaining >= countporterNumberBooking)
                        {
                            if (countporter > 0)
                            {
                                countRemaining -= (int)countporter;

                                await AssignPortersToBooking(
                                    message,
                                    redisKey,
                                    existingBooking.BookingAt!.Value,
                                    countporterNumberBooking,
                                    existingBooking.EstimatedDeliveryTime ?? 3,
                                    listAssignments,
                                    redisService,
                                    scheduleBooking!.Id,
                                    bookingDetail
                                );
                                countporterNumberBooking -= (int)countporter;
                            }

                            var googleMapsService = scope.ServiceProvider.GetRequiredService<IGoogleMapsService>();
                            // xử lý
                            await AllocatePortersToBookingAsync(
                                existingBooking,
                                existingBooking.BookingAt!.Value,
                                countporterNumberBooking,
                                existingBooking.EstimatedDeliveryTime ?? 3,
                                listAssignments,
                                assignedPortersAvailable1Hours,
                                assignedPortersAvailable2Hours,
                                assignedPortersAvailableOther,
                                scheduleBooking!.Id,
                                googleMapsService,
                                bookingDetail
                            );
                            await unitOfWork.AssignmentsRepository.SaveOrUpdateRangeAsync(listAssignments);
                            unitOfWork.Save();

                            var bookingFirebase = await unitOfWork.BookingRepository.GetByIdAsyncV1(existingBooking.Id,
                                includeProperties:
                                "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments");

                            await firebaseServices.SaveBooking(bookingFirebase, bookingFirebase.Id, "bookings");
                        }
                        else
                        {
                            var user = await unitOfWork.UserRepository.GetManagerAsync();
                            bookingDetail.Status = BookingDetailStatusEnums.WAITING.ToString();
                            await unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetail);

                            // danh tag failedm, noti to manager
                            var notification = new Notification
                            {
                                UserId = user.Id,
                                SentFrom = "System",
                                Receive = user.Name,
                                Name = "porter Shortage Notification",
                                Description =
                                    $"Only {countRemaining} porters available for {countporterNumberBooking} bookings.",
                                Topic = "porterAssignment",
                                IsRead = false
                            };

                            // Save the notification to Firestore
                            await firebaseServices.SaveMailManager(notification, notification.Id, "reports");
                        }

                        //đánh tag faild cần reviewer can thiệp
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("AssignporterWorker - " + e.Message);
            throw;
        }
    }

    /// <summary>
    /// Assigns Staffs to a booking by dequeuing Staffs IDs from Redis, creating assignments,
    /// and saving them in the schedule booking repository.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to assign Staffs to.</param>
    /// <param name="redisKey">The Redis key used to dequeue available Staffs IDs.</param>
    /// <param name="startTime">The start time for the assignment schedule.</param>
    /// <param name="porterCount">The number of Staffs to assign to the booking.</param>
    /// <param name="estimatedDeliveryTime">The estimated time in hours for delivery.</param>
    /// <param name="listAssignments">The list of assignments where new porter assignments will be added.</param>
    /// <param name="redisService">Service used for interacting with Redis.</param>
    /// <param name="scheduleBookingId">The ID of the scheduleBooking to assign porters to.</param>
    /// <param name="bookingDetail">The ID of the bookingDetail</param>
    /// <returns>A task that represents the asynchronous operation of assigning porters and saving schedule data.</returns>
    private async Task<List<Assignment>> AssignPortersToBooking(int bookingId, string redisKey, DateTime startTime,
        int porterCount,
        double estimatedDeliveryTime, List<Assignment> listAssignments,
        IRedisService redisService, int scheduleBookingId, BookingDetail bookingDetail)
    {
        for (int i = 0; i < porterCount; i++)
        {
            var staffId = await redisService.DequeueAsync<int>(redisKey);
            var endTime = startTime.AddHours(estimatedDeliveryTime);
            var newAssignmentporter = new Assignment()
            {
                BookingId = bookingId,
                StaffType = RoleEnums.PORTER.ToString(),
                Status = AssignmentStatusEnums.WAITING.ToString(),
                UserId = staffId,
                StartDate = startTime,
                EndDate = endTime,
                IsResponsible = false,
                ScheduleBookingId = scheduleBookingId,
                BookingDetailsId = bookingDetail.Id
            };

            listAssignments.Add(newAssignmentporter);
        }

        return listAssignments;
    }

    /// <summary>
    /// Assigns available porters to a booking based on proximity and estimated travel time.
    /// Calculates a rating for each porter based on travel duration, then selects the most suitable porters.
    /// </summary>
    /// <param name="booking">The current booking that requires porter assignment.</param>
    /// <param name="startTime">The start time for the booking's assigned porters.</param>
    /// <param name="porterCount">The number of porters needed for this booking.</param>
    /// <param name="estimatedDeliveryTime">The estimated time needed to complete the delivery for this booking.</param>
    /// <param name="listAssignments">The list of Assignment records to be updated and saved to the database.</param>
    /// <param name="listporterAvailable1Hours">The list of porters available within 1 hour.</param>
    /// <param name="listporterAvailable2Hours">The list of porters available within 2 hours.</param>
    /// <param name="listporterAvailableOthers">The list of porters available beyond 2 hours.</param>
    /// <param name="unitOfWork">The UnitOfWork instance used to save data to the database.</param>
    /// <param name="scheduleBookingId">The schedule booking ID for identifying porter details.</param>
    /// <param name="googleMapsService">The Google Maps service for calculating distance and travel time between locations.</param>
    /// <param name="bookingDetail"></param>
    private async Task<List<Assignment>> AllocatePortersToBookingAsync(
        Booking booking,
        DateTime startTime,
        int porterCount,
        double estimatedDeliveryTime,
        List<Assignment> listAssignments,
        List<Assignment> listporterAvailable1Hours,
        List<Assignment> listporterAvailable2Hours,
        List<Assignment> listporterAvailableOthers,
        int scheduleBookingId,
        IGoogleMapsService googleMapsService,
        BookingDetail bookingDetail)
    {
        var staffDistances = new List<(Assignment Staff, double rate)>();

        // 1 H
        foreach (var assignment in listporterAvailable1Hours)
        {
            var rate = 2;
            GoogleMapDTO? googleMapDto = null;
            if (assignment.StartDate >= booking.BookingAt!.Value)
            {
                googleMapDto =
                    await googleMapsService.GetDistanceAndDuration(assignment.Booking!.PickupPoint!,
                        booking.DeliveryPoint!);
            }
            else
            {
                googleMapDto =
                    await googleMapsService.GetDistanceAndDuration(assignment.Booking!.DeliveryPoint!,
                        booking.PickupPoint!);
            }

            var distance = googleMapDto.Distance.Value;
            var duration = googleMapDto.Duration.Value;
            rate = rate * distance / duration;
            staffDistances.Add((assignment, rate));
        }

        // 2 H
        foreach (var assignment in listporterAvailable2Hours)
        {
            var rate = 1.5;
            GoogleMapDTO? googleMapDto = null;
            if (assignment.StartDate >= booking.BookingAt!.Value)
            {
                googleMapDto =
                    await googleMapsService.GetDistanceAndDuration(assignment.Booking!.PickupPoint!,
                        booking.DeliveryPoint!);
            }
            else
            {
                googleMapDto =
                    await googleMapsService.GetDistanceAndDuration(assignment.Booking!.DeliveryPoint!,
                        booking.PickupPoint!);
            }

            var distance = googleMapDto.Distance.Value;
            var duration = googleMapDto.Duration.Value;
            rate = rate * distance / duration;
            staffDistances.Add((assignment, rate));
        }

        // Other
        foreach (var assignment in listporterAvailableOthers)
        {
            var rate = 1;
            GoogleMapDTO? googleMapDto = null;
            if (assignment.StartDate >= booking.BookingAt!.Value)
            {
                googleMapDto =
                    await googleMapsService.GetDistanceAndDuration(assignment.Booking!.PickupPoint!,
                        booking.DeliveryPoint!);
            }
            else
            {
                googleMapDto =
                    await googleMapsService.GetDistanceAndDuration(assignment.Booking!.DeliveryPoint!,
                        booking.PickupPoint!);
            }

            var distance = googleMapDto.Distance.Value;
            var duration = googleMapDto.Duration.Value;
            rate = rate * distance / duration;
            staffDistances.Add((assignment, rate));
        }

        staffDistances = staffDistances
            .GroupBy(d => d.Staff.UserId)
            .Select(g => g.First())
            .ToList();

        var closestporters = staffDistances
            .OrderByDescending(x => x.rate)
            .Take(porterCount)
            .ToList();

        foreach (var porter in closestporters)
        {
            var assignment = porter.Staff;
            var staffId = assignment.UserId!.Value;
            var endTime = startTime.AddHours(estimatedDeliveryTime);

            listAssignments.Add(new Assignment
            {
                BookingId = booking.Id,
                StaffType = RoleEnums.PORTER.ToString(),
                Status = AssignmentStatusEnums.WAITING.ToString(),
                UserId = staffId,
                StartDate = startTime,
                EndDate = endTime,
                IsResponsible = false,
                ScheduleBookingId = scheduleBookingId,
                BookingDetailsId = bookingDetail.Id
            });
        }

        return listAssignments;
    }
}