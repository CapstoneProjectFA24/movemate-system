using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.Utils;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class ScheduleBookingServices : IScheduleBooingServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<ScheduleBookingServices> _logger;

        public ScheduleBookingServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ScheduleBookingServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;        
        }

        public async Task<OperationResult<List<ScheduleBookingResponse>>> GetAllScheduleBooking(GetAllScheduleBookingRequest request)
        {
            var result = new OperationResult<List<ScheduleBookingResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.ScheduleBookingRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder(),
                    includeProperties: "ScheduleBookingDetails"
                );
                var listResponse = _mapper.Map<List<ScheduleBookingResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListScheduleBookingEmpty,
                        listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListScheduleBookingSuccess,
                    listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<ScheduleBookingResponse>> GetScheduleBookingById(int id)
        {
            var result = new OperationResult<ScheduleBookingResponse>();
            try
            {
                var entity =
                    await _unitOfWork.ScheduleBookingRepository.GetByIdAsync(id, includeProperties: "ScheduleBookingDetails");

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundScheduleBooking);
                }
                else
                {
                    var productResponse = _mapper.Map<ScheduleBookingResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetScheduleBookingSuccess,
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

        public async Task<OperationResult<ScheduleBookingResponse>> UpdateScheduleBooking(int id, UpdateScheduleBookingRequest request)
        {
            var result = new OperationResult<ScheduleBookingResponse>();
            try
            {
                var schedule = await _unitOfWork.ScheduleBookingRepository.GetByIdAsync(id);
                if (schedule == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundScheduleBooking);
                    return result;
                }

                ReflectionUtils.UpdateProperties(request, schedule);
                await _unitOfWork.ScheduleBookingRepository.SaveOrUpdateAsync(schedule);
                var saveResult = _unitOfWork.Save();

                // Check save result and return response
                if (saveResult > 0)
                {
                    schedule = await _unitOfWork.ScheduleBookingRepository.GetByIdAsync(schedule.Id);
                    var response = _mapper.Map<ScheduleBookingResponse>(schedule);

                    result.AddResponseStatusCode(StatusCode.Ok,
                        MessageConstant.SuccessMessage.ScheduleBookingUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ScheduleBookingUpdateFail);
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<ScheduleBookingResponse>> CreateScheduleBooking(CreateScheduleBookingRequest request)
        {

            var result = new OperationResult<ScheduleBookingResponse>();
            try
            {
                var schedule = _mapper.Map<ScheduleBooking>(request);

                await _unitOfWork.ScheduleBookingRepository.AddAsync(schedule);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<ScheduleBookingResponse>(schedule);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateScheduleBookingCategory,
                    response);

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<bool>> DeleteScheduleBooking(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var schedule = await _unitOfWork.ScheduleBookingRepository.GetByIdAsync(id);
                if (schedule == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundScheduleBooking);
                    return result;
                }

                if (schedule.IsActived == false)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ScheduleBookingAlreadyDeleted);
                    return result;
                }

                schedule.IsActived = false;

                _unitOfWork.ScheduleBookingRepository.Update(schedule);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteScheduleBooking, true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }
    }
}
