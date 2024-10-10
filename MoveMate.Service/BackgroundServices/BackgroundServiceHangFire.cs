using Hangfire;
using MoveMate.Service.Utils;

namespace MoveMate.Service.BackgroundServices;

public class BackgroundServiceHangFire : IBackgroundServiceHangFire
{
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

    }
}