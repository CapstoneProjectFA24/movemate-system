
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.RabbitMQ.Annotation;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.Worker;

public class SetScheduleReview
{
    private readonly ILogger<SetScheduleReview> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SetScheduleReview(ILogger<SetScheduleReview> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    [Consumer("movemate.setup_schedule_review")]
    public async Task HandleMessage(int message)
    {
        Console.WriteLine("Message received - movemate.setup_schedule_review");
        await Task.Delay(100);
        try
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var unitOfWork = (UnitOfWork)scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                
                var existingBooking = await unitOfWork.BookingRepository.GetByIdAsync(message, "Assignments");
                
                var assignmentStatus = existingBooking!.Assignments.FirstOrDefault(a =>
                    a.Status == AssignmentStatusEnums.ASSIGNED.ToString()&&
                    a.StaffType == RoleEnums.REVIEWER.ToString());
                
                /*var newScheduleReview = new ScheduleBookingDetail()
                {
                    BookingId = message,
                    Type = RoleEnums.REVIEWER + " OFFLINE",
                    StartDate = existingBooking.ReviewAt,
                    UserId = assignmentStatus!.UserId,
                };*/
                //existingBooking.ScheduleBookingDetails.Add(newScheduleReview);
                existingBooking.UpdatedAt = DateTime.Now;
                unitOfWork.BookingRepository.Update(existingBooking);
                await unitOfWork.SaveChangesAsync();
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


}