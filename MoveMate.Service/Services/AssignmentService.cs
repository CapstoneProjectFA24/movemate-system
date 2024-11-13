using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.GoongMap;
using MoveMate.Service.ThirdPartyService.GoongMap.Models;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.Utils;

namespace MoveMate.Service.Services;

public class AssignmentService
{
    private UnitOfWork _unitOfWork;
    private IMapper _mapper;
    private readonly ILogger<AssignmentService> _logger;
    private readonly IMessageProducer _producer;
    private readonly IFirebaseServices _firebaseServices;
    private readonly IRedisService _redisService;
    private readonly IGoogleMapsService _googleMapsService;

    public AssignmentService(UnitOfWork unitOfWork, IMapper mapper, ILogger<AssignmentService> logger, IMessageProducer producer, IFirebaseServices firebaseServices, IRedisService redisService, IGoogleMapsService googleMapsService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _producer = producer;
        _firebaseServices = firebaseServices;
        _redisService = redisService;
        _googleMapsService = googleMapsService;
    }

    public async Task HandleMessage(int message)
    {
        var existingBooking = await _unitOfWork.BookingRepository.GetByIdAsync(message);
        if (existingBooking == null)
        {
            throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
        }
        
        var endTime = existingBooking.BookingAt!.Value.AddHours(existingBooking.EstimatedDeliveryTime!.Value);

        var date = DateUtil.GetShard(existingBooking.BookingAt);
                
        var redisKey = DateUtil.GetKeyDriver(existingBooking.BookingAt, existingBooking.TruckNumber!.Value);
        var redisKeyV2 = DateUtil.GetKeyDriverV2(existingBooking.BookingAt, existingBooking.TruckNumber!.Value);

        var checkExistQueue = await _redisService.KeyExistsQueueAsync(redisKeyV2);

        var scheduleBooking = await _unitOfWork.ScheduleBookingRepository.GetByShard(date);
        var listAssignments = new List<Assignment>();
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
    /// <param name="scheduleBookingId">The ID of the scheduleBooking to assign drivers to.</param>
    /// <returns>A task that represents the asynchronous operation of assigning drivers and saving schedule data.</returns>
    private async Task<List<Assignment>> AssignDriversToBooking(int bookingId, string redisKey, DateTime startTime, int driverCount,
        double estimatedDeliveryTime, List<Assignment> listAssignments,
        IRedisService redisService, int scheduleBookingId)
    {
        for (int i = 0; i < driverCount; i++)
        {
            var driverId = await redisService.DequeueAsync<int>(redisKey);
            var endTime = startTime.AddHours(estimatedDeliveryTime);
            var truck = await _unitOfWork.TruckRepository.FindByUserIdAsync(driverId);
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
                TruckId = truck.Id
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
    /// <param name="scheduleBookingId">The schedule booking ID for identifying driver details.</param>
    private async Task<List<Assignment>> AllocateDriversToBookingAsync(
        Booking booking,
        DateTime startTime,
        int driverCount,
        double estimatedDeliveryTime,
        List<Assignment> listAssignments,
        List<Assignment> listDriverAvailable1Hours,
        List<Assignment> listDriverAvailable2Hours,
        List<Assignment> listDriverAvailableOthers,
        int scheduleBookingId
        )
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
                    await _googleMapsService.GetDistanceAndDuration(assignment.Booking!.PickupPoint!,
                        booking.DeliveryPoint!);
            }
            else
            {
                googleMapDto =
                    await _googleMapsService.GetDistanceAndDuration(assignment.Booking!.DeliveryPoint!,
                        booking.PickupPoint!);
            }
            
            var distance = googleMapDto.Distance.Value;
            var duration = googleMapDto.Duration.Value;
            rate = rate / duration;
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
                    await _googleMapsService.GetDistanceAndDuration(assignment.Booking!.PickupPoint!,
                        booking.DeliveryPoint!);
            }
            else
            {
                googleMapDto =
                    await _googleMapsService.GetDistanceAndDuration(assignment.Booking!.DeliveryPoint!,
                        booking.PickupPoint!);
            }

            var distance = googleMapDto.Distance.Value;
            var duration = googleMapDto.Duration.Value;
            rate = rate / duration;
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
                    await _googleMapsService.GetDistanceAndDuration(assignment.Booking!.PickupPoint!,
                        booking.DeliveryPoint!);
            }
            else
            {
                googleMapDto =
                    await _googleMapsService.GetDistanceAndDuration(assignment.Booking!.DeliveryPoint!,
                        booking.PickupPoint!);
            }

            var distance = googleMapDto.Distance.Value;
            var duration = googleMapDto.Duration.Value;
            rate = rate / duration;
            driverDistances.Add((assignment, rate));
        }

        var closestDrivers = driverDistances
            .OrderBy(x => x.rate)
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
            });
        }

        return listAssignments;
       
    }


}