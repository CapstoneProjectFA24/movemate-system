﻿using AutoMapper;
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
    public class ServiceServices : IServiceServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<ServiceServices> _logger;
        public ServiceServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ServiceServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<List<ServiceResponse>>> GetAll(GetAllServiceRequest request)
        {
            var result = new OperationResult<List<ServiceResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.ServiceRepository.Get(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder()

                );
                var listResponse = _mapper.Map<List<ServiceResponse>>(entities);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, "List Service is Empty!", listResponse);
                    return result;
                }

                pagin.PageSize = request.per_page;
                pagin.TotalItemsCount = listResponse.Count();

                result.AddResponseStatusCode(StatusCode.Ok, "Get List Services Done.", listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<ServicesResponse>> GetById(int id)
        {
            var result = new OperationResult<ServicesResponse>();
            try
            {
                var entity = await _unitOfWork.ServiceRepository.GetByIdAsyncV1(id, includeProperties: "InverseParentService");

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, $"Can't found Service with Id: {id}");
                }
                else
                {
                    var productResponse = _mapper.Map<ServicesResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, $"Get Service by Id: {id} Success!", productResponse);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Get Service By Id service method for ID: {id}");
                throw;
            }
        }

    }
}