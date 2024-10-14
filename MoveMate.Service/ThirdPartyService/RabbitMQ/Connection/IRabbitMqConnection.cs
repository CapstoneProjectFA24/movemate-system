using RabbitMQ.Client;

namespace MoveMate.Service.ThirdPartyService.Redis.Connection;

public interface IRabbitMqConnection
{
    IConnection Connection { get; }
}