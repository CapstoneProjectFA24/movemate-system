using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;

namespace MoveMate.Service.Services;

public class BookingStaffDailyService
{
    private UnitOfWork _unitOfWork;
    private IMapper _mapper;
    private readonly ILogger<BookingStaffDailyService> _logger;

    public BookingStaffDailyService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<BookingStaffDailyService> logger)
    {
        _unitOfWork = (UnitOfWork) unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task AddDriverDailyByShard(string shard)
    {
        _logger.LogInformation($"Adding daily for {shard}");
        _unitOfWork.UserRepository.FindAllUserByRoleIdAsync(4);
    }
}