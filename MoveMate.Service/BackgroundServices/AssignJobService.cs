using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.ThirdPartyService.Redis;

namespace MoveMate.Service.BackgroundServices;

public class AssignJobService : IAssignJobService
{
    private UnitOfWork? _unitOfWork;
    private readonly ILogger<BackgroundServiceHangFire> _logger;
    private readonly IRedisService _redisService;

    public AssignJobService(IUnitOfWork unitOfWork, ILogger<BackgroundServiceHangFire> logger,
        IRedisService redisService)
    {
        _unitOfWork = (UnitOfWork?)unitOfWork;
        _logger = logger;
        _redisService = redisService;
    }

    public async void AddReviewerJob()
    {
        List<int> listReviewer = await _unitOfWork.UserRepository.FindAllUserByRoleIdAsync(2);
        await _redisService.EnqueueMultipleAsync("", listReviewer);
    }
}