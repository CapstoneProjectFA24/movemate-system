using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.Utils;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker;

public class AssginDriverWorker
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AssginDriverWorker(ILogger logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    [Consumer("movemate.booking_assign_driver_local")]
    public async Task HandleMessage(int message)
    {
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                
                var booking = await unitOfWork.BookingRepository.GetByIdAsync(message);

                if (booking == null)
                {
                    throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
                }
                
                var driverList = await unitOfWork.UserRepository.GetUsersWithTruckCategoryIdAsync(booking!.TruckNumber!.Value);
                
                var bookingDate = DateUtil.GetDateFormat(booking!.BookingAt!.Value);
                
                //var schedule = unitOfWork.ScheduleRepository.get                                
                string redisKey = DateUtil.GetKeyReview();

                var date = DateUtil.GetShard(booking.BookingAt);

                /*var checkBookingStaffDaily =
                    await unitOfWork.BookingStaffDailyRepository.GetStaffActiveNowBookingStaffDailies(4);*/

                if (driverList.Count > 0)
                {
                    for (int i = 0; i < booking.TruckNumber; i++)
                    {

                    }
                    
                    var driver = driverList.FirstOrDefault();

                    var driverDetail = new BookingDetail()
                    {
                        BookingId = message,
                        Status = BookingDetailStatus.ASSIGNED.ToString(),
                        UserId = driver!.Id,
                        StaffType = RoleEnums.DRIVER.ToString(),
                    };

                    //driver.Status = BookingStaffDailyEnums.BUSSY.ToString();

                    booking.BookingDetails.Add(driverDetail);

                    booking.Status = BookingDetailStatus.ASSIGNED.ToString();

                    var endtime = booking.BookingAt!.Value.AddHours(booking.EstimatedDeliveryTime ?? 3);
                    /*var workDate = new ScheduleDetail()
                    {
                        UserId = driver.UserId,
                        WorkingDays = DateTime.Now,
                        StartDate = booking.BookingAt,
                        EndDate = endtime
                    };*/

                    //save BookingStaffDaily
                    /*await unitOfWork.ScheduleDetailRepository.AddAsync(workDate);
                    unitOfWork.BookingStaffDailyRepository.UpdateRange(checkBookingStaffDaily);*/
                    
                    await unitOfWork.BookingDetailRepository.AddAsync(driverDetail);
                }
                else
                {
                    // logic nếu ko đủ
                    //khó quá à
                }

                //unitOfWork.
                unitOfWork.Save();

                Console.WriteLine($"Booking assign_driver info: {booking.Id}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing booking review for message {Message}", message);
            throw;
        }

        Console.WriteLine($"Received booking_assign_driver message: {message}");
    }
}