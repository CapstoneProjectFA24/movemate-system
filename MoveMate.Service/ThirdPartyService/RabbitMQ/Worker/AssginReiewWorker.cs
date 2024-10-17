using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker;

public class AssginReiewWorker
{
    private UnitOfWork _unitOfWork;
    private IMapper _mapper;
    private readonly ILogger<AssginReiewWorker> _logger;
    
    public AssginReiewWorker(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AssginReiewWorker> logger)
    {
        _unitOfWork = (UnitOfWork)unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    [Consumer("movemate.booking_assign_review")]
    public async void HandleMessage(int message)
    {
        var booking = await _unitOfWork.BookingRepository.GetByIdAsync(message);


        // Xử lý thông điệp ở đây
        Console.WriteLine($"Received message: {message}");
    }
}