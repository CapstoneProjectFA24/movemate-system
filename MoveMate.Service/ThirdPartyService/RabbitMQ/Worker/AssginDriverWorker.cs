using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
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

    [Consumer("movemate.booking_assign_driver")]
    public async Task HandleMessage(int message)
    {
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = (UnitOfWork) scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                
                
                var booking = await unitOfWork.BookingRepository.GetByIdAsync(message);

                string redisKey = DateUtil.GetKeyReview();

                var date = DateUtil.GetShard(booking.BookingAt);

                var checkBookingStaffDaily = await unitOfWork.BookingStaffDailyRepository.GetStaffActiveNowBookingStaffDailies(4);

                if(checkBookingStaffDaily.Count > 0)
                {
                    var driver = checkBookingStaffDaily.FirstOrDefault();
                    
                    var driverDetail = new BookingDetail()
                    {
                        BookingId = message,
                        Status = BookingDetailStatus.ASSIGNED.ToString(),
                        UserId = driver!.UserId,
                        StaffType = RoleEnums.DRIVER.ToString(),
                    };
                    
                    driver.Status = BookingStaffDailyEnums.BUSSY.ToString();
                    
                    booking.BookingDetails.Add(driverDetail);
                
                    booking.Status = BookingDetailStatus.ASSIGNED.ToString();
                    
                    var workDate = new ScheduleDetail()
                    {
                        UserId = driver.UserId,
                        WorkingDays = DateTime.Now,
                        
                    }
                    
                    //save BookingStaffDaily
                    unitOfWork.BookingStaffDailyRepository.UpdateRange(checkBookingStaffDaily);

                }
                else
                {
                    // logic nếu ko đủ
                    //khó quá à
                    
                    
                    
                }
                
               
                unitOfWork.BookingRepository.Update(booking);
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