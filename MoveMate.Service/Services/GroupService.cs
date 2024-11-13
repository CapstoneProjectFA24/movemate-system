using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;

namespace MoveMate.Service.Services;

public class GroupService : IGroupServices
{
    private UnitOfWork _unitOfWork;
    private IMapper _mapper;
    private readonly ILogger<GroupService> _logger;

    public GroupService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GroupService> logger)
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

    public Task<OperationResult<List<GroupResponse>>> GetAll(GetAllStaffDailyRequest request)
    {
        throw new NotImplementedException();
    }
}