using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System.ComponentModel.DataAnnotations;

namespace MoveMate.Service.Services;

public class GroupService : IGroupServices
{
    private UnitOfWork _unitOfWork;
    private IMapper _mapper;
    private readonly ILogger<GroupService> _logger;

    public GroupService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GroupService> logger)
    {
        _unitOfWork = (UnitOfWork)unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task AddDriverDailyByShard(string shard)
    {
        _logger.LogInformation($"Adding daily for {shard}");
        _unitOfWork.UserRepository.FindAllUserByRoleIdAsync(4);
    }

    public async Task<OperationResult<List<GroupResponse>>> GetAll(GetAllGroupRequest request)
    {
        var result = new OperationResult<List<GroupResponse>>();

        var pagin = new Pagination();

        var filter = request.GetExpressions();

        try
        {
            var entities = _unitOfWork.GroupRepository.GetWithCount(
                filter: request.GetExpressions(),
                pageIndex: request.page,
                pageSize: request.per_page,
                orderBy: request.GetOrder(),
                includeProperties: "ScheduleWorkings,Users"
            );
            var listResponse = _mapper.Map<List<GroupResponse>>(entities.Data);

            if (listResponse == null || !listResponse.Any())
            {
                result.AddResponseStatusCode(StatusCode.Ok,
                    MessageConstant.SuccessMessage.GetListTruckCategoryEmpty, listResponse);
                return result;
            }

            pagin.pageSize = request.per_page;
            pagin.totalItemsCount = entities.Count;

            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListHouseTypeSuccess,
                listResponse, pagin);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred in getAll Service Method");
            throw;
        }
    }

    public async Task<OperationResult<bool>> DeleteGroup(int id)
    {
        var result = new OperationResult<bool>();
        try
        {
            var group = await _unitOfWork.GroupRepository.GetByIdAsync(id);
            if (group == null)
            {
                result.AddResponseErrorStatusCode(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundGroup, false);
                return result;
            }

            group.IsActived = false;

            await _unitOfWork.GroupRepository.SaveOrUpdateAsync(group);
            await _unitOfWork.SaveChangesAsync();
            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteGroup, true);
        }
        catch
        {
            result.AddResponseErrorStatusCode(StatusCode.ServerError, MessageConstant.FailMessage.ServerError, false);
        }

        return result;
    }

    public async Task<OperationResult<GroupResponse>> GetGroupById(int id)
    {
        var result = new OperationResult<GroupResponse>();
        try
        {
            var entity =
                await _unitOfWork.GroupRepository.GetByIdAsyncV1(id, includeProperties: "ScheduleWorkings,Users");

            if (entity == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundGroup);
            }
            else
            {
                var productResponse = _mapper.Map<GroupResponse>(entity);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetGroupSuccess,
                    productResponse);
            }

            return result;
        }
        catch (Exception ex)
        {
            result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            return result;
        }
    }

    public async Task<OperationResult<TruckResponse>> CreateTruck(CreateTruckRequest request)
    {
        var result = new OperationResult<TruckResponse>();
        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

        if (!isValid)
        {
            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ValidateField);
            return result;
        }
        try
        {
            var truckCategory =
                await _unitOfWork.TruckCategoryRepository.GetByIdAsync((int)request.TruckCategoryId);
            if (truckCategory == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruckCategory);
                return result;
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync((int)request.UserId);
            if (user == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUserInfo);
                return result;
            }

            if (user.IsDriver == false)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.UserNotDriver);
                return result;
            }

            var truckExist = await _unitOfWork.TruckRepository.FindByUserIdAsync((int)request.UserId);
            if (truckExist != null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.UserHaveTruck);
                return result;
            }

            List<TruckImg> truckImgList = _mapper.Map<List<TruckImg>>(request.TruckImgs);

            var truck = _mapper.Map<Truck>(request);

            await _unitOfWork.TruckRepository.AddAsync(truck);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<TruckResponse>(truck);
            result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateTruckImg,
                response);

            return result;
        }
        catch (Exception ex)
        {
            result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
        }

        return result;
    }
}