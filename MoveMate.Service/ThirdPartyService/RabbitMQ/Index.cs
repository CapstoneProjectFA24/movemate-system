using Microsoft.Extensions.Hosting;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Worker;
using MoveMate.Service.ThirdPartyService.Redis.Connection;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Config;

public class Index : BackgroundService
{
    private readonly IRabbitMqConsumer _consumer;

    public Index(IRabbitMqConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.StartConsuming<MyMessageHandlerWorker>();
        _consumer.StartConsuming<AssignReviewWorker>();
        _consumer.StartConsuming<SetScheduleReview>();
        _consumer.StartConsuming<AssignDriverWorker>();
        _consumer.StartConsuming<NotificationWorker>();
        _consumer.StartConsuming<PushToFirebaseWorker>();
        return Task.CompletedTask;
    }
}