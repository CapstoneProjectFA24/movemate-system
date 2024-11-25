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
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelRequests.Assignments;
using MoveMate.Service.ViewModels.ModelResponses;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;
using System.IO;

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
        var result = new OperationResult<AssignManualDriverResponse>();

        var existingBooking =
            await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, includeProperties: "Assignments");
        if (existingBooking == null)
        {
            throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
        }

        if (existingBooking.Status == BookingEnums.ASSIGNED.ToString() || existingBooking.Status ==
                                                                       BookingEnums.WAITING.ToString()
                                                                       || existingBooking.Status ==
                                                                       BookingEnums.PENDING.ToString() ||
                                                                       existingBooking.Status ==
                                                                       BookingEnums.DEPOSITING.ToString()
                                                                       || existingBooking.Status ==
                                                                       BookingEnums.REVIEWED.ToString() ||
                                                                       existingBooking.Status ==
                                                                       BookingEnums.REVIEWED.ToString())
        {
            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotAssigned);
            return result;
        }

        var bookingDetailTruck =
            await _unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(
                TypeServiceEnums.TRUCK.ToString(), bookingId);

        if (bookingDetailTruck == null)
        {
            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundBookingDetail);
            return result;
        }

        var schedule =
            await _unitOfWork.ScheduleWorkingRepository.GetScheduleByBookingAtAsync(existingBooking.BookingAt.Value);

        var endTime = existingBooking.BookingAt!.Value.AddHours(existingBooking.EstimatedDeliveryTime!.Value);

        var date = DateUtil.GetShard(existingBooking.BookingAt);

        var redisKey = DateUtil.GetKeyDriver(existingBooking.BookingAt, existingBooking.TruckNumber!.Value,
            schedule.GroupId.Value, schedule.Id);
        var redisKeyV2 = DateUtil.GetKeyDriverV2(existingBooking.BookingAt, existingBooking.TruckNumber!.Value,
            schedule.GroupId.Value, schedule.Id);

        var checkExistQueue = await _redisService.KeyExistsQueueAsync(redisKeyV2);

        var scheduleBooking = await _unitOfWork.ScheduleBookingRepository.GetByShard(date);
        var listAssignments = new List<Assignment>();

        if (checkExistQueue == false)
        {
            var timeExpiryRedisQueue = DateUtil.TimeUntilEndOfDay(existingBooking.BookingAt!.Value);

            var driverIds =
                await _unitOfWork.UserRepository.GetUsersWithTruckCategoryIdAsync(existingBooking!.TruckNumber!
                    .Value, schedule.GroupId.Value);
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
                bookingDetailTruck,
                existingBooking.Assignments.ToList()
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
            int countDriverNumberBooking = existingBooking.DriverNumber!.Value; // count driver need in booking
            int countDriverExistingBooking = existingBooking.Assignments.Count(assignment =>
                assignment.StaffType == RoleEnums.DRIVER.ToString() &&
                assignment.Status !=
                AssignmentStatusEnums.FAILED.ToString()); //count driver have been assignment in booking 
            if (countDriverExistingBooking > 0)
            {
                countDriverNumberBooking -= countDriverExistingBooking;
                countDriverExistingBooking = 0;
            }

            var countDriver = await _redisService.CheckQueueCountAsync(redisKey);
            if (countDriverNumberBooking <= countDriverExistingBooking)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.AssignmentUpdateFail);
                return result;
            }

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
                    bookingDetailTruck,
                    existingBooking.Assignments.ToList()
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
                        bookingDetailTruck,
                        existingBooking.Assignments.ToList()
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

        //var result = new OperationResult<AssignManualDriverResponse>();
        var response = new AssignManualDriverResponse();
        var listAssignmentResponse = _mapper.Map<List<AssignmentResponse>>(listAssignments);
        response.AssignmentManualStaffs.AddRange(listAssignmentResponse);
        response.BookingNeedStaffs = existingBooking.DriverNumber.Value -
                                     existingBooking.Assignments.Count(assignment =>
                                         assignment.StaffType == RoleEnums.DRIVER.ToString() &&
                                         assignment.Status != AssignmentStatusEnums.FAILED.ToString());
        var isGroup1 = schedule.GroupId == 1 ? true : false;

        response.StaffType = RoleEnums.DRIVER.ToString();
        if (listAssignmentResponse.Count() >= response.BookingNeedStaffs)
        {
            response.IsSuccessed = true;
            bookingDetailTruck.Status = BookingDetailStatusEnums.AVAILABLE.ToString();
            await _unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetailTruck);
            await _unitOfWork.SaveChangesAsync();
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.AssignmentManual, response);
        }
        else
        {
            var groupId = isGroup1 ? 2 : 1;

            var listDriverNeed = await _unitOfWork.UserRepository
                .GetWithTruckCategoryIdAsync(existingBooking.TruckNumber.Value, groupId, includeProperties: "Role");

            var userAssignedIds = existingBooking.Assignments
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString())
                .Where(a => a.UserId.HasValue)
                .Select(a => a.UserId.Value)
                .Distinct()
                .ToList();

            var usersNotAssigned = listDriverNeed
                .Where(user => !userAssignedIds.Contains(user.Id))
                .ToList();

            var listUserResponse = _mapper.Map<List<UserResponse>>(usersNotAssigned);
            response.OtherStaffs.AddRange(listUserResponse);

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
        BookingDetail bookingDetail,
        List<Assignment> existingAssignments)
    {
        var existingUserIds = listAssignments
            .Select(a => a.UserId)
            .Concat(existingAssignments
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString() &&
                            a.Status != AssignmentStatusEnums.FAILED.ToString())
                .Select(a => a.UserId))
            .ToHashSet();

        int driversNeeded = driverCount - existingUserIds.Count;
        if (driversNeeded <= 0)
        {
            return listAssignments;
        }

        for (int i = 0; i < driversNeeded; i++)
        {
            var driverId = await _redisService.DequeueAsync<int>(redisKey);

            if (existingUserIds.Contains(driverId))
            {
                continue; // Pass if exist
            }

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
        var result = new OperationResult<AssignManualDriverResponse>();

        var existingBooking =
            await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, includeProperties: "Assignments");
        if (existingBooking == null)
        {
            throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
        }

        if (existingBooking.Status == BookingEnums.ASSIGNED.ToString() || existingBooking.Status ==
                                                                       BookingEnums.WAITING.ToString()
                                                                       || existingBooking.Status ==
                                                                       BookingEnums.PENDING.ToString() ||
                                                                       existingBooking.Status ==
                                                                       BookingEnums.DEPOSITING.ToString()
                                                                       || existingBooking.Status ==
                                                                       BookingEnums.REVIEWED.ToString() ||
                                                                       existingBooking.Status ==
                                                                       BookingEnums.REVIEWED.ToString())
        {
            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotAssigned);
            return result;
        }

        var bookingDetail =
            await _unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(
                TypeServiceEnums.PORTER.ToString(), bookingId);

        if (bookingDetail == null)
        {
            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundBookingDetail);
            return result;
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
                bookingDetail,
                existingBooking.Assignments.ToList()
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
            int countPorterExistingBooking = existingBooking.Assignments.Count(assignment =>
                assignment.StaffType == RoleEnums.PORTER.ToString() &&
                assignment.Status !=
                AssignmentStatusEnums.FAILED.ToString()); //count porter have been assignment in booking 
            if (countPorterExistingBooking > 0)
            {
                countporterNumberBooking -= countPorterExistingBooking;
                countPorterExistingBooking = 0;
            }

            var countPorter = await _redisService.CheckQueueCountAsync(redisKey);
            if (countporterNumberBooking <= countPorterExistingBooking)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.AssignmentUpdateFail);
                return result;
            }

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
                    bookingDetail,
                    existingBooking.Assignments.ToList()
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
                        bookingDetail,
                        existingBooking.Assignments.ToList()
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

        //var result = new OperationResult<AssignManualDriverResponse>();
        var response = new AssignManualDriverResponse();
        var listAssignmentResponse = _mapper.Map<List<AssignmentResponse>>(listAssignments);
        response.AssignmentManualStaffs.AddRange(listAssignmentResponse);
        response.BookingNeedStaffs = existingBooking.PorterNumber.Value -
                                     existingBooking.Assignments.Count(assignment =>
                                         assignment.StaffType == RoleEnums.PORTER.ToString() &&
                                         assignment.Status != AssignmentStatusEnums.FAILED.ToString());
        var isGroup1 = schedule.GroupId == 1 ? true : false;


        response.StaffType = RoleEnums.PORTER.ToString();
        if (listAssignmentResponse.Count() >= response.BookingNeedStaffs)
        {
            response.IsSuccessed = true;
            bookingDetail.Status = BookingDetailStatusEnums.AVAILABLE.ToString();
            await _unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetail);
            await _unitOfWork.SaveChangesAsync();
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.AssignmentManual, response);
        }
        else
        {
            var groupId = isGroup1 ? 2 : 1;

            var listDriverNeed = await _unitOfWork.UserRepository
                .GetUserWithRoleIdAndGroupIdAsync(5, groupId, includeProperties: "Role");

            var userAssignedIds = existingBooking.Assignments
                .Where(a => a.StaffType == RoleEnums.PORTER.ToString())
                .Where(a => a.UserId.HasValue)
                .Select(a => a.UserId.Value)
                .Distinct()
                .ToList();

            var usersNotAssigned = listDriverNeed
                .Where(user => !userAssignedIds.Contains(user.Id))
                .ToList();

            var listUserResponse = _mapper.Map<List<UserResponse>>(usersNotAssigned);
            response.OtherStaffs.AddRange(listUserResponse);

            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.FailMessage.AssignmentManual, response);
        }

        return result;
    }


    private async Task<List<Assignment>> AssignPortersToBooking(int bookingId, string redisKey, DateTime startTime,
        int porterCount,
        double estimatedDeliveryTime, List<Assignment> listAssignments,
        int scheduleBookingId, BookingDetail bookingDetail,
        List<Assignment> existingAssignments)
    {
        var existingUserIds = listAssignments
            .Select(a => a.UserId)
            .Concat(existingAssignments
                .Where(a => a.StaffType == RoleEnums.PORTER.ToString() &&
                            a.Status != AssignmentStatusEnums.FAILED.ToString())
                .Select(a => a.UserId))
            .ToHashSet();
        for (int i = 0; i < porterCount; i++)
        {
            var porterId = await _redisService.DequeueAsync<int>(redisKey);

            if (existingUserIds.Contains(porterId))
            {
                continue; // Pass if exist
            }

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
            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundBooking);
            return result;
        }

        if (request.FailedAssignmentId != null)
        {
            var failedAssignment =
                await _unitOfWork.AssignmentsRepository.GetByIdAsync(request.FailedAssignmentId.Value);
            failedAssignment.Status = AssignmentStatusEnums.FAILED.ToString();
            await _unitOfWork.AssignmentsRepository.UpdateAsync(failedAssignment);
        }

        if (existingBooking.Status == BookingEnums.ASSIGNED.ToString() || existingBooking.Status ==
                                                                       BookingEnums.WAITING.ToString()
                                                                       || existingBooking.Status ==
                                                                       BookingEnums.PENDING.ToString() ||
                                                                       existingBooking.Status ==
                                                                       BookingEnums.DEPOSITING.ToString()
                                                                       || existingBooking.Status ==
                                                                       BookingEnums.REVIEWED.ToString() ||
                                                                       existingBooking.Status ==
                                                                       BookingEnums.REVIEWED.ToString())
        {
            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotAssigned);
            return result;
        }


        var date = DateUtil.GetShard(existingBooking.BookingAt);

        var scheduleBooking = await _unitOfWork.ScheduleBookingRepository.GetByShard(date);

        switch (request.StaffType)
        {
            case var role when role == RoleEnums.DRIVER.ToString(): // type for Driver
                var endTime = existingBooking.BookingAt!.Value.AddHours(existingBooking.EstimatedDeliveryTime!.Value);
                var bookingDetailDriver =
                    await _unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(
                        TypeServiceEnums.TRUCK.ToString(), bookingId);
                if (bookingDetailDriver == null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundBookingDetail);
                    return result;
                }

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
                        BookingDetailsId = bookingDetailDriver.Id
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

            case var role when role == RoleEnums.PORTER.ToString(): //type for Porter
                var endTimePorter =
                    existingBooking.BookingAt!.Value.AddHours(existingBooking.EstimatedDeliveryTime!.Value);
                var bookingDetailPorter =
                    await _unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(
                        request.StaffType, bookingId);
                if (bookingDetailPorter == null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundBookingDetail);
                    return result;
                }

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
                        EndDate = endTimePorter,
                        IsResponsible = false,
                        ScheduleBookingId = scheduleBooking!.Id,
                        BookingDetailsId = bookingDetailPorter.Id
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

            case "REVIEWER": // type for Review

                var reviewerAssignments = existingBooking.Assignments
                    .Where(a => a.StaffType == RoleEnums.REVIEWER.ToString() &&
                                a.Status != AssignmentStatusEnums.FAILED.ToString())
                    .ToList();

                if (reviewerAssignments.Any(a => a.UserId == request.AssignToUserId))
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.AssignmentDuplicate);
                    return result;
                }

                Console.WriteLine($"Filtered {reviewerAssignments.Count} REVIEWER assignments.");

                var review = await _unitOfWork.UserRepository.GetByIdAsyncV1(request.AssignToUserId.Value);

                var isReview = review.RoleId == 2 ? true : false;
                if (isReview)
                {
                    var newStaff = new Assignment
                    {
                        BookingId = existingBooking.Id,
                        StaffType = request.StaffType,
                        Status = AssignmentStatusEnums.ASSIGNED.ToString(),
                        UserId = request.AssignToUserId,
                        StartDate = existingBooking.BookingAt!.Value,
                        IsResponsible = false,
                        ScheduleBookingId = scheduleBooking!.Id
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

        existingBooking = await _unitOfWork.BookingRepository.GetByIdAsyncV1((int)bookingId,
            includeProperties:
            "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments,Vouchers");
        await _firebaseServices.SaveBooking(existingBooking, existingBooking.Id, "bookings");

        var response = _mapper.Map<BookingResponse>(existingBooking);
        result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingUpdateSuccess,
            response);

        return result;
    }

    public async Task<OperationResult<DriverInfoDTO>> GetAvailableDriversForBooking(int bookingId)
    {
        var result = new OperationResult<DriverInfoDTO>();
        var responses = new DriverInfoDTO();
        var existingBooking =
            await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, includeProperties: "Assignments");
        if (existingBooking == null)
        {
            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundBooking);
            return result;
        }

        if (existingBooking.DriverNumber == 0)
        {
            responses.BookingNeedStaffs = 0;
            responses.IsSuccessed = true;
            responses.StaffType = RoleEnums.DRIVER.ToString();
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingHasNoDriver, responses);
            return result;
        }

        var bookingDetailTruck =
            await _unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(
                TypeServiceEnums.TRUCK.ToString(), bookingId);

        if (bookingDetailTruck == null)
        {
            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundBookingDetail);
            return result;
        }

        var schedule =
            await _unitOfWork.ScheduleWorkingRepository.GetScheduleByBookingAtAsync(existingBooking.BookingAt.Value);

        DateTime endTime;

        if (!existingBooking.EstimatedDeliveryTime.HasValue)
        {
            GoogleMapDTO? googleMapDto = null;
            googleMapDto =
                await _googleMapsService.GetDistanceAndDuration(existingBooking.PickupPoint!,
                    existingBooking.DeliveryPoint!);
            var duration = googleMapDto.Duration.Value;
            var durationEndtime = duration / 3600.0 + 1.5;
            endTime = existingBooking.BookingAt!.Value.AddHours(durationEndtime);
        }
        else
        {
            endTime = existingBooking.BookingAt!.Value.AddHours(existingBooking.EstimatedDeliveryTime.HasValue
                ? existingBooking.EstimatedDeliveryTime.Value
                : 2);
        }

        var date = DateUtil.GetShard(existingBooking.BookingAt);

        var redisKey = DateUtil.GetKeyDriver(existingBooking.BookingAt, existingBooking.TruckNumber!.Value,
            schedule.GroupId.Value, schedule.Id);
        var redisKeyV2 = DateUtil.GetKeyDriverV2(existingBooking.BookingAt, existingBooking.TruckNumber!.Value,
            schedule.GroupId.Value, schedule.Id);

        var checkExistQueue = await _redisService.KeyExistsQueueAsync(redisKeyV2);

        var scheduleBooking = await _unitOfWork.ScheduleBookingRepository.GetByShard(date);
        var listAssignments = new List<Assignment>();
        var countDriver = await _redisService.CheckQueueCountAsync(redisKey);

        if (checkExistQueue == false)
        {
            var timeExpiryRedisQueue = DateUtil.TimeUntilEndOfDay(existingBooking.BookingAt!.Value);

            var driverIds =
                await _unitOfWork.UserRepository.GetUsersWithTruckCategoryIdAsync(existingBooking!.TruckNumber!
                    .Value, schedule.GroupId.Value);
            await _redisService.EnqueueMultipleAsync(redisKey, driverIds, timeExpiryRedisQueue);
            await _redisService.EnqueueMultipleAsync(redisKeyV2, driverIds, timeExpiryRedisQueue);
            var driverNumberBooking = existingBooking.DriverNumber!.Value;
            if (driverNumberBooking > driverIds.Count)
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.AssignmentUpdateFail);
                return result;
            }

            // đánh tag pass 
            await AssignDriversNotDequeueToBooking(
                bookingId,
                redisKey,
                existingBooking.BookingAt.Value,
                (int)countDriver,
                existingBooking.EstimatedDeliveryTime ?? 3,
                listAssignments,
                scheduleBooking!.Id,
                bookingDetailTruck,
                existingBooking.Assignments.ToList()
            );

            result.AddResponseStatusCode(StatusCode.Ok, "Assignments successfully added.", responses);
        }
        else
        {
            int countDriverNumberBooking = existingBooking.DriverNumber!.Value; // count driver need in booking

            if (countDriver >= countDriverNumberBooking)
            {
                await AssignDriversNotDequeueToBooking(
                    bookingId,
                    redisKey,
                    existingBooking.BookingAt!.Value,
                    (int)countDriver,
                    existingBooking.EstimatedDeliveryTime ?? 3,
                    listAssignments,
                    scheduleBooking!.Id,
                    bookingDetailTruck,
                    existingBooking.Assignments.ToList()
                );
                result.AddResponseStatusCode(StatusCode.Ok, "Assignments successfully added.", responses);
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

                if (countDriver > 0)
                {
                    countRemaining -= (int)countDriver;

                    await AssignDriversNotDequeueToBooking(
                        bookingId,
                        redisKey,
                        existingBooking.BookingAt!.Value,
                        (int)countDriver,
                        existingBooking.EstimatedDeliveryTime ?? 3,
                        listAssignments,
                        scheduleBooking!.Id,
                        bookingDetailTruck,
                        existingBooking.Assignments.ToList()
                    );
                    countDriverNumberBooking -= (int)countDriver;
                }

                await AllocateDriversToBookingAsync(
                    existingBooking,
                    existingBooking.BookingAt!.Value,
                    (int)countRemaining,
                    existingBooking.EstimatedDeliveryTime ?? 3,
                    listAssignments,
                    assignedDriverAvailable1Hours,
                    assignedDriverAvailable2Hours,
                    assignedDriverAvailableOther,
                    scheduleBooking!.Id,
                    bookingDetailTruck
                );
                result.AddResponseStatusCode(StatusCode.Ok, "Assignments successfully added.", responses);
            }
        }

        //var result = new OperationResult<AssignManualDriverResponse>();

        var listAssignmentResponse = _mapper.Map<List<AssignmentResponse>>(listAssignments);
        var userIds = listAssignments
            .Where(a => a.UserId.HasValue)
            .Select(a => a.UserId.Value)
            .Distinct()
            .ToList();

        // Step 2: Fetch User Details from Repository (assuming a method exists)
        var users = await _unitOfWork.UserRepository.GetByListIdsAsync(userIds, includeProperties: "Role");

        // Step 3: Map User Details to UserResponse
        var userResponses = _mapper.Map<List<UserResponse>>(users);
        var relatedAssignments = existingBooking.Assignments?
            .Where(a => a.StaffType == RoleEnums.DRIVER.ToString())
            .ToList();


        // Map Assignments to AssignmentResponse
        var assignmentInBooking = _mapper.Map<List<AssignmentResponse>>(relatedAssignments);
        responses.AssignmentInBooking.AddRange(assignmentInBooking);
        responses.BookingNeedStaffs = existingBooking.DriverNumber.Value;
        var isGroup1 = schedule.GroupId == 1 ? true : false;
        responses.CountStaffInslots = listAssignmentResponse.Count();
        responses.StaffInSlot.AddRange(userResponses);

        var groupId = isGroup1 ? 2 : 1;

        var listDriverNeed = await _unitOfWork.UserRepository
            .GetWithTruckCategoryIdAsync(existingBooking.TruckNumber.Value, groupId, includeProperties: "Role");

        var userAssignedIds = existingBooking.Assignments
            .Where(a => a.StaffType == RoleEnums.DRIVER.ToString())
            .Where(a => a.UserId.HasValue)
            .Select(a => a.UserId.Value)
            .Distinct()
            .ToList();

        var usersNotAssigned = listDriverNeed
            .Where(user => !userAssignedIds.Contains(user.Id))
            .ToList();

        var listUserResponse = _mapper.Map<List<UserResponse>>(usersNotAssigned);
        responses.OtherStaffs.AddRange(listUserResponse);

        responses.CountOtherStaff = responses.OtherStaffs.Count();
        responses.StaffType = RoleEnums.DRIVER.ToString();
        if (listAssignmentResponse.Count() >= responses.BookingNeedStaffs)
        {
            responses.IsSuccessed = true;
            bookingDetailTruck.Status = BookingDetailStatusEnums.AVAILABLE.ToString();
            await _unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetailTruck);

            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.AssignmentManual, responses);
        }
        else
        {
            responses.IsSuccessed = false;
            bookingDetailTruck.Status = BookingDetailStatusEnums.AVAILABLE.ToString();
            await _unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetailTruck);
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.FailMessage.AssignmentManual, responses);
        }

        return result;
    }

    public async Task<OperationResult<DriverInfoDTO>> GetAvailablePortersForBooking(int bookingId)
    {
        var result = new OperationResult<DriverInfoDTO>();
        var response = new DriverInfoDTO();
        var existingBooking =
            await _unitOfWork.BookingRepository.GetByIdAsync(bookingId, includeProperties: "Assignments");
        if (existingBooking == null)
        {
            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundBooking);
            return result;
        }

        if (existingBooking.PorterNumber == 0)
        {
            response.BookingNeedStaffs = 0;
            response.IsSuccessed = true;
            response.StaffType = RoleEnums.PORTER.ToString();
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingHasNoPorter, response);
            return result;
        }

        var bookingDetail =
            await _unitOfWork.BookingDetailRepository.GetAsyncByTypeAndBookingId(
                TypeServiceEnums.PORTER.ToString(), bookingId);

        if (bookingDetail == null)
        {
            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundBookingDetail);
            return result;
        }

        var schedule =
            await _unitOfWork.ScheduleWorkingRepository.GetScheduleByBookingAtAsync(existingBooking.BookingAt.Value);

        DateTime endTime;

        if (!existingBooking.EstimatedDeliveryTime.HasValue)
        {
            GoogleMapDTO? googleMapDto = null;
            googleMapDto =
                await _googleMapsService.GetDistanceAndDuration(existingBooking.PickupPoint!,
                    existingBooking.DeliveryPoint!);
            var duration = googleMapDto.Duration.Value;
            var durationEndtime = duration / 3600.0 + 1.5;
            endTime = existingBooking.BookingAt!.Value.AddHours(durationEndtime);
        }

        else
        {
            endTime = existingBooking.BookingAt!.Value.AddHours(existingBooking.EstimatedDeliveryTime.HasValue
                ? existingBooking.EstimatedDeliveryTime.Value
                : 2);
        }

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
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.AssignmentUpdateFail);
                return result;
            }

            // đánh tag pass 
            await AssignPortersNotDequeueToBooking(
                bookingId,
                redisKey,
                existingBooking.BookingAt.Value,
                porterIds.Count,
                existingBooking.EstimatedDeliveryTime ?? 3,
                listAssignments,
                scheduleBooking!.Id,
                bookingDetail,
                existingBooking.Assignments.ToList()
            );
            result.AddResponseStatusCode(StatusCode.Ok, "Assignments successfully added.", response);
        }
        else
        {
            int countporterNumberBooking = existingBooking.DriverNumber!.Value;
            var countPorter = await _redisService.CheckQueueCountAsync(redisKey);
            if (countPorter >= countporterNumberBooking)
            {
                await AssignPortersNotDequeueToBooking(
                    bookingId,
                    redisKey,
                    existingBooking.BookingAt!.Value,
                    (int)countPorter,
                    existingBooking.EstimatedDeliveryTime ?? 3,
                    listAssignments,
                    scheduleBooking!.Id,
                    bookingDetail,
                    existingBooking.Assignments.ToList()
                );
                result.AddResponseStatusCode(StatusCode.Ok, "Assignments successfully added.", response);
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


                if (countPorter > 0)
                {
                    countRemaining -= (int)countPorter;

                    await AssignPortersNotDequeueToBooking(
                        bookingId,
                        redisKey,
                        existingBooking.BookingAt!.Value,
                        (int)countPorter,
                        existingBooking.EstimatedDeliveryTime ?? 3,
                        listAssignments,
                        scheduleBooking!.Id,
                        bookingDetail,
                        existingBooking.Assignments.ToList()
                    );
                    countporterNumberBooking -= (int)countPorter;
                }

                await AllocatePortersToBookingAsync(
                    existingBooking,
                    existingBooking.BookingAt!.Value,
                    (int)countRemaining,
                    existingBooking.EstimatedDeliveryTime ?? 3,
                    listAssignments,
                    assignedPortersAvailable1Hours,
                    assignedPortersAvailable2Hours,
                    assignedPortersAvailableOther,
                    scheduleBooking!.Id,
                    bookingDetail
                );
                result.AddResponseStatusCode(StatusCode.Ok, "Assignments successfully added.", response);
            }
        }

        //var result = new OperationResult<AssignManualDriverResponse>();

        var listAssignmentResponse = _mapper.Map<List<AssignmentResponse>>(listAssignments);
        var userIds = listAssignments
            .Where(a => a.UserId.HasValue)
            .Select(a => a.UserId.Value)
            .Distinct()
            .ToList();

        // Step 2: Fetch User Details from Repository (assuming a method exists)
        var users = await _unitOfWork.UserRepository.GetByListIdsAsync(userIds, includeProperties: "Role");

        // Step 3: Map User Details to UserResponse
        var userResponses = _mapper.Map<List<UserResponse>>(users);
        var relatedAssignments = existingBooking.Assignments?
            .Where(a => a.StaffType == RoleEnums.PORTER.ToString())
            .ToList();


        // Map Assignments to AssignmentResponse
        var assignmentInBooking = _mapper.Map<List<AssignmentResponse>>(relatedAssignments);
        response.AssignmentInBooking.AddRange(assignmentInBooking);
        response.BookingNeedStaffs = existingBooking.PorterNumber.Value;
        var isGroup1 = schedule.GroupId == 1 ? true : false;
        response.CountStaffInslots = listAssignmentResponse.Count();
        response.StaffInSlot.AddRange(userResponses);
        response.StaffType = RoleEnums.PORTER.ToString();

        var groupId = isGroup1 ? 2 : 1;

        var listDriverNeed = await _unitOfWork.UserRepository
            .GetUserWithRoleIdAndGroupIdAsync(5, groupId, includeProperties: "Role");

        var userAssignedIds = existingBooking.Assignments
            .Where(a => a.StaffType == RoleEnums.PORTER.ToString())
            .Where(a => a.UserId.HasValue)
            .Select(a => a.UserId.Value)
            .Distinct()
            .ToList();

        var usersNotAssigned = listDriverNeed
            .Where(user => !userAssignedIds.Contains(user.Id))
            .ToList();

        var listUserResponse = _mapper.Map<List<UserResponse>>(usersNotAssigned);
        response.OtherStaffs.AddRange(listUserResponse);

        response.CountOtherStaff = response.OtherStaffs.Count();
        if (listAssignmentResponse.Count() >= response.BookingNeedStaffs)
        {
            response.IsSuccessed = true;
            bookingDetail.Status = BookingDetailStatusEnums.AVAILABLE.ToString();
            await _unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetail);

            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.AssignmentManual, response);
        }
        else
        {
            response.IsSuccessed = false;
            bookingDetail.Status = BookingDetailStatusEnums.AVAILABLE.ToString();
            await _unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetail);
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.FailMessage.AssignmentManual, response);
        }

        return result;
    }

    private async Task<List<Assignment>> AssignDriversNotDequeueToBooking(int bookingId, string redisKey,
        DateTime startTime, int driverCount, double estimatedDeliveryTime, List<Assignment> listAssignments,
        int scheduleBookingId, BookingDetail bookingDetail, List<Assignment> existingAssignments)
    {
        var existingUserIds = listAssignments
            .Select(a => a.UserId)
            .Concat(existingAssignments
                .Where(a => a.StaffType == RoleEnums.DRIVER.ToString())
                .Select(a => a.UserId))
            .ToHashSet();
        var availableDrivers = await _redisService.PeekAsync<int>(redisKey);
        foreach (var driverId in availableDrivers)
        {
            if (existingUserIds.Contains(driverId))
            {
                continue; // Bỏ qua nếu tài xế đã được gán
            }

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

    private async Task<List<Assignment>> AssignPortersNotDequeueToBooking(int bookingId, string redisKey,
        DateTime startTime,
        int porterCount,
        double estimatedDeliveryTime, List<Assignment> listAssignments,
        int scheduleBookingId, BookingDetail bookingDetail,
        List<Assignment> existingAssignments)
    {
        var existingUserIds = listAssignments
            .Select(a => a.UserId)
            .Concat(existingAssignments
                .Where(a => a.StaffType == RoleEnums.PORTER.ToString())
                .Select(a => a.UserId))
            .ToHashSet();
        var availablePorters = await _redisService.PeekAsync<int>(redisKey);

        foreach (var porterId in availablePorters)
        {
            if (existingUserIds.Contains(porterId))
            {
                continue; // Bỏ qua nếu porter đã được gán
            }


            var endTime = startTime.AddHours(estimatedDeliveryTime);
            var newAssignmentporter = new Assignment()
            {
                BookingId = bookingId,
                StaffType = RoleEnums.PORTER.ToString(),
                Status = AssignmentStatusEnums.WAITING.ToString(),
                UserId = porterId,
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


    public async Task<OperationResult<List<BookingDetailReport>>> GetAll(GetAllBookingDetailReport request)
    {
        var result = new OperationResult<List<BookingDetailReport>>();

        var pagin = new Pagination();

        var filter = request.GetExpressions();

        try
        {
            // Fetch the entities with pagination and count
            var entities = _unitOfWork.BookingDetailRepository.GetWithCount(
                filter: request.GetExpressions(),
                pageIndex: request.page,
                pageSize: request.per_page,
                orderBy: request.GetOrder(),
                includeProperties: "Booking.User"
            );

            // Map to BookingDetailReport
            var listResponse = _mapper.Map<List<BookingDetailReport>>(entities.Data);

            if (listResponse == null || !listResponse.Any())
            {
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListBookingEmpty,
                    listResponse);
                return result;
            }

            // Populate the number field based on the type
            foreach (var report in listResponse)
            {
                // Get the associated booking
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(report.BookingId.Value);
                if (booking != null)
                {
                    // If Type is DRIVER, calculate the TruckNumber
                    if (report.Type == TypeServiceEnums.TRUCK.ToString())
                    {
                        var truckAssignmentsCount = booking.Assignments
                            .Where(assignment =>
                                assignment.StaffType == TypeServiceEnums.TRUCK.ToString() &&
                                assignment.Status != AssignmentStatusEnums.WAITING.ToString())
                            .Count();

                        report.Number = booking.DriverNumber - truckAssignmentsCount;
                        report.BookingAt = booking.BookingAt;
                    }

                    // If Type is PORTER, calculate the PorterNumber
                    if (report.Type == TypeServiceEnums.PORTER.ToString())
                    {
                        var porterAssignmentsCount = booking.Assignments
                            .Where(assignment =>
                                assignment.StaffType == TypeServiceEnums.PORTER.ToString() &&
                                assignment.Status != AssignmentStatusEnums.WAITING.ToString())
                            .Count();

                        report.Number = booking.PorterNumber - porterAssignmentsCount;
                        report.BookingAt = booking.BookingAt;
                    }
                }
            }

            // Add pagination details
            pagin.pageSize = request.per_page;
            pagin.totalItemsCount = entities.Count;

            // Return success response
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListBookingSuccess,
                listResponse, pagin);

            return result;
        }
        catch (Exception e)
        {
            result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            return result;
        }
    }

    public async Task<OperationResult<BookingDetailWaitingResponse>> StaffReportFail(int assignmentId,
        FailReportRequest request)
    {
        var result = new OperationResult<BookingDetailWaitingResponse>();
        try
        {
            var assignment = await _unitOfWork.AssignmentsRepository.GetByIdAsync(assignmentId);
            if (assignment == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundAssignment);
                return result;
            }

            assignment.Status = AssignmentStatusEnums.FAILED.ToString();
            assignment.FailedReason = request.FailReason;
            _unitOfWork.AssignmentsRepository.Update(assignment);

            var bookingDetail =
                await _unitOfWork.BookingDetailRepository.GetByIdAsync((int)assignment.BookingDetailsId);
            if (bookingDetail == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundBookingDetail);
                return result;
            }

            bookingDetail.Status = BookingDetailStatusEnums.WAITING.ToString();


            await _unitOfWork.BookingDetailRepository.SaveOrUpdateAsync(bookingDetail);

            // Create notification
            var user = await _unitOfWork.UserRepository.GetManagerAsync();
            var staffLeader = await _unitOfWork.UserRepository.GetByIdAsync((int)assignment.UserId);
            var notification = new Notification
            {
                BookingId = bookingDetail.BookingId,
                UserId = user.Id,
                SentFrom = staffLeader?.Name,
                Receive = user.Name,
                Name = "Service Fail Notification",
                Description = $"Booking with id {bookingDetail.BookingId} has an issue: {request.FailReason}",
                Topic = "StaffReportFail",
                IsRead = false
            };

            await _unitOfWork.NotificationRepository.AddAsync(notification);
            var saveResult = await _unitOfWork.SaveChangesAsync();

            if (saveResult > 0)
            {
                var assignments =
                    await _unitOfWork.AssignmentsRepository.GetAssignmentByBookingDetailIdAndStatusAsync(
                        bookingDetail.Id, AssignmentStatusEnums.FAILED.ToString());
                var responseAssignment = _mapper.Map<List<AssignmentResponse>>(assignments);
                bookingDetail = await _unitOfWork.BookingDetailRepository.GetByIdAsync((int)bookingDetail.Id);
                var response = _mapper.Map<BookingDetailWaitingResponse>(bookingDetail);
                response.Assignments = responseAssignment; // Populate assignments in the response
                string Id = notification.Id + "-" + bookingDetail.BookingId;
                await _firebaseServices.SaveMailManager(notification, Id, "reports");

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.BookingDetailUpdateSuccess,
                    response);
            }
            else
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingUpdateFail);
            }
        }
        catch (Exception ex)
        {
            result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
        }

        return result;
    }
}