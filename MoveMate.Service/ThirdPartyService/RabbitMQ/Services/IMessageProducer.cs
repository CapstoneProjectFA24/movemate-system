namespace MoveMate.Service.ThirdPartyService.RabbitMQ;

public interface IMessageProducer
{
    public void SendingMessage<T>(T message);
}