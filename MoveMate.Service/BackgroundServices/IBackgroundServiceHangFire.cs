namespace MoveMate.Service.BackgroundServices;

public interface IBackgroundServiceHangFire
{
    public Task StartAllBackgroundJob();
}