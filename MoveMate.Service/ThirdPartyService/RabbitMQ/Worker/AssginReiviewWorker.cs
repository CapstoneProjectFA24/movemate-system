using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ;

public class AssginReiviewWorker
{
    [Consumer("movemate.booking_assign_review")]
    public void HandleMessage(int message)
    {
        // Xử lý thông điệp ở đây
        Console.WriteLine($"Received message: {message}");
    }
}