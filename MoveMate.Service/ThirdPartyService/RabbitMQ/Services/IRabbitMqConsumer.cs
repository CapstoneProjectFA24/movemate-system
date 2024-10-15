namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Config;

public interface IRabbitMqConsumer
{
    public void StartConsuming<T>() where T : class;
}