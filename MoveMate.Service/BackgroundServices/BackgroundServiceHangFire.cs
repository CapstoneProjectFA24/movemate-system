using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.Utils;

namespace MoveMate.Service.BackgroundServices;

public class BackgroundServiceHangFire : IBackgroundServiceHangFire
{
    private readonly ILogger<BackgroundServiceHangFire> _logger;

    private readonly IRedisService _redisService;

    //private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly UnitOfWork _unitOfWork;

    public BackgroundServiceHangFire(ILogger<BackgroundServiceHangFire> logger, IRedisService redisService,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _redisService = redisService;
        _unitOfWork = (UnitOfWork)unitOfWork;
    }

    public async Task StartAllBackgroundJob()
    {
        //RecurringJob.AddOrUpdate(() => Console.WriteLine("hello from hangfire normal"),"* * * * *");

        RecurringJob.AddOrUpdate("test job",
            () => Console.WriteLine("hello from hangfire test job"),
            cronExpression: "* * * * *",
            new RecurringJobOptions
            {
                // sync time(utc +7)
                TimeZone = DateUtil.GetSEATimeZone(),
            });

        BackgroundJob.Enqueue(() => AddStaffJob());
        RecurringJob.AddOrUpdate(
            "add-staff-job",
            () => AddStaffJob(),
            "0 1 * * *",
            new RecurringJobOptions
            {
                // sync time(utc +7)
                TimeZone = DateUtil.GetSEATimeZone(),
            });
    }

    public async Task AddStaffJob()
    {
        try
        {
            List<int> listReviewer = await _unitOfWork.UserRepository.FindAllUserByRoleIdAsync(2);
            string redisKey = DateUtil.GetKeyReview();

            //List<BookingStaffDaily> listDriver =
            //    await _unitOfWork.BookingStaffDailyRepository.GetBookingStaffDailiesNow(4);


            //foreach (var driver in listDriver)
            //{
            //    driver.Status = BookingStaffDailyEnums.CLOSE.ToString();
            //}

            //_unitOfWork.BookingStaffDailyRepository.UpdateRange(listDriver);

            //List<int> drivers = await _unitOfWork.UserRepository.FindAllUserByRoleIdAsync(4);

            //List<BookingStaffDaily> newListDriver = new List<BookingStaffDaily>();
            //foreach (var driver in drivers)
            //{
            //    var newDriver = new BookingStaffDaily()
            //    {
            //        AddressCurrent = "10.841140522859849, 106.80987597826075",
            //        UserId = driver,
            //        IsActived = true,
            //        CreatedAt = DateTime.Now,
            //        UpdatedAt = DateTime.Now,
            //        Status = BookingStaffDailyEnums.NOW.ToString(),
            //    };
            //    newListDriver.Add(newDriver);
            //}

            //await _unitOfWork.BookingStaffDailyRepository.AddRangeAsync(newListDriver);

            //var check = await _unitOfWork.SaveChangesAsync();
            await _redisService.EnqueueMultipleAsync(redisKey, listReviewer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }

        

        Console.WriteLine($"BackgroundServiceHangFire AddStaffJob: {DateTime.Now}");
    }
}