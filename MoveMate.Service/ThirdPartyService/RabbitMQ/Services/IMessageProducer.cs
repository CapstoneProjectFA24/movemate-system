namespace MoveMate.Service.ThirdPartyService.RabbitMQ;

public interface IMessageProducer
{
    public void SendingMessage<T>(T message);
    public void SendingMessage<T>(string channel, T message);
}