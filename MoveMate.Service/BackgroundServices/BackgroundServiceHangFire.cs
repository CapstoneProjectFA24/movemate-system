using Hangfire;

namespace MoveMate.Service.BackgroundServices;

public class BackgroundServiceHangFire : IBackgroundServiceHangFire
{
    public async Task StartAllBackgroundJob()
    {
        RecurringJob.AddOrUpdate(() => Console.WriteLine("hello from hangfire"),"* * * * *");

        RecurringJob.AddOrUpdate("test job",
            () => Console.WriteLine("hello from hangfire"),
            cronExpression: "* * * * *",
            new RecurringJobOptions
            {
                // sync time(utc +7)
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"),
            });

    }
}