using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ;

public class MyMessageHandlerWorker
{
    [Consumer("chanel-1")]
    public async Task HandleMessage(object message)
    {
        // Xử lý thông điệp ở đây
        await Task.Delay(1000);
        Console.WriteLine($"Received message: {message}");
    }
}