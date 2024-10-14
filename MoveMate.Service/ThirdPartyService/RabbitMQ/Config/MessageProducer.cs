using System.Text;
using System.Text.Json;
using MoveMate.Service.ThirdPartyService.Redis.Connection;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ;

public class MessageProducer : IMessageProducer
{
    private readonly IRabbitMqConnection _connection;
    private readonly IModel _channel;

    public MessageProducer(IRabbitMqConnection connection)
    {
        _connection = connection;
        _channel = _connection.Connection.CreateModel();

    }

    public void SendingMessage<T>(T message)
    {
        //using var channel = _connection.Connection.CreateModel();

       // channel.QueueDeclare("chanel-1", durable: true, exclusive: false, autoDelete: false);
       DeclareQueueIfNotExists("chanel-1");
       
        var jsonString = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonString);

        _channel.BasicPublish("", "chanel-1", body: body);
    }
    
    private void DeclareQueueIfNotExists(string queueName)
    {
        try
        {
            _channel.QueueDeclarePassive(queueName);
        }
        catch (OperationInterruptedException)
        {
            _channel.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);
        }
    }
}