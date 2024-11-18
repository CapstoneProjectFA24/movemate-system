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

public class AssignDriverWorker
{
    private readonly ILogger<AssignDriverWorker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AssignDriverWorker(ILogger<AssignDriverWorker> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

/*
Auto-Assign Driver Workflow:

1. Shard Check:
   - If the shard does not exist:
     - Create a new shard.
     - Find a list of drivers with the appropriate truck type and schedule.
     - Additional check: Verify if the driver is currently handling a high-priority task.
     - Add to Redis queue _booking_at.
     - Add to the assignment list.

   - If the shard exists:
     - Check the Redis queue _booking_at:
       - If the queue has elements:
          - Dequeue and add to assignment.
          -* Performance history check*:
            - If a driver has a high refusal rate, lower their assignment priority.

       - If the queue is empty or insufficient:
         - Check driver schedule booking details:
           - Condition check:
             - endTime of booking_at > startTime of schedule or startTime of booking < endTime of schedule.
           - If there is available time:
             - If count > 1:
               - Choose schedule by location.
               - Calculate rate for each schedule:
                 - Consider task type (e.g., hazardous materials) to adjust rate.
               - Check if schedule ends within 1-2 hours:
                 - Rate calculations:
                   - Within 1 hour: rate += 1.
                   - Within 2 hours: rate += 0.5.
               - Compare and choose driver:
                 - Calculate rate based on distance: rate = rate / distance.
                 - ! Additional: Calculate environmental factors (weather, traffic) into the rate.
             - If count = 1: Select immediately.
             - If count < 1: Tag "auto-assign failed, needs review intervention".
           - If no free schedule meets conditions, tag "auto-assign failed, needs review intervention".

2. Special Case Additions:
   - Emergency Case - when an emergency arises:
     - For an urgent request, the system pauses other auto-assignments and prioritizes the nearest driver.
   - Backup when lacking drivers:
     - If no suitable driver is found, automatically place a standby order and seek a manager.
   - Alternative driver selection:
     - If a driver has conflicting schedules, put them on a standby list for reassignment if the primary driver is unavailable.
   - Management alert:
     - Alert management if the failure rate for assignment is too high or no suitable driver is found.
   -* Ranking-based allocation - requires a scoring formula:
     - Calculate ranking score based on performance, reliability, and customer feedback to prioritize high-quality drivers for complex tasks.

3. Process Monitoring and Improvement:
   - ! Periodic analysis:
     - Conduct periodic analysis to evaluate the effectiveness of the driver assignment process and adjust parameters and rules accordingly.
   - ! Driver feedback:
     - Create a feedback channel for drivers to provide input on the assignment process and make improvements based on their opinions.
   - ! Technology updates:
     - Explore and apply new technologies such as AI and machine learning to automate and optimize the driver assignment process.
*/


    /// <summary>
    /// Automatically assigns drivers to a booking based on the booking ID received in the message.
    /// </summary>
    /// <param name="message">An integer representing the booking ID to which drivers need to be assigned.</param>
    /// <exception cref="NotFoundException">Thrown when the booking ID does not exist or cannot be found in the system.</exception>
    [Consumer("movemate.booking_assign_driver_local")]
    public async Task HandleMessage(int message)
    {
        // Implementation of driver assignment logic will go here.
        // The method should use the booking ID (message) to find the relevant booking,
        // check available drivers, and assign them as necessary.
        // This method is marked as asynchronous to support non-blocking operations.

        // Example logic:
        // 1. Retrieve the booking details by booking ID (message).
        // 2. Check the availability of drivers based on the booking requirements.
        // 3. Assign a driver to the booking if an appropriate driver is found.
        // 4. Save the updated booking details.
        await Task.Delay(100);
        Console.WriteLine("movemate.booking_assign_driver_local");
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // var
                var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                var firebaseServices = scope.ServiceProvider.GetRequiredService<IFirebaseServices>();
                
                // check booking
                var existingBooking = await unitOfWork.BookingRepository.GetByIdAsync(message);
                if (existingBooking == null)
                {
                    throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
                }

                var bookingDetailTruck =
                    await unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(
                        TypeServiceEnums.TRUCK.ToString(), message);

                if (bookingDetailTruck == null)
                {
                    throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
                }
                
                var schedule = await unitOfWork.ScheduleWorkingRepository.GetScheduleByBookingAtAsync(existingBooking.BookingAt.Value);
                
                var endTime = existingBooking.BookingAt!.Value.AddHours(existingBooking.EstimatedDeliveryTime!.Value);

                var date = DateUtil.GetShard(existingBooking.BookingAt);
                
                var redisKey = DateUtil.GetKeyDriver(existingBooking.BookingAt, existingBooking.TruckNumber!.Value, schedule.GroupId.Value, schedule.Id);
                var redisKeyV2 = DateUtil.GetKeyDriverV2(existingBooking.BookingAt, existingBooking.TruckNumber!.Value, schedule.GroupId.Value, schedule.Id);

                var checkExistQueue = await redisService.KeyExistsQueueAsync(redisKeyV2);

                var scheduleBooking = await unitOfWork.ScheduleBookingRepository.GetByShard(date);
                var listAssignments = new List<Assignment>();

                var keyAssigned = DateUtil.GetKeyDriverBooking(existingBooking.BookingAt, existingBooking.Id);
                var timeExpiryRedisQueue = DateUtil.TimeUntilEndOfDay(existingBooking.BookingAt!.Value);

                redisService.SetData(keyAssigned, existingBooking.Id, timeExpiryRedisQueue);
                
                if (checkExistQueue == false)
                {
                    var driverIds = 
                        await unitOfWork.UserRepository.GetUsersWithTruckCategoryIdAsync(existingBooking!.TruckNumber!
                            .Value, schedule.GroupId.Value);
                    
                    await redisService.EnqueueMultipleAsync(redisKey, driverIds, timeExpiryRedisQueue);
                    await redisService.EnqueueMultipleAsync(redisKeyV2, driverIds, timeExpiryRedisQueue);

                    // check DriverNumber in booking and driver in system
                    var driverNumberBooking = existingBooking.DriverNumber!.Value;
                    if (driverNumberBooking > driverIds.Count)
                    {
                        var user = await unitOfWork.UserRepository.GetManagerAsync();
                        bookingDetailTruck.Status = BookingDetailStatusEnums.WAITING.ToString();
                        await unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetailTruck);
                        
                        var notification = new Notification
                        {
                           
                            UserId = user.Id,  
                            SentFrom = "System",  
                            Receive = user.Name,    
                            Name = $"Driver Shortage Notification in {existingBooking.Id}",
                            Description = $"Only {driverIds.Count} drivers available for {driverNumberBooking} bookings.",
                            Topic = "DriverAssignment",                         
                            IsRead = false
                        };

                        await unitOfWork.NotificationRepository.SaveOrUpdateAsync(notification);
                        // Save the notification to Firestore
                        await firebaseServices.SaveMailManager(notification, existingBooking.Id, "reports");

                    }

                    await AssignDriversToBooking(
                        message,
                        redisKey,
                        existingBooking.BookingAt.Value,
                        existingBooking.DriverNumber!.Value,
                        existingBooking.EstimatedDeliveryTime ?? 3,
                        listAssignments,
                        redisService,
                        unitOfWork,
                        scheduleBooking!.Id,
                        bookingDetailTruck
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
                    int countDriverNumberBooking = existingBooking.DriverNumber!.Value;
                    var countDriver = await redisService.CheckQueueCountAsync(redisKey);
                    if (countDriver >= countDriverNumberBooking)
                    {
                        await AssignDriversToBooking(
                            message,
                            redisKey,
                            existingBooking.BookingAt!.Value,
                            existingBooking.DriverNumber.Value,
                            existingBooking.EstimatedDeliveryTime ?? 3,
                            listAssignments,
                            redisService,
                            unitOfWork,
                            scheduleBooking!.Id,
                            bookingDetailTruck
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
                        // nếu mà list countDriver < existingBooking.DriverNumber thì
                        //  
                        // 1H
                        var assignedDriverAvailable1Hours =
                            await unitOfWork.AssignmentsRepository.GetDriverByGroupAvailableWithExtendedAsync(
                                existingBooking.BookingAt.Value, endTime, scheduleBooking.Id,
                                existingBooking.TruckNumber!.Value, schedule.GroupId);

                        // 2H
                        var assignedDriverAvailable2Hours =
                            await unitOfWork.AssignmentsRepository.GetDriverByGroupAvailableWithExtendedAsync(
                                existingBooking.BookingAt.Value, endTime, scheduleBooking.Id,
                                existingBooking.TruckNumber!.Value, schedule.GroupId, 1, 1);
                        // OTHER
                        var assignedDriverAvailableOther =
                            await unitOfWork.AssignmentsRepository.GetDriverByGroupAvailableWithOverlapAsync(
                                existingBooking.BookingAt.Value, endTime, scheduleBooking.Id,
                                existingBooking.TruckNumber!.Value, schedule.GroupId, 2);

                        var countRemaining = (int)countDriver + assignedDriverAvailable1Hours.Count() +
                                             assignedDriverAvailable2Hours.Count() +
                                             assignedDriverAvailableOther.Count();
                        if (countRemaining >= countDriverNumberBooking)
                        {
                            if (countDriver > 0)
                            {
                                countRemaining -= (int)countDriver;
                                
                                await AssignDriversToBooking(
                                    message,
                                    redisKey,
                                    existingBooking.BookingAt!.Value,
                                    countDriverNumberBooking,
                                    existingBooking.EstimatedDeliveryTime ?? 3,
                                    listAssignments,
                                    redisService,
                                    unitOfWork,
                                    scheduleBooking!.Id,
                                    bookingDetailTruck
                                );
                                countDriverNumberBooking -= (int)countDriver;
                                
                            }

                            var googleMapsService = scope.ServiceProvider.GetRequiredService<IGoogleMapsService>();
                            // xử lý
                            await AllocateDriversToBookingAsync(
                                existingBooking,
                                existingBooking.BookingAt!.Value,
                                countDriverNumberBooking,
                                existingBooking.EstimatedDeliveryTime ?? 3,
                                listAssignments,
                                assignedDriverAvailable1Hours,
                                assignedDriverAvailable2Hours,
                                assignedDriverAvailableOther,
                                unitOfWork,
                                scheduleBooking!.Id,
                                googleMapsService,
                                bookingDetailTruck
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
                            //đánh tag faild cần reviewer can thiệp
                            var user = await unitOfWork.UserRepository.GetManagerAsync();
                            bookingDetailTruck.Status = BookingDetailStatusEnums.WAITING.ToString();
                            await unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetailTruck);

                            var notification = new Notification
                            {
                                UserId = user.Id,  
                                SentFrom = "System",  
                                Receive = user.Name,    
                                Name = $"Driver Shortage Notification in {existingBooking.Id}",
                                Description = $"Only {countRemaining} drivers available for {countDriverNumberBooking} bookings.",
                                Topic = "DriverAssignment",                         
                                IsRead = false
                            };

                            await unitOfWork.NotificationRepository.SaveOrUpdateAsync(notification);
                            // Save the notification to Firestore
                            await firebaseServices.SaveMailManager(notification, existingBooking.Id, "reports");

                        }
                        
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("AssignDriverWorker - " + e.Message);
            throw;
        }
    }

    /// <summary>
    /// Assigns drivers to a booking by dequeuing driver IDs from Redis, creating assignments,
    /// and saving them in the schedule booking repository.
    /// </summary>
    /// <param name="bookingId">The ID of the booking to assign drivers to.</param>
    /// <param name="redisKey">The Redis key used to dequeue available driver IDs.</param>
    /// <param name="startTime">The start time for the assignment schedule.</param>
    /// <param name="driverCount">The number of drivers to assign to the booking.</param>
    /// <param name="estimatedDeliveryTime">The estimated time in hours for delivery.</param>
    /// <param name="listAssignments">The list of assignments where new driver assignments will be added.</param>
    /// <param name="redisService">Service used for interacting with Redis.</param>
    /// <param name="unitOfWork">Unit of work for handling database operations.</param>
    /// <param name="scheduleBookingId">The ID of the scheduleBooking to assign drivers to.</param>
    /// <param name="bookingDetailTruck">The ID of the bookingDetail</param>
    /// <returns>A task that represents the asynchronous operation of assigning drivers and saving schedule data.</returns>
    private async Task<List<Assignment>> AssignDriversToBooking(int bookingId, string redisKey, DateTime startTime, int driverCount,
        double estimatedDeliveryTime, List<Assignment> listAssignments,
        IRedisService redisService, UnitOfWork unitOfWork, int scheduleBookingId, BookingDetail bookingDetailTruck)
    {
        for (int i = 0; i < driverCount; i++)
        {
            var driverId = await redisService.DequeueAsync<int>(redisKey);
            var endTime = startTime.AddHours(estimatedDeliveryTime);
            var truck = await unitOfWork.TruckRepository.FindByUserIdAsync(driverId);
            var newAssignmentDriver = new Assignment()
            {
                BookingId = bookingId,
                StaffType = RoleEnums.DRIVER.ToString(),
                Status = AssignmentStatusEnums.WAITING.ToString(),
                UserId = driverId,
                StartDate = startTime,
                EndDate = endTime,
                IsResponsible = false,
                ScheduleBookingId = scheduleBookingId,
                TruckId = truck.Id,
                BookingDetailsId = bookingDetailTruck.Id
            };

            listAssignments.Add(newAssignmentDriver);
        }

        return listAssignments;
    }

    /// <summary>
    /// Assigns available drivers to a booking based on proximity and estimated travel time.
    /// Calculates a rating for each driver based on travel duration, then selects the most suitable drivers.
    /// </summary>
    /// <param name="booking">The current booking that requires driver assignment.</param>
    /// <param name="startTime">The start time for the booking's assigned drivers.</param>
    /// <param name="driverCount">The number of drivers needed for this booking.</param>
    /// <param name="estimatedDeliveryTime">The estimated time needed to complete the delivery for this booking.</param>
    /// <param name="listAssignments">The list of Assignment records to be updated and saved to the database.</param>
    /// <param name="listDriverAvailable1Hours">The list of drivers available within 1 hour.</param>
    /// <param name="listDriverAvailable2Hours">The list of drivers available within 2 hours.</param>
    /// <param name="listDriverAvailableOthers">The list of drivers available beyond 2 hours.</param>
    /// <param name="unitOfWork">The UnitOfWork instance used to save data to the database.</param>
    /// <param name="scheduleBookingId">The schedule booking ID for identifying driver details.</param>
    /// <param name="googleMapsService">The Google Maps service for calculating distance and travel time between locations.</param>
    /// <param name="bookingDetailTruck">The Id of booking Detail</param>
    private async Task<List<Assignment>> AllocateDriversToBookingAsync(
        Booking booking,
        DateTime startTime,
        int driverCount,
        double estimatedDeliveryTime,
        List<Assignment> listAssignments,
        List<Assignment> listDriverAvailable1Hours,
        List<Assignment> listDriverAvailable2Hours,
        List<Assignment> listDriverAvailableOthers,
        UnitOfWork unitOfWork,
        int scheduleBookingId,
        IGoogleMapsService googleMapsService,
        BookingDetail bookingDetailTruck)
    {
        var driverDistances = new List<(Assignment Driver, double rate)>();

        // 1 H
        foreach (var assignment in listDriverAvailable1Hours)
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
            driverDistances.Add((assignment, rate));
        }

        // 2 H
        foreach (var assignment in listDriverAvailable2Hours)
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
            driverDistances.Add((assignment, rate));
        }

        // Other
        foreach (var assignment in listDriverAvailableOthers)
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
            driverDistances.Add((assignment, rate));
        }
        
        driverDistances = driverDistances
            .GroupBy(d => d.Driver.UserId)        
            .Select(g => g.First())               
            .ToList();

        var closestDrivers = driverDistances
            .OrderByDescending(x => x.rate)
            .Take(driverCount)
            .ToList();

        foreach (var driver in closestDrivers)
        {
            var assignment = driver.Driver;
            var driverId = assignment.UserId!.Value;
            var endTime = startTime.AddHours(estimatedDeliveryTime);

            listAssignments.Add(new Assignment
            {
                BookingId = booking.Id,
                StaffType = RoleEnums.DRIVER.ToString(),
                Status = AssignmentStatusEnums.WAITING.ToString(),
                UserId = driverId,
                StartDate = startTime,
                EndDate = endTime,
                IsResponsible = false,
                ScheduleBookingId = scheduleBookingId,
                TruckId = assignment.TruckId,
                BookingDetailsId = bookingDetailTruck.Id
            });
        }

        return listAssignments;
       
    }
}