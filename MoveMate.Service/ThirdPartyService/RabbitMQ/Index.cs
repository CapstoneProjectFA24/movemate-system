using Microsoft.Extensions.Hosting;
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

        return Task.CompletedTask;
    }
}