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

        if (checkExistQueue == false)
        {
            var timeExpiryRedisQueue = DateUtil.TimeUntilEndOfDay(existingBooking.BookingAt!.Value);

            var driverIds =
                await _unitOfWork.UserRepository.GetUsersWithTruckCategoryIdAsync(existingBooking!.TruckNumber!
                    .Value);
            await _redisService.EnqueueMultipleAsync(redisKey, driverIds, timeExpiryRedisQueue);
            await _redisService.EnqueueMultipleAsync(redisKeyV2, driverIds, timeExpiryRedisQueue);
            var driverNumberBooking = existingBooking.DriverNumber!.Value;
            if (driverNumberBooking > driverIds.Count)
            {
                // đánh tag faild
            }
            // đánh tag pass 
        }
        
    }
    
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
            
            rate = rate * distance / duration;
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