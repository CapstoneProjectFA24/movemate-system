using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker;

public class AssginReiewWorker
{
    private readonly ILogger<AssginReiewWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    public AssginReiewWorker(ILogger<AssginReiewWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    [Consumer("movemate.booking_assign_review")]
    public async Task  HandleMessage(int message)
    {
        try
        {
            // Tạo một scope mới để quản lý lifecycle của các dependency
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = (UnitOfWork) scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                // Thực hiện xử lý booking
                var booking = await unitOfWork.BookingRepository.GetByIdAsync(message);
                Console.WriteLine($"Booking info: {booking}");

                // Xử lý dữ liệu tại đây, sử dụng mapper nếu cần
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