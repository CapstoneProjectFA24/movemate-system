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
using MoveMate.Service.ViewModels.ModelRequests.Assignments;
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

    #region FEATURE: HandleAssignManualDriver.

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
        response.AssignmentManualStaffs.AddRange(listAssignmentResponse);
        response.BookingNeedStaffs = existingBooking.DriverNumber.Value;
        var isGroup1 = schedule.GroupId == 1 ? true : false;

        if (isGroup1)
        {
            var listDriverNeed = await
                _unitOfWork.UserRepository.GetWithTruckCategoryIdAsync(existingBooking.TruckNumber.Value, 2);
            var listUserResponse = _mapper.Map<List<UserResponse>>(listDriverNeed);
            response.OtherStaffs.AddRange(listUserResponse);
        }
        else
        {
            var listDriverNeed = await
                _unitOfWork.UserRepository.GetWithTruckCategoryIdAsync(existingBooking.TruckNumber.Value, 1);
            var listUserResponse = _mapper.Map<List<UserResponse>>(listDriverNeed);
            response.OtherStaffs.AddRange(listUserResponse);
        }

        response.StaffType = RoleEnums.DRIVER.ToString();
        if (listAssignmentResponse.Count() >= response.BookingNeedStaffs)
        {
            response.IsSussed = true;
            bookingDetailTruck.Status = BookingDetailStatusEnums.AVAILABLE.ToString();
            await _unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetailTruck);

            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.AssignmentManual, response);
        }
        else
        {
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.FailMessage.AssignmentManual, response);
        }

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

    #endregion

    #region FEATURE: HandleAssignManualPorter.

    public async Task<OperationResult<AssignManualDriverResponse>> HandleAssignManualPorter(int bookingId)
    {
        var existingBooking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
        if (existingBooking == null)
        {
            throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
        }

        var bookingDetail =
            await _unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(
                TypeServiceEnums.PORTER.ToString(), bookingId);

        if (bookingDetail == null)
        {
            throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
        }

        var schedule =
            await _unitOfWork.ScheduleWorkingRepository.GetScheduleByBookingAtAsync(existingBooking.BookingAt.Value);

        var endTime = existingBooking.BookingAt!.Value.AddHours(existingBooking.EstimatedDeliveryTime!.Value);

        var date = DateUtil.GetShard(existingBooking.BookingAt);

        var redisKey = DateUtil.GetKeyPorter(existingBooking.BookingAt);
        var redisKeyV2 = DateUtil.GetKeyPorterV2(existingBooking.BookingAt);

        var checkExistQueue = await _redisService.KeyExistsQueueAsync(redisKeyV2);

        var scheduleBooking = await _unitOfWork.ScheduleBookingRepository.GetByShard(date);
        var listAssignments = new List<Assignment>();

        if (checkExistQueue == false)
        {
            var timeExpiryRedisQueue = DateUtil.TimeUntilEndOfDay(existingBooking.BookingAt!.Value);

            var porterIds =
                await _unitOfWork.UserRepository.FindAllUserByRoleIdAndGroupIdAsync(5
                    , schedule.GroupId.Value);
            await _redisService.EnqueueMultipleAsync(redisKey, porterIds, timeExpiryRedisQueue);
            await _redisService.EnqueueMultipleAsync(redisKeyV2, porterIds, timeExpiryRedisQueue);
            var porterNumberBooking = existingBooking.PorterNumber!.Value;
            if (porterNumberBooking > porterIds.Count)
            {
                // đánh tag faild
            }

            // đánh tag pass 
            await AssignPortersToBooking(
                bookingId,
                redisKey,
                existingBooking.BookingAt.Value,
                existingBooking.PorterNumber!.Value,
                existingBooking.EstimatedDeliveryTime ?? 3,
                listAssignments,
                scheduleBooking!.Id,
                bookingDetail
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
            int countporterNumberBooking = existingBooking.DriverNumber!.Value;
            var countPorter = await _redisService.CheckQueueCountAsync(redisKey);
            if (countPorter >= countporterNumberBooking)
            {
                await AssignPortersToBooking(
                    bookingId,
                    redisKey,
                    existingBooking.BookingAt!.Value,
                    existingBooking.PorterNumber.Value,
                    existingBooking.EstimatedDeliveryTime ?? 3,
                    listAssignments,
                    scheduleBooking!.Id,
                    bookingDetail
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
                var assignedPortersAvailable1Hours =
                    await _unitOfWork.AssignmentsRepository.GetPortersByGroupAvailableWithExtendedAsync(
                        existingBooking.BookingAt.Value, endTime, scheduleBooking.Id,
                        schedule.GroupId);

                // 2H
                var assignedPortersAvailable2Hours =
                    await _unitOfWork.AssignmentsRepository.GetPortersByGroupAvailableWithExtendedAsync(
                        existingBooking.BookingAt.Value, endTime, scheduleBooking.Id,
                        schedule.GroupId, 1, 1);
                // OTHER
                var assignedPortersAvailableOther =
                    await _unitOfWork.AssignmentsRepository.GetPorterByGroupAvailableWithOverlapAsync(
                        existingBooking.BookingAt.Value, endTime, scheduleBooking.Id,
                        schedule.GroupId, 2);

                var countRemaining = (int)countPorter + assignedPortersAvailable1Hours.Count() +
                                     assignedPortersAvailable2Hours.Count() +
                                     assignedPortersAvailableOther.Count();
                if (countRemaining < countporterNumberBooking)
                {
                    // đánh tag failed
                }

                if (countPorter > 0)
                {
                    countRemaining -= (int)countPorter;

                    await AssignPortersToBooking(
                        bookingId,
                        redisKey,
                        existingBooking.BookingAt!.Value,
                        countporterNumberBooking,
                        existingBooking.EstimatedDeliveryTime ?? 3,
                        listAssignments,
                        scheduleBooking!.Id,
                        bookingDetail
                    );
                    countporterNumberBooking -= (int)countPorter;
                }

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
                    bookingDetail
                );
            }
        }

        var result = new OperationResult<AssignManualDriverResponse>();
        var response = new AssignManualDriverResponse();
        var listAssignmentResponse = _mapper.Map<List<AssignmentResponse>>(listAssignments);
        response.AssignmentManualStaffs.AddRange(listAssignmentResponse);
        response.BookingNeedStaffs = existingBooking.PorterNumber.Value;
        var isGroup1 = schedule.GroupId == 1 ? true : false;

        if (isGroup1)
        {
            var listDriverNeed = await
                _unitOfWork.UserRepository.GetWithTruckCategoryIdAsync(existingBooking.TruckNumber.Value, 2);
            var listUserResponse = _mapper.Map<List<UserResponse>>(listDriverNeed);
            response.OtherStaffs.AddRange(listUserResponse);
        }
        else
        {
            var listDriverNeed = await
                _unitOfWork.UserRepository.GetWithTruckCategoryIdAsync(existingBooking.TruckNumber.Value, 1);
            var listUserResponse = _mapper.Map<List<UserResponse>>(listDriverNeed);
            response.OtherStaffs.AddRange(listUserResponse);
        }

        response.StaffType = RoleEnums.PORTER.ToString();
        if (listAssignmentResponse.Count() >= response.BookingNeedStaffs)
        {
            response.IsSussed = true;
            bookingDetail.Status = BookingDetailStatusEnums.AVAILABLE.ToString();
            await _unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetail);

            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.AssignmentManual, response);
        }
        else
        {
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.FailMessage.AssignmentManual, response);
        }

        return result;
    }


    private async Task<List<Assignment>> AssignPortersToBooking(int bookingId, string redisKey, DateTime startTime,
        int porterCount,
        double estimatedDeliveryTime, List<Assignment> listAssignments,
        int scheduleBookingId, BookingDetail bookingDetail)
    {
        for (int i = 0; i < porterCount; i++)
        {
            var staffId = await _redisService.DequeueAsync<int>(redisKey);
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

    #endregion

    public async Task<OperationResult<BookingResponse>> HandleAssignManualStaff(int bookingId,
        AssignedManualStaffRequest request)
    {
        var result = new OperationResult<BookingResponse>();

        var existingBooking = await _unitOfWork.BookingRepository.GetByIdAsyncV1(bookingId,
            includeProperties:
            "Assignments");
        if (existingBooking == null)
        {
            throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
        }

        var bookingDetail =
            await _unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(
                request.StaffType, bookingId);

        if (bookingDetail == null)
        {
            throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
        }

        if (request.FailedAssignmentId != null)
        {
            var failedAssignment =
                await _unitOfWork.AssignmentsRepository.GetByIdAsync(request.FailedAssignmentId.Value);
            failedAssignment.Status = AssignmentStatusEnums.FAILED.ToString();
            await _unitOfWork.AssignmentsRepository.UpdateAsync(failedAssignment);
        }
        
        var endTime = existingBooking.BookingAt!.Value.AddHours(existingBooking.EstimatedDeliveryTime!.Value);

        var date = DateUtil.GetShard(existingBooking.BookingAt);

        var scheduleBooking = await _unitOfWork.ScheduleBookingRepository.GetByShard(date);

        switch (request.StaffType)
        {
            case "DRIVER":

                var driverAssignments = existingBooking.Assignments
                    .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                                a.Status != AssignmentStatusEnums.FAILED.ToString())
                    .ToList();

                if (driverAssignments.Count >= existingBooking.DriverNumber)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.AssignmentUpdateFail);
                    return result;
                }

                if (driverAssignments.Any(a => a.UserId == request.AssignToUserId))
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.AssignmentDuplicate);
                    return result;
                }

                Console.WriteLine($"Filtered {driverAssignments.Count} DRIVER assignments.");

                var driver = await _unitOfWork.UserRepository.GetByIdAsyncV1(request.AssignToUserId.Value,
                    includeProperties:
                    "Truck");

                var isDriver = driver.RoleId == 4 ? true : false;
                if (isDriver)
                {
                 
                    if (driver.Truck.TruckCategoryId != existingBooking.TruckNumber)
                    {
                        result.AddError(StatusCode.NotFound,
                            MessageConstant.FailMessage.AssignmentUpdateFailByDiffTruckCate);
                        return result;
                    }

                    var newStaff = new Assignment
                    {
                        BookingId = existingBooking.Id,
                        StaffType = request.StaffType,
                        Status = AssignmentStatusEnums.WAITING.ToString(),
                        UserId = request.AssignToUserId,
                        StartDate = existingBooking.BookingAt!.Value,
                        EndDate = endTime,
                        IsResponsible = false,
                        ScheduleBookingId = scheduleBooking!.Id,
                        BookingDetailsId = bookingDetail.Id
                    };
                    await _unitOfWork.AssignmentsRepository.SaveOrUpdateAsync(newStaff);
                }
                else
                {
                    result.AddError(StatusCode.NotFound,
                        MessageConstant.FailMessage.AssignmentUpdateFailOtherStaffType);
                    return result;
                }

                break;

            case "PORTER":

                var porterAssignments = existingBooking.Assignments
                    .Where(a => a.StaffType == RoleEnums.PORTER.ToString() &&
                                a.Status != AssignmentStatusEnums.FAILED.ToString())
                    .ToList();

                if (porterAssignments.Count >= existingBooking.DriverNumber)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.AssignmentUpdateFail);
                    return result;
                }

                if (porterAssignments.Any(a => a.UserId == request.AssignToUserId))
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.AssignmentDuplicate);
                    return result;
                }

                Console.WriteLine($"Filtered {porterAssignments.Count} PORTER assignments.");

                var porter = await _unitOfWork.UserRepository.GetByIdAsync(request.AssignToUserId.Value);

                var isPorter = porter.RoleId == 5 ? true : false;

                if (isPorter)
                {
                    var newStaff = new Assignment
                    {
                        BookingId = existingBooking.Id,
                        StaffType = request.StaffType,
                        Status = AssignmentStatusEnums.WAITING.ToString(),
                        UserId = request.AssignToUserId,
                        StartDate = existingBooking.BookingAt!.Value,
                        EndDate = endTime,
                        IsResponsible = false,
                        ScheduleBookingId = scheduleBooking!.Id,
                        BookingDetailsId = bookingDetail.Id
                    };

                    await _unitOfWork.AssignmentsRepository.SaveOrUpdateAsync(newStaff);
                }
                else
                {
                    result.AddError(StatusCode.NotFound,
                        MessageConstant.FailMessage.AssignmentUpdateFailOtherStaffType);
                    return result;
                }

                break;

            default:
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.AssignmentUpdateFail);
                return result;
        }

        await _unitOfWork.SaveChangesAsync();
        
        existingBooking = await _unitOfWork.BookingRepository.GetByIdAsyncV1((int)bookingDetail.BookingId,
            includeProperties:
            "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments,Vouchers");
        await _firebaseServices.SaveBooking(existingBooking, existingBooking.Id, "bookings");

        var response = _mapper.Map<BookingResponse>(existingBooking);
        result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingUpdateSuccess,
            response);
        
        return result;
    }
}