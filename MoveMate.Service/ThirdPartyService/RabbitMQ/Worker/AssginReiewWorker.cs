using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker;

public class AssginReiewWorker
{
    [Consumer("movemate.booking_assign_review")]
    public async void HandleMessage(int message)
    {
        //var booking = await _unitOfWork.BookingRepository.GetByIdAsync(message);


        // Xử lý thông điệp ở đây
        Console.WriteLine($"Received message: {message}");
    }
}