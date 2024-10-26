using Hangfire;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.Utils;

namespace MoveMate.Service.BackgroundServices;

public class BackgroundServiceHangFire : IBackgroundServiceHangFire
{
    private readonly ILogger<BackgroundServiceHangFire> _logger;
    private readonly IRedisService _redisService;
    private readonly IFirebaseServices _firebaseServices;

    private readonly UnitOfWork _unitOfWork;

    public BackgroundServiceHangFire(ILogger<BackgroundServiceHangFire> logger, IRedisService redisService,
        IUnitOfWork unitOfWork, IFirebaseServices firebaseServices)
    {
        _logger = logger;
        _redisService = redisService;
        _firebaseServices = firebaseServices;
        _unitOfWork = (UnitOfWork)unitOfWork;
    }

    public async Task StartAllBackgroundJob()
    {
        _logger.LogInformation("Starting all background jobs");

        RecurringJob.AddOrUpdate("test job",
            () => Console.WriteLine("Hello from Hangfire test job"),
            cronExpression: "* * * * *",
            new RecurringJobOptions
            {
                TimeZone = DateUtil.GetSEATimeZone(),
            });

        BackgroundJob.Enqueue(() => AddStaffJob());
        RecurringJob.AddOrUpdate(
            "add-staff-job",
            () => AddStaffJob(),
            "0 1 * * *", // Runs daily at 1 AM
            new RecurringJobOptions
            {
                TimeZone = DateUtil.GetSEATimeZone(),
            });

        BackgroundJob.Enqueue(() => DeleteCanceledBookingsJob());
        RecurringJob.AddOrUpdate(
            "delete-canceled-bookings-job",
            () => DeleteCanceledBookingsJob(),
            "0 2 * * *", // Runs daily at 2 AM
            new RecurringJobOptions
            {
                TimeZone = DateUtil.GetSEATimeZone(),
            }
        );

        _logger.LogInformation("All background jobs started successfully");
    }

    public async Task AddStaffJob()
    {
        try
        {
            _logger.LogInformation("Starting AddStaffJob at {Time}", DateTime.Now);

            List<int> listReviewer = await _unitOfWork.UserRepository.FindAllUserByRoleIdAsync(2);
            string redisKey = DateUtil.GetKeyReview();

            await _redisService.EnqueueMultipleAsync(redisKey, listReviewer);

            _logger.LogInformation("AddStaffJob completed successfully at {Time}", DateTime.Now);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in AddStaffJob: {Message}", e.Message);
            throw;
        }
    }

    public async Task DeleteCanceledBookingsJob()
    {
        try
        {
            _logger.LogInformation("Starting DeleteCanceledBookingsJob at {Time}", DateTime.Now);

            var threeDaysAgo = DateTime.Now.AddDays(-3);
            var canceledBookings = await _firebaseServices.GetCanceledBookingsOlderThanAsync(threeDaysAgo);

            foreach (var booking in canceledBookings)
            {
                await _firebaseServices.Delete(booking.Id.ToString(), "bookings");
            }

            _logger.LogInformation("Deleted {Count} canceled bookings older than 3 days at {Time}",
                canceledBookings.Count, DateTime.Now);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting canceled bookings: {Message}", e.Message);
            throw;
        }
    }
}