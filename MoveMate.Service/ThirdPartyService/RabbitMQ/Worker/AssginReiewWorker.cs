using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker;

public class AssginReiewWorker
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AssginReiewWorker> _logger;

    public AssginReiewWorker(IServiceScopeFactory serviceScopeFactory, ILogger<AssginReiewWorker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    [Consumer("movemate.booking_assign_review")]
    public async Task HandleMessage(int message)
    {
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork =(UnitOfWork) scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                var booking = await unitOfWork.BookingRepository.GetByIdAsync(message);
                Console.WriteLine($"Booking info: {booking}");

            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing booking review for message {Message}", message);
            throw;
        }
        Console.WriteLine($"Received message: {message}");
    }
}