using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.GoongMap;
using MoveMate.Service.ThirdPartyService.GoongMap.Models;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.Utils;
using MoveMate.Service.ViewModels.ModelResponses;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;

namespace MoveMate.Service.Services;

public class AssignmentService : IAssignmentService
{
    private UnitOfWork _unitOfWork;
    private IMapper _mapper;
    private readonly ILogger<AssignmentService> _logger;
    private readonly IMessageProducer _producer;
    private readonly IFirebaseServices _firebaseServices;
    private readonly IRedisService _redisService;
    private readonly IGoogleMapsService _googleMapsService;

    public AssignmentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AssignmentService> logger,
        IMessageProducer producer, IFirebaseServices firebaseServices, IRedisService redisService,
        IGoogleMapsService googleMapsService)
    {
        _unitOfWork = (UnitOfWork)unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _producer = producer;
        _firebaseServices = firebaseServices;
        _redisService = redisService;
        _googleMapsService = googleMapsService;
    }

    public async Task<OperationResult<AssignManualDriverResponse>> HandleAssignManualDriver(int bookingId)
    {
        var existingBooking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
        if (existingBooking == null)
        {
            throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
        }

        var bookingDetailTruck =
            await _unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(
                TypeServiceEnums.TRUCK.ToString(), bookingId);

        if (bookingDetailTruck == null)
        {
            throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
        }

        var schedule =
            await _unitOfWork.ScheduleWorkingRepository.GetScheduleByBookingAtAsync(existingBooking.BookingAt.Value);

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
            await AssignDriversToBooking(
                bookingId,
                redisKey,
                existingBooking.BookingAt.Value,
                existingBooking.DriverNumber!.Value,
                existingBooking.EstimatedDeliveryTime ?? 3,
                listAssignments,
                scheduleBooking!.Id,
                bookingDetailTruck
            );


            await _unitOfWork.AssignmentsRepository.SaveOrUpdateRangeAsync(listAssignments);
            _unitOfWork.Save();

            var bookingFirebase = await _unitOfWork.BookingRepository.GetByIdAsyncV1(existingBooking.Id,
                includeProperties:
                "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments");

            await _firebaseServices.SaveBooking(bookingFirebase, bookingFirebase.Id, "bookings");
        }
        else
        {
            int countDriverNumberBooking = existingBooking.DriverNumber!.Value;
            var countDriver = await _redisService.CheckQueueCountAsync(redisKey);
            if (countDriver >= countDriverNumberBooking)
            {
                await AssignDriversToBooking(
                    bookingId,
                    redisKey,
                    existingBooking.BookingAt!.Value,
                    existingBooking.DriverNumber.Value,
                    existingBooking.EstimatedDeliveryTime ?? 3,
                    listAssignments,
                    scheduleBooking!.Id,
                    bookingDetailTruck
                );
                await _unitOfWork.AssignmentsRepository.SaveOrUpdateRangeAsync(listAssignments);
                _unitOfWork.Save();

                var bookingFirebase = await _unitOfWork.BookingRepository.GetByIdAsyncV1(existingBooking.Id,
                    includeProperties:
                    "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments");

                await _firebaseServices.SaveBooking(bookingFirebase, bookingFirebase.Id, "bookings");
            }
            else
            {
                var assignedDriverAvailable1Hours =
                    await _unitOfWork.AssignmentsRepository.GetDriverByGroupAvailableWithExtendedAsync(
                        existingBooking.BookingAt.Value, endTime, scheduleBooking.Id,
                        existingBooking.TruckNumber!.Value, schedule.GroupId);

                // 2H
                var assignedDriverAvailable2Hours =
                    await _unitOfWork.AssignmentsRepository.GetDriverByGroupAvailableWithExtendedAsync(
                        existingBooking.BookingAt.Value, endTime, scheduleBooking.Id,
                        existingBooking.TruckNumber!.Value, schedule.GroupId, 1, 1);
                // OTHER
                var assignedDriverAvailableOther =
                    await _unitOfWork.AssignmentsRepository.GetDriverByGroupAvailableWithOverlapAsync(
                        existingBooking.BookingAt.Value, endTime, scheduleBooking.Id,
                        existingBooking.TruckNumber!.Value, schedule.GroupId, 2);

                var countRemaining = (int)countDriver + assignedDriverAvailable1Hours.Count() +
                                     assignedDriverAvailable2Hours.Count() +
                                     assignedDriverAvailableOther.Count();
                if (countRemaining < countDriverNumberBooking)
                {
                    // đánh tag failed
                }
                if (countDriver > 0)
                {
                    countRemaining -= (int)countDriver;

                    await AssignDriversToBooking(
                        bookingId,
                        redisKey,
                        existingBooking.BookingAt!.Value,
                        countDriverNumberBooking,
                        existingBooking.EstimatedDeliveryTime ?? 3,
                        listAssignments,
                        scheduleBooking!.Id,
                        bookingDetailTruck
                    );
                    countDriverNumberBooking -= (int)countDriver;
                }
                
                await AllocateDriversToBookingAsync(
                    existingBooking,
                    existingBooking.BookingAt!.Value,
                    countDriverNumberBooking,
                    existingBooking.EstimatedDeliveryTime ?? 3,
                    listAssignments,
                    assignedDriverAvailable1Hours,
                    assignedDriverAvailable2Hours,
                    assignedDriverAvailableOther,
                    scheduleBooking!.Id,
                    bookingDetailTruck
                );
            }
        }
        var result = new OperationResult<AssignManualDriverResponse>();
        var response = new AssignManualDriverResponse();
        var listAssignmentResponse = _mapper.Map<List<AssignmentResponse>>(listAssignments);
        response.AssignmentManualDrivers.AddRange(listAssignmentResponse);
        response.BookingNeedDrivers = existingBooking.DriverNumber.Value;
        var isGroup1 = schedule.GroupId == 1 ? true : false;

        if (isGroup1)
        {
            var listDriverNeed = await 
                _unitOfWork.UserRepository.GetWithTruckCategoryIdAsync(existingBooking.TruckNumber.Value, 2);
            var listUserResponse = _mapper.Map<List<UserResponse>>(listDriverNeed);
            response.OtherDrivers.AddRange(listUserResponse);
        }
        else
        {
            var listDriverNeed = await
                _unitOfWork.UserRepository.GetWithTruckCategoryIdAsync(existingBooking.TruckNumber.Value, 1);
            var listUserResponse = _mapper.Map<List<UserResponse>>(listDriverNeed);
            response.OtherDrivers.AddRange(listUserResponse);

        }

        if (listAssignmentResponse.Count() >= response.BookingNeedDrivers)
        {
            response.IsSussed = true;
        } 
        
        result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.AssignmentManual, response);

        return result;
    }

    private async Task<List<Assignment>> AssignDriversToBooking(
        int bookingId,
        string redisKey,
        DateTime startTime,
        int driverCount,
        double estimatedDeliveryTime,
        List<Assignment> listAssignments,
        int scheduleBookingId,
        BookingDetail bookingDetail)
    {
        for (int i = 0; i < driverCount; i++)
        {
            var driverId = await _redisService.DequeueAsync<int>(redisKey);
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
                TruckId = truck.Id,
                BookingDetailsId = bookingDetail.Id
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
        int scheduleBookingId,
        BookingDetail bookingDetail
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
                BookingDetailsId = bookingDetail.Id

            });
        }

        return listAssignments;
    }
}