using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.GoongMap;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class ScheduleService : IScheduleService
    {

        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<ScheduleService> _logger;
        

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ScheduleService> logger)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _mapper = mapper;
            _logger = logger;          
        }

        public async Task<OperationResult<List<ScheduleDailyResponse>>> GetAll(GetAllScheduleDailyRequest request)
        {
            var result = new OperationResult<List<ScheduleDailyResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.ScheduleRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder(),
                    includeProperties: "ScheduleWorkings"
                );
                var listResponse = _mapper.Map<List<ScheduleDailyResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok,
                        MessageConstant.SuccessMessage.GetListScheduleDailyEmpty, listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListScheduleDailySuccess,
                    listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<ScheduleDailyResponse>> CreateSchedule(ScheduleRequest request)
        {
            var result = new OperationResult<ScheduleDailyResponse>();
            try
            {
                var scheduleWorking = await _unitOfWork.ScheduleWorkingRepository.GetByIdAsync(request.ScheduleWorkingId);
                if (scheduleWorking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundScheduleWorking);
                    return result;
                }

                var group = await _unitOfWork.GroupRepository.GetByIdAsync(request.GroupId);
                if (group == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundGroup);
                    return result;
                }

                var schedule = _unitOfWork.ScheduleRepository.GetByDate(request.Date);
                if (schedule.IsDeleted == true)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ScheduleIsDeleted);
                    return result;
                }
                if (schedule == null)
                {
                    schedule = new Schedule
                    {
                        Date = DateOnly.ParseExact(request.Date, "MM/dd/yyyy"), 
                        IsDeleted = false
                    };

                    // Thêm Schedule vào cơ sở dữ liệu
                    await _unitOfWork.ScheduleRepository.AddAsync(schedule);

                    scheduleWorking.ScheduleId = schedule.Id;
                    scheduleWorking.GroupId = group.Id;
                }
                else
                {
                    scheduleWorking.ScheduleId = schedule.Id;
                    scheduleWorking.GroupId = group.Id;
                }
                await _unitOfWork.ScheduleWorkingRepository.SaveOrUpdateAsync(scheduleWorking);
                await _unitOfWork.GroupRepository.SaveOrUpdateAsync(group);
                await _unitOfWork.ScheduleRepository.SaveOrUpdateAsync(schedule);
                await _unitOfWork.SaveChangesAsync();
                schedule = await _unitOfWork.ScheduleRepository.GetByIdAsync(schedule.Id, includeProperties: "ScheduleWorkings");
                var response = _mapper.Map<ScheduleDailyResponse>(schedule);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateSchedule,
                    response);

                return result;

            }
            catch (Exception e)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }
    }
}
