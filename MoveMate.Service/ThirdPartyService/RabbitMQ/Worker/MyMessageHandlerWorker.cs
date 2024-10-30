using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ;

public class MyMessageHandlerWorker
{
    //private UnitOfWork _unitOfWork;

    /*public MyMessageHandlerWorker(IUnitOfWork unitOfWork)
    {
        _unitOfWork = (UnitOfWork) unitOfWork;
    }*/

    [Consumer("chanel-2")]
    public async Task HandleMessage(String message)
    {
        // Xử lý thông điệp ở đây
        //await Task.Delay(100);
        //var check = _unitOfWork.ServiceRepository.GetByIdAsync(1).Result;, check: {check.Id}
        Console.WriteLine($"Received message: {message}");
    }
}