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

public class AssignDriverWorker
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AssignDriverWorker(ILogger logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    [Consumer("movemate.booking_assign_driver_local")]
    public async Task HandleMessage(int message)
    {
        await Task.Delay(100);
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // var
                var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();

                // check booking
                var booking = await unitOfWork.BookingRepository.GetByIdAsync(message);

                if (booking == null)
                {
                    throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
                }

                /*check shard xem co k
                nếu ko thì tạo mới
                   tìm kiếm list driver có loại xe và lịch làm việc đó
                   add vào redis queue
                   add vào assignment
                nếu có
                 thì check các schedule detail xem có lịch nào ko
                 rule check - condition thuận chiều (có booking nào nằm thỏa condition ko):
                 endtime cua booking at > starttime của schedule or starttime của booking < endtime của schedule
                 nếu có tức đang rảnh
                   check count > 1 thì check theo location mà chọn
                    var rate cuả mỗi loại schedule là 1
                       filter xem có lịch nào xong trong vòng 1h - 2h ko
                       (endtime của booking là 9 check xem có lịch startime nào nằm trong 9 + 1 và nhỏ hơn 9 + 2 để assign liên tục)
                       or
                       starttime của booking check xem có lịch endtime nào nằm trong 9 -1 và lớn hớn 9 -2 để assign liên tục
                       nếu có thì nằm trong 1h thì rate là + 1
                       nếu có nằm trong vòng 2h thì rate là +0,5
                       tiếp theo so về location endpoint của booking, công thức là  rate = rate/khoảng cách
                    chọn rate cao nhất mà pick
                   check count = 1 thì pick lun
                   check count < 0 đánh tag là tự động assign failed cần review can thiệp
                  nếu không => đánh tag là tự động assign failed cần review can thiệp
    
                */
                var date = DateUtil.GetShard(booking.BookingAt);

                var schedule = await unitOfWork.ScheduleBookingRepository.GetByShard(date);

                if (schedule == null)
                {
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("AssignDriverWorker - " + e.Message);
            throw;
        }
    }

    //[Consumer("movemate.booking_assign_driver_local")]
    //public async Task HandleMessage(int message)
    //{
    //    try
    //    {
    //        using (var scope = _serviceScopeFactory.CreateScope())
    //        {
    //            var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    //            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    //            var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();

    //            var booking = await unitOfWork.BookingRepository.GetByIdAsync(message);

    //            if (booking == null)
    //            {
    //                throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
    //            }

    //            var driverList = await unitOfWork.UserRepository.GetUsersWithTruckCategoryIdAsync(booking!.TruckNumber!.Value);

    //            string redisKey = DateUtil.GetKeyReview();

    //            var date = DateUtil.GetShard(booking.BookingAt);

    //            var checkBookingStaffDaily =
    //                await unitOfWork.BookingStaffDailyRepository.GetStaffActiveNowBookingStaffDailies(4);

    //            if (checkBookingStaffDaily.Count > 0)
    //            {
    //                var driver = checkBookingStaffDaily.FirstOrDefault();

    //                var driverDetail = new BookingDetail()
    //                {
    //                    BookingId = message,
    //                    Status = AssignmentStatusEnums.ASSIGNED.ToString(),
    //                    UserId = driver!.UserId,
    //                    StaffType = RoleEnums.DRIVER.ToString(),
    //                };

    //                driver.Status = BookingStaffDailyEnums.BUSSY.ToString();

    //                booking.BookingDetails.Add(driverDetail);

    //                booking.Status = AssignmentStatusEnums.ASSIGNED.ToString();

    //                var endtime = booking.BookingAt!.Value.AddHours(booking.EstimatedDeliveryTime ?? 3);
    //                var workDate = new ScheduleDetail()
    //                {
    //                    UserId = driver.UserId,
    //                    WorkingDays = DateTime.Now,
    //                    StartDate = booking.BookingAt,
    //                    EndDate = endtime
    //                };

    //                //save BookingStaffDaily
    //                await unitOfWork.ScheduleDetailRepository.AddAsync(workDate);
    //                unitOfWork.BookingStaffDailyRepository.UpdateRange(checkBookingStaffDaily);
    //            }
    //            else
    //            {
    //                // logic nếu ko đủ
    //                //khó quá à
    //            }


    //            unitOfWork.BookingRepository.Update(booking);
    //            unitOfWork.Save();

    //            Console.WriteLine($"Booking assign_driver info: {booking.Id}");
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        _logger.LogError(e, "Error processing booking review for message {Message}", message);
    //        throw;
    //    }

    //    Console.WriteLine($"Received booking_assign_driver message: {message}");
    //}
}