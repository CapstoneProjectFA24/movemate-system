using Microsoft.Extensions.Hosting;
using MoveMate.Service.ThirdPartyService.Redis.Connection;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Config;

public class RabbitMqWorker : BackgroundService
{
    private readonly IRabbitMqConsumer _consumer;

    public RabbitMqWorker(IRabbitMqConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.StartConsuming<MyMessageHandler>();

        return Task.CompletedTask;
    }
}