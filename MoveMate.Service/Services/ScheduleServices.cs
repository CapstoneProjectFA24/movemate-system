using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class ScheduleServices : IScheduleServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<ScheduleServices> _logger;

        public ScheduleServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ScheduleServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<List<ScheduleResponse>>> GetAll(GetAllSchedule request)
        {
            var result = new OperationResult<List<ScheduleResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.ScheduleRepository.Get(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder(),
                    includeProperties: "ScheduleDetails"
                );
                var listResponse = _mapper.Map<List<ScheduleResponse>>(entities);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, "List Schedule is Empty!", listResponse);
                    return result;
                }

                pagin.PageSize = request.per_page;
                pagin.TotalItemsCount = listResponse.Count();

                result.AddResponseStatusCode(StatusCode.Ok, "Get List Schedule Done", listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<ScheduleResponse>> GetById(int id)
        {
            var result = new OperationResult<ScheduleResponse>();
            try
            {
                var entity =
                    await _unitOfWork.ScheduleRepository.GetByIdAsyncV1(id, includeProperties: "ScheduleDetails");

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, "Schedule not found");
                }
                else if ((bool)entity.IsActived)
                {
                    var productResponse = _mapper.Map<ScheduleResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, "Get Schedule by Id Success!", productResponse);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Get Auction By Id service method for ID: {id}");
                throw;
            }
        }
    }
}