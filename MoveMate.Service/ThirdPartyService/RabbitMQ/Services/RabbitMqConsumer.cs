using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly int _maxRetryAttempts;
    private readonly string? _deadLetterQueue;

    public RabbitMqConsumer(IRabbitMqConnection connection, ILogger<RabbitMqConsumer> logger,
        IConfiguration configuration)
    {
        _connection = connection;
        _logger = logger;
        _channel = _connection.Connection.CreateModel();
        _maxRetryAttempts = configuration.GetValue<int>("RabbitMQ:MaxRetryAttempts");
        _deadLetterQueue = configuration.GetValue<string>("RabbitMQ:DeadLetterQueue");
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
        DeclareQueueIfNotExists(queueName);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var jsonString = Encoding.UTF8.GetString(body);

            try
            {
                var expectedType = method.GetParameters()[0].ParameterType;

                var message = JsonSerializer.Deserialize(jsonString, expectedType);

                if (message != null && expectedType.IsInstanceOfType(message))
                {
                    method.Invoke(instance, new object[] { message });
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    _logger.LogError(
                        "Deserialized message type mismatch. Expected: {ExpectedType}, Actual: {ActualType}",
                        expectedType.Name, message?.GetType().Name);
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from queue {QueueName}", queueName);

                int retryCount = GetRetryCount(ea);
                if (retryCount >= _maxRetryAttempts)
                {
                    MoveToDeadLetterQueue(ea, queueName);
                }
                else
                {
                    RetryMessage(ea, retryCount);
                }
            }
        };

        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        //DeclareQueueIfNotExists(queueName);
        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
    }

    private int GetRetryCount(BasicDeliverEventArgs ea)
    {
        if (ea.BasicProperties.Headers != null &&
            ea.BasicProperties.Headers.TryGetValue("x-retry-count", out var retryHeader))
        {
            return Convert.ToInt32(retryHeader);
        }

        return 0;
    }

    private void RetryMessage(BasicDeliverEventArgs ea, int currentRetryCount)
    {
        var properties = ea.BasicProperties;
        properties.Headers ??= new Dictionary<string, object>();
        properties.Headers["x-retry-count"] = currentRetryCount + 1;

        _channel.BasicPublish(exchange: "", routingKey: ea.RoutingKey, basicProperties: properties, body: ea.Body);
        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }

    private void MoveToDeadLetterQueue(BasicDeliverEventArgs ea, string queueName)
    {
        _channel.BasicPublish(exchange: "", routingKey: _deadLetterQueue, basicProperties: ea.BasicProperties,
            body: ea.Body);
        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="queueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? GetMessageFromQueue<T>(string queueName) where T : class
    {
        DeclareQueueIfNotExists(queueName); // Đảm bảo queue tồn tại

        var result = _channel.BasicGet(queueName, autoAck: false); // Lấy message từ queue mà không auto ack

        if (result != null)
        {
            var body = result.Body.ToArray();
            var jsonString = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Message received from queue {QueueName}: {Message}", queueName, jsonString);

            try
            {
                // Deserialize JSON message into the expected type
                var message = JsonSerializer.Deserialize<T>(jsonString);

                if (message != null)
                {
                    _channel.BasicAck(result.DeliveryTag, false); // Xác nhận đã nhận message thành công
                    return message; // Trả về đối tượng message
                }
                else
                {
                    _logger.LogError("Failed to deserialize message from queue {QueueName}.", queueName);
                    _channel.BasicNack(result.DeliveryTag, false, requeue: true); // Nack và requeue nếu thất bại
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message from queue {QueueName}.", queueName);

                // Retry logic
                int retryCount = GetRetryCount(result.BasicProperties);
                if (retryCount >= _maxRetryAttempts)
                {
                    MoveToDeadLetterQueue(result, queueName);
                }
                else
                {
                    RetryMessage(result, retryCount);
                }
            }
        }

        return null; // Queue trống hoặc không xử lý được message
    }

    private int GetRetryCount(IBasicProperties basicProperties)
    {
        if (basicProperties.Headers != null &&
            basicProperties.Headers.TryGetValue("x-retry-count", out var retryHeader))
        {
            return Convert.ToInt32(retryHeader);
        }

        return 0;
    }

    private void RetryMessage(BasicGetResult result, int currentRetryCount)
    {
        var properties = result.BasicProperties;
        properties.Headers ??= new Dictionary<string, object>();
        properties.Headers["x-retry-count"] = currentRetryCount + 1;

        _channel.BasicPublish(exchange: "", routingKey: result.RoutingKey, basicProperties: properties,
            body: result.Body);
        _channel.BasicAck(result.DeliveryTag, false); // Ack message ban đầu để không giữ lại trong queue
    }

    private void MoveToDeadLetterQueue(BasicGetResult result, string queueName)
    {
        _channel.BasicPublish(exchange: "", routingKey: _deadLetterQueue, basicProperties: result.BasicProperties,
            body: result.Body);
        _channel.BasicAck(result.DeliveryTag, false); // Ack message để không giữ lại trong queue chính
    }


    public void Dispose()
    {
        _channel?.Close();
        _connection?.Connection.Close();
    }
}