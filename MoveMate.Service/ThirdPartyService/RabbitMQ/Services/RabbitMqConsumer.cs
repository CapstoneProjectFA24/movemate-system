using System.Reflection;
using System.Text;
using System.Text.Json;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace MoveMate.Service.ThirdPartyService.Redis.Connection;

public class RabbitMqConsumer : IRabbitMqConsumer, IDisposable
{
    private readonly IRabbitMqConnection _connection;
    private readonly IModel _channel;

    public RabbitMqConsumer(IRabbitMqConnection connection)
    {
        _connection = connection;
        _channel = _connection.Connection.CreateModel();
    }
    
    public void StartConsuming<T>() where T : class
    {
        var methods = typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public);

        foreach (var method in methods)
        {
            var consumerAttribute = method.GetCustomAttribute<ConsumerAttribute>();
            if (consumerAttribute != null)
            {
                string queueName = consumerAttribute.QueueName;
                StartListening(queueName, method, Activator.CreateInstance<T>());
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
            _channel.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);
        }
    }
    
    private void StartListening(string queueName, MethodInfo method, object instance)
    {
        //DeclareQueueIfNotExists(queueName);
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var jsonString = Encoding.UTF8.GetString(body);

            // Deserialize message to the expected type (if necessary)
            var message = JsonSerializer.Deserialize<object>(jsonString); // Use the correct type here

            // Invoke the method with the deserialized message
            method.Invoke(instance, new object[] { message });
        };

        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        //DeclareQueueIfNotExists(queueName);
        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Connection.Close();
    }
}