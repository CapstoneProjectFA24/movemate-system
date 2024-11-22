using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.IRepository;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Utils;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class ScheduleWorkingServices : IScheduleWorkingServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<ScheduleWorkingServices> _logger;

        public ScheduleWorkingServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ScheduleWorkingServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<List<ScheduleWorkingResponse>>> GetAll(GetAllScheduleWorking request)
        {
            var result = new OperationResult<List<ScheduleWorkingResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.ScheduleWorkingRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder()
                );
                var listResponse = _mapper.Map<List<ScheduleWorkingResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok,
                        MessageConstant.SuccessMessage.GetListScheduleWorkingEmpty, listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListScheduleWorkingSuccess,
                    listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<ScheduleWorkingResponse>> GetScheduleWorkingById(int id)
        {
            var result = new OperationResult<ScheduleWorkingResponse>();
            try
            {
                var entity =
                    await _unitOfWork.ScheduleWorkingRepository.GetByIdAsync(id);

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundScheduleWorking);
                }
                else
                {
                    var productResponse = _mapper.Map<ScheduleWorkingResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetScheduleWorkingSuccess,
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

        public async Task<OperationResult<ScheduleWorkingResponse>> UpdateScheduleWorking(int id, UpdateScheduleWorkingRequest request)
        {
            var result = new OperationResult<ScheduleWorkingResponse>();

            try
            {

                var schedule = await _unitOfWork.ScheduleWorkingRepository.GetByIdAsync(id);
                if (schedule == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruckCategory);
                    return result;
                }

                if (!string.IsNullOrEmpty(request.StartDate) && !string.IsNullOrEmpty(request.EndDate))
                {
                    if (!TimeOnly.TryParse(request.StartDate, out var startDate))
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TimeOnlyFormat);
                        return result;
                    }

                    if (!TimeOnly.TryParse(request.EndDate, out var endDate))
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TimeOnlyFormat);
                        return result;
                    }

                    // Validate StartDate < EndDate
                    if (startDate >= endDate)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InvalidDates);
                        return result;
                    }
                }

                if (!string.IsNullOrEmpty(request.StartDate) && string.IsNullOrEmpty(request.EndDate))
                {
                    if (!TimeOnly.TryParse(request.EndDate, out var endDate))
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TimeOnlyFormat);
                        return result;
                    }
                    if (schedule.StartDate >= endDate)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InvalidDates);
                        return result;
                    }
                }
                if (string.IsNullOrEmpty(request.StartDate) && !string.IsNullOrEmpty(request.EndDate))
                {
                    if (!TimeOnly.TryParse(request.StartDate, out var startDate))
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TimeOnlyFormat);
                        return result;
                    }
                    if (startDate >= schedule.EndDate)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InvalidDates);
                        return result;
                    }
                }

                ReflectionUtils.UpdateProperties(request, schedule);
                if (schedule.StartDate.HasValue && schedule.EndDate.HasValue)
                {
                    schedule.DurationTimeActived = (int)(schedule.EndDate.Value - schedule.StartDate.Value).TotalHours;
                }
                await _unitOfWork.ScheduleWorkingRepository.SaveOrUpdateAsync(schedule);
                var saveResult = _unitOfWork.Save();

                // Check save result and return response
                if (saveResult > 0)
                {
                    schedule = await _unitOfWork.ScheduleWorkingRepository.GetByIdAsync(schedule.Id);
                    var response = _mapper.Map<ScheduleWorkingResponse>(schedule);

                    result.AddResponseStatusCode(StatusCode.Ok,
                        MessageConstant.SuccessMessage.ScheduleWorkingUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ScheduleWorkingUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<ScheduleWorkingResponse>> CreateScheduleWorking(CreateScheduleWorkingRequest request)
        {
            var result = new OperationResult<ScheduleWorkingResponse>();
         
            try
            {
                // Validate that StartDate is less than EndDate
                if (!TimeOnly.TryParse(request.StartDate, out var startDate))
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TimeOnlyFormat);
                    return result;
                }

                if (!TimeOnly.TryParse(request.EndDate, out var endDate))
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TimeOnlyFormat);
                    return result;
                }

                // Validate StartDate < EndDate
                if (startDate >= endDate)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InvalidDates);
                    return result;
                }


                // Map request to ScheduleWorking entity
                var schedule = _mapper.Map<ScheduleWorking>(request);

                // Calculate DurationTimeActived in minutes, hours, or days as appropriate
                schedule.DurationTimeActived = (int)(endDate - startDate).TotalHours;

                // Save to the database
                await _unitOfWork.ScheduleWorkingRepository.AddAsync(schedule);
                await _unitOfWork.SaveChangesAsync();

                // Map to response object and return success
                var response = _mapper.Map<ScheduleWorkingResponse>(schedule);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateScheduleWorking, response);

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }


        public async Task<OperationResult<bool>> DeleteScheduleWorking(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var schedule = await _unitOfWork.ScheduleWorkingRepository.GetByIdAsync(id);
                if (schedule == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundScheduleWorking);
                    return result;
                }

                if (schedule.IsActived == false)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ScheduleWorkingAlreadyDeleted);
                    return result;
                }

                schedule.IsActived = false;

                await _unitOfWork.ScheduleWorkingRepository.SaveOrUpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteScheduleWorking, true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<ScheduleWorkingResponse>> AddGroupIntoSchedule(AddScheduleIntoGroup request)
        {
            var result = new OperationResult<ScheduleWorkingResponse>();
            try
            {
                var schedule = await _unitOfWork.ScheduleWorkingRepository.GetByIdAsync(request.ScheduleId);
                if (schedule == null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundScheduleWorking);
                    return result;
                }
                var group = await _unitOfWork.GroupRepository.GetByIdAsync(request.GroupId);
                if (group == null)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotFoundGroup);
                    return result;
                }

                schedule.GroupId = request.GroupId;
                await _unitOfWork.ScheduleWorkingRepository.SaveOrUpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync();

                // Fetch the updated group and map response
                schedule = await _unitOfWork.ScheduleWorkingRepository.GetByIdAsync(request.ScheduleId);
                var scheduleResponse = _mapper.Map<ScheduleWorkingResponse>(schedule);

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.AddScheduleToGroup, scheduleResponse);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddUserIntoGroup method");
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }
    }
}
