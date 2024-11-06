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
    private readonly ILogger<AssignDriverWorker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AssignDriverWorker(ILogger<AssignDriverWorker> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }
/*
Quy trình nâng cấp tự động gán tài xế:

1. Kiểm tra shard:
   - Nếu không tồn tại:
     - Tạo mới shard.
     - Tìm danh sách tài xế có loại xe và lịch làm việc phù hợp.
     - Bổ sung: Kiểm tra xem tài xế có đang thực hiện nhiệm vụ ưu tiên cao hay không.
     - Thêm vào Redis queue _booking_at.
     - Thêm vào danh sách phân công (assignment).

   - Nếu shard đã tồn tại:
     - Kiểm tra Redis queue _booking_at:
       - Nếu queue có phần tử:
          - Lấy phần tử từ queue và gán vào assignment.
          -* Kiểm tra lịch sử năng suất*:
            - Nếu tài xế có tần suất từ chối nhiệm vụ cao, giảm ưu tiên gán.

       - Nếu queue rỗng, hoặc không đủ:
         - Kiểm tra chi tiết lịch làm việc (schedule booking detail) của tài xế:
           - Điều kiện kiểm tra:
             - endTime của booking_at > startTime của schedule hoặc startTime của booking < endTime của schedule.
           - Nếu có lịch rảnh:
             - Nếu count > 1:
               - Chọn lịch theo location.
               - Tính rate cho mỗi lịch:
                 - Xem xét loại nhiệm vụ (như hàng hóa nguy hiểm) để điều chỉnh rate.
               - Kiểm tra lịch kết thúc trong vòng từ 1 - 2 giờ:
                 - Tính toán rate:
                   - Trong vòng 1 giờ: rate += 1.
                   - Trong vòng 2 giờ: rate += 0.5.
               - So sánh và chọn tài xế:
                 - Tính toán rate dựa trên khoảng cách: rate = rate / khoảng cách.
                 - ! Bổ sung: Tính toán yếu tố môi trường (thời tiết, tắc đường) vào rate.
             - Nếu count = 1: Chọn ngay.
             - Nếu count < 1: Đánh tag "auto-assign failed, cần review can thiệp".
           - Nếu không có lịch rảnh thỏa điều kiện, đánh tag "auto-assign failed, cần review can thiệp".

2. Bổ sung các tình huống đặc biệt:
   - Trường hợp khẩn cấp - khi nào tình huống khẩn cấp:
     - Khi có yêu cầu khẩn cấp, hệ thống tạm ngừng gán tự động khác và ưu tiên tài xế gần nhất.
   - Dự phòng khi thiếu tài xế:
     - Nếu không có tài xế phù hợp, tự động đặt lệnh chờ (standby) và tìm kiếm manager.
   - Lựa chọn tài xế dự phòng:
     - Nếu tài xế có lịch xung đột, đưa họ vào danh sách dự phòng để tái gán khi tài xế chính không khả dụng.
   - Cảnh báo cho quản lý:
     - Gửi cảnh báo cho quản lý khi tỷ lệ gán thất bại quá cao hoặc không có tài xế phù hợp.
   -* Phân bổ dựa trên xếp hạng - cần công thức tính toán:
     - Tính toán điểm xếp hạng dựa trên hiệu suất, độ uy tín, và phản hồi từ khách hàng để ưu tiên tài xế chất lượng cao cho nhiệm vụ phức tạp.

3. Giám sát và cải tiến quy trình:
   - ! Phân tích định kỳ:
     - Thực hiện phân tích định kỳ để đánh giá hiệu quả của quy trình gán tài xế, từ đó điều chỉnh các tham số và quy định.
   - ! Phản hồi từ tài xế:
     - Tạo kênh phản hồi cho tài xế để thu thập thông tin về quy trình gán và cải tiến dựa trên ý kiến của họ.
   - ! Cập nhật công nghệ:
     - Nghiên cứu và áp dụng các công nghệ mới như AI và machine learning để tự động hóa và tối ưu hóa quy trình gán tài xế.
*/

    /// <summary>
    ///  An automation assign drivers to booking
    /// </summary>
    /// <param name="message">int</param>
    /// <exception cref="NotFoundException"></exception>
    [Consumer("movemate.booking_assign_driver")]
    public async Task HandleMessage(int message)
    {
        await Task.Delay(100);
        Console.WriteLine("movemate.booking_assign_driver");
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // var
                var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();

                // check booking
                var existingBooking = await unitOfWork.BookingRepository.GetByIdAsync(message);

                if (existingBooking == null)
                {
                    throw new NotFoundException(MessageConstant.FailMessage.NotFoundBooking);
                }

                var date = DateUtil.GetShard(existingBooking.BookingAt);
                var redisKey = DateUtil.GetKeyDriver(existingBooking.BookingAt);
                var checkExistQueue = await redisService.KeyExistsQueueAsync(redisKey);

                var scheduleBooking = await unitOfWork.ScheduleBookingRepository.GetByShard(date);
                var listAssignments = new List<Assignment>();

                if (checkExistQueue == false)
                {
                    var timeExpiryRedisQueue = DateUtil.TimeUntilEndOfDay(existingBooking.BookingAt!.Value);

                    var driverIds =
                        await unitOfWork.UserRepository.GetUsersWithTruckCategoryIdAsync(existingBooking!.TruckNumber!
                            .Value);

                    await redisService.EnqueueMultipleAsync(redisKey, driverIds, timeExpiryRedisQueue);

                    // check DriverNumber in booking and driver in system
                    var driverNumberBooking = existingBooking.DriverNumber!.Value;
                    if (driverNumberBooking > driverIds.Count)
                    {
                        // đánh tag faild cần reviewer can thiệp
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
                        scheduleBooking!.Id
                    );
                }
                else
                {
                    var countDriver = await redisService.CheckQueueCountAsync(redisKey);
                    if (countDriver > existingBooking.DriverNumber)
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
                            scheduleBooking!.Id
                        );
                    }
                    // nếu mà list countDriver < existingBooking.DriverNumber thì
                    // 
                    
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
    /// <returns>A task that represents the asynchronous operation of assigning drivers and saving schedule data.</returns>
    private async Task AssignDriversToBooking(int bookingId, string redisKey, DateTime startTime, int driverCount,
        double estimatedDeliveryTime, List<Assignment> listAssignments,
        IRedisService redisService, UnitOfWork unitOfWork, int scheduleBookingId)
    {
        for (int i = 0; i < driverCount; i++)
        {
            var driverId = await redisService.DequeueAsync<int>(redisKey);
            var endTime = startTime.AddHours(estimatedDeliveryTime);

            var newAssignmentDriver = new Assignment()
            {
                BookingId = bookingId,
                StaffType = RoleEnums.DRIVER.ToString(),
                Status = AssignmentStatusEnums.WAITING.ToString(),
                UserId = driverId,
                StartDate = startTime,
                EndDate = endTime,
                IsResponsible = false,
                ScheduleBookingId = scheduleBookingId
            };
           
            listAssignments.Add(newAssignmentDriver);
        }
        
        await unitOfWork.AssignmentsRepository.SaveOrUpdateRangeAsync(listAssignments);
        unitOfWork.Save();
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