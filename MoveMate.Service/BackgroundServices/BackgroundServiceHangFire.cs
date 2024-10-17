using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.Utils;

namespace MoveMate.Service.BackgroundServices;

public class BackgroundServiceHangFire : IBackgroundServiceHangFire
{
    //private readonly ILogger<BackgroundServiceHangFire> _logger;
    //private readonly IRedisService _redisService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BackgroundServiceHangFire(IServiceScopeFactory serviceScopeFactory)
    {
        //_logger = logger;
        //_redisService = redisService;
        _serviceScopeFactory = serviceScopeFactory;
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

        /*BackgroundJob.Enqueue(() => AddReviewerJob());
        RecurringJob.AddOrUpdate(
            "add-reviewer-job",
            () => AddReviewerJob(),
            "0 1 * * *");*/
    }

    /*public async Task AddReviewerJob()
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
            var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();

            List<int> listReviewer =  await unitOfWork.UserRepository.FindAllUserByRoleIdAsync(2);
            await redisService.EnqueueMultipleAsync("", listReviewer);
        }
    }*/
}