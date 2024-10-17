using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ;

public class MyMessageHandlerWorker
{
    [Consumer("chanel-1")]
    public void HandleMessage(object message)
    {
        // Xử lý thông điệp ở đây
        Console.WriteLine($"Received message: {message}");
    }
}