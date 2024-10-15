using System.Text;
using System.Text.Json;
using MoveMate.Service.ThirdPartyService.Redis.Connection;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ;

public class MessageProducer : IMessageProducer, IDisposable
{
    private readonly IRabbitMqConnection _connection;
    private readonly IModel _channel;
    private bool _disposed;
    private const int RetryCount = 3;
    private const int DelayMilliseconds = 2000;

    public MessageProducer(IRabbitMqConnection connection)
    {
        _connection = connection;
        _channel = _connection.Connection.CreateModel();
    }

    public void SendingMessage<T>(T message)
    {
        SendingMessage("chanel-1", message);
    }

    public void SendingMessage<T>(string channel, T message)
    {
        DeclareQueueIfNotExists(channel);

        var jsonString = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonString);

        int attempt = 0;
        bool success = false;

        while (attempt < RetryCount && !success)
        {
            try
            {
                _channel.BasicPublish("", channel, body: body);
                success = true; 
            }
            catch (Exception ex)
            {
                attempt++;
                if (attempt >= RetryCount)
                {
                    
                    Console.WriteLine($"Failed to send message after {RetryCount} attempts: {ex.Message}");
                    throw; 
                }
                else
                {
                    
                    Console.WriteLine($"Attempt {attempt} failed, retrying in {DelayMilliseconds} ms: {ex.Message}");
                    Thread.Sleep(DelayMilliseconds);
                }
            }
        }
    }

    private void DeclareQueueIfNotExists(string queueName)
    {
        try
        {
            _channel.QueueDeclarePassive(queueName);
        }
        catch (OperationInterruptedException)
        {
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _channel?.Close();
            _channel?.Dispose();
        }

        _disposed = true;
    }
}