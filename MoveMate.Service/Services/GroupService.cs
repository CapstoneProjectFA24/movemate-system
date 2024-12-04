using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Utils;
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
            foreach (var group in listResponse)
            {
                group.CountUser = group.Users?.Count ?? 0;
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
                productResponse.CountUser = productResponse.Users.Count();
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


    public async Task<OperationResult<GroupResponse>> UpdateGroup(int id, UpdateGroupRequest request)
    {
        var result = new OperationResult<GroupResponse>();
        try
        {
            var group = await _unitOfWork.GroupRepository.GetByIdAsyncV1(id, includeProperties: "ScheduleWorkings,Users");
            if (group == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundGroup);
                return result;
            }

            ReflectionUtils.UpdateProperties(request, group);
            await _unitOfWork.GroupRepository.SaveOrUpdateAsync(group);
            var saveResult = _unitOfWork.Save();

            // Check save result and return response
            if (saveResult > 0)
            {
                group = await _unitOfWork.GroupRepository.GetByIdAsyncV1(group.Id, includeProperties: "ScheduleWorkings,Users");
                var response = _mapper.Map<GroupResponse>(group);
                response.CountUser = response.Users.Count();
                result.AddResponseStatusCode(StatusCode.Ok,
                    MessageConstant.SuccessMessage.TruckCategoryUpdateSuccess,
                    response);
            }
            else
            {
                result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TruckCategoryUpdateFail);
            }
        }
        catch (Exception ex)
        {
            result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
        }

        return result;
    }

    public async Task<OperationResult<GroupResponse>> CreateGroup(CreateGroupRequest request)
    {
        var result = new OperationResult<GroupResponse>();
       
        try
        {
         
            var group = _mapper.Map<Group>(request);

            await _unitOfWork.GroupRepository.AddAsync(group);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<GroupResponse>(group);
            response.CountUser = response.Users.Count();
            result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateGroup,
                response);

            return result;
        }
        catch (Exception ex)
        {
            result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
        }

        return result;
    }

    public async Task<OperationResult<GroupResponse>> AddUserIntoGroup(AddUserIntoGroup request)
    {
        var result = new OperationResult<GroupResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                return result;
            }

            if(user.RoleId == 1 || user.RoleId == 3 ||  user.RoleId == 6)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.UserCannotAdd);
                return result;
            }
            var group = await _unitOfWork.GroupRepository.GetByIdAsync(request.GroupId);
            if (group == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundGroup);
                return result;
            }

            if (user.IsDriver == true && user.RoleId == 4)
            {
                var truck = await _unitOfWork.TruckRepository.FindByUserIdAsync(user.Id);
                if (truck == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruck);
                    return result;
                }
                switch (truck.TruckCategoryId)
                {
                    case 1:
                        var truckCategory1Count = await _unitOfWork.UserRepository.GetUsersByTruckCategoryIdAsync(1, request.GroupId);
                        if (truckCategory1Count.Count() >= 4)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotAddStaff);
                            return result;
                        }
                        break;
                    case 2:
                        var truckCategory2Count = await _unitOfWork.UserRepository.GetUsersByTruckCategoryIdAsync(2, request.GroupId);
                        if (truckCategory2Count.Count() >= 4)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotAddStaff);
                            return result;
                        }
                        break;
                    case 3:
                        var truckCategory3Count = await _unitOfWork.UserRepository.GetUsersByTruckCategoryIdAsync(3, request.GroupId);
                        if (truckCategory3Count.Count() >= 4)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotAddStaff);
                            return result;
                        }
                        break;
                    case 4:
                        var truckCategory4Count = await _unitOfWork.UserRepository.GetUsersByTruckCategoryIdAsync(4, request.GroupId);
                        if (truckCategory4Count.Count() >= 4)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotAddStaff);
                            return result;
                        }
                        break;
                    case 5:
                        var truckCategory5Count = await _unitOfWork.UserRepository.GetUsersByTruckCategoryIdAsync(5, request.GroupId);
                        if (truckCategory5Count.Count() >= 8)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotAddStaff);
                            return result;
                        }
                        break;
                    case 6:
                        var truckCategory6Count = await _unitOfWork.UserRepository.GetUsersByTruckCategoryIdAsync(6, request.GroupId);
                        if (truckCategory6Count.Count() >= 2)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotAddStaff);
                            return result;
                        }
                        break;
                    default:
                        
                        break;
                }
            }

            switch (user.RoleId)
            {
                case 2: // RoleId 2 check
                    var role2Count = await _unitOfWork.UserRepository.FindAllUserByRoleIdAsync(2, request.GroupId);
                    if (role2Count.Count() >= 3)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotAddReviewer);
                        return result;
                    }
                    break;
                case 5: // RoleId 5 check
                    var role5Count = await _unitOfWork.UserRepository.FindAllUserByRoleIdAsync(5, request.GroupId);
                    if (role5Count.Count() >= 40)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotAddStaff);
                        return result;
                    }
                    break;

                default:
                    // For other RoleIds, no special restrictions are applied
                    break;
            }

            // Assign the group to the user
            user.GroupId = request.GroupId;
            await _unitOfWork.UserRepository.SaveOrUpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Fetch the updated group and map response
            group = await _unitOfWork.GroupRepository.GetByIdAsyncV1(request.GroupId, includeProperties: "Users");
            var groupResponse = _mapper.Map<GroupResponse>(group);

            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.AddUserToGroup, groupResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in AddUserIntoGroup method");
            result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
        }

        return result;
    }

    public async Task<OperationResult<GroupUserResponse>> GetUserIntoGroup(int groupId)
    {
        var result = new OperationResult<GroupUserResponse>();
        var response = new GroupUserResponse();
        try
        {
            var group = await _unitOfWork.GroupRepository.GetByIdAsyncV1(groupId, includeProperties: "Users");
            if (group == null)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundGroup);
                return result;
            }

            var reviewers = await _unitOfWork.UserRepository.GetUsersByGroupIdAsync(groupId, 2);
            response.Reviewers = reviewers.Count();
            response.ReviewersNeed = 3- reviewers.Count(); 

            for (int truckCategoryId = 1; truckCategoryId <= 6; truckCategoryId++)
            {
                var drivers = await _unitOfWork.UserRepository.GetUsersByTruckCategoryIdAsync(truckCategoryId, groupId);
                switch (truckCategoryId)
                {
                    case 1:
                        response.DriversTruck1 = drivers.Count();
                        response.DriversTruck1Need = 4 - drivers.Count() ; 
                        break;
                    case 2:
                        response.DriversTruck2 = drivers.Count();
                        response.DriversTruck2Need = 4 - drivers.Count();
                        break;
                    case 3:
                        response.DriversTruck3 = drivers.Count();
                        response.DriversTruck3Need = 4 - drivers.Count();
                        break;
                    case 4:
                        response.DriversTruck4 = drivers.Count();
                        response.DriversTruck4Need = 4 - drivers.Count();
                        break;
                    case 5:
                        response.DriversTruck5 = drivers.Count();
                        response.DriversTruck5Need = 8 - drivers.Count();
                        break;
                    case 6:
                        response.DriversTruck6 = drivers.Count();
                        response.DriversTruck6Need = 2 - drivers.Count();
                        break;
                }
            }

            var porters = await _unitOfWork.UserRepository.GetUsersByGroupIdAsync(groupId, 5);
            response.Porters = porters.Count();
            response.PortersNeed = 40 - porters.Count(); 

            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetGroupSuccess, response);
            return result;

        }
        catch(Exception ex)
        {
            result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            return result;
        }
    }
}