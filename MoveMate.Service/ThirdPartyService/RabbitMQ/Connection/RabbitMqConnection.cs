using RabbitMQ.Client;

namespace MoveMate.Service.ThirdPartyService.Redis.Connection;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private IConnection? _connection;
    public IConnection Connection => _connection;

    public RabbitMqConnection()
    {
        InitConnection();
    }

    private void InitConnection()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "14.225.204.144",
            UserName = "root",
            Password = "admin123",
            VirtualHost = "/"
        };

        _connection = factory.CreateConnection();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}