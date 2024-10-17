using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.Utils;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker;

public class AssginReiewWorker
{
    private readonly ILogger<AssginReiewWorker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public AssginReiewWorker(ILogger<AssginReiewWorker> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    [Consumer("movemate.booking_assign_review_local")]
    public async Task  HandleMessage(int message)
    {
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = (UnitOfWork) scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                
                var booking = await unitOfWork.BookingRepository.GetByIdAsync(message);

                string redisKey = DateUtil.GetKeyReview();
                var reviewerId = await redisService.DequeueAsync<int>(redisKey);

                var reviewer = new BookingDetail()
                {
                    BookingId = message,
                    Status = BookingDetailStatus.ASSIGNED.ToString(),
                    UserId = reviewerId,
                    StaffType = RoleEnums.REVIEWER.ToString(),
                };
                
                booking.BookingDetails.Add(reviewer);
                
                booking.Status = BookingDetailStatus.ASSIGNED.ToString();
                unitOfWork.BookingRepository.Update(booking);
                unitOfWork.Save();
                
                redisService.EnqueueAsync(redisKey, reviewerId);
                
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