namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class ConsumerAttribute : Attribute
{
    public string QueueName { get; }

    public ConsumerAttribute(string queueName)
    {
        QueueName = queueName;
    }
}