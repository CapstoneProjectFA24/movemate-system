using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
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

        public async Task<OperationResult<List<ServicesResponse>>> GetAll(GetAllServiceRequest request)
        {
            var result = new OperationResult<List<ServicesResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.ServiceRepository.GetAllWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder()
                );
                var listResponse = _mapper.Map<List<ServicesResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListServiceEmpty, listResponse);
                    return result;
                }

                pagin.PageSize = request.per_page;
                pagin.TotalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListServiceSuccess, listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<List<ServicesResponse>>> GetAllNotTruck(GetAllServiceNotTruckRequest request)
        {
            var result = new OperationResult<List<ServicesResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.ServiceRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder()
                );
                var listResponse = _mapper.Map<List<ServicesResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListServiceEmpty, listResponse);
                    return result;
                }

                pagin.PageSize = request.per_page;
                pagin.TotalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListServiceSuccess, listResponse, pagin);

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
                var entity =
                    await _unitOfWork.ServiceRepository.GetByIdAsyncV1(id, includeProperties: "InverseParentService");

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundService);
                }
                else
                {
                    var productResponse = _mapper.Map<ServicesResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetServiceSuccess, productResponse);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in Get Service By Id service method for ID: {id}");
                throw;
            }
        }

        public async Task<OperationResult<List<ServiceResponse>>> GetAllServiceTruck(GetAllServiceTruckType request)
        {
            var result = new OperationResult<List<ServiceResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.ServiceRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder(),
                    includeProperties: "TruckCategory"
                );
                var listResponse = _mapper.Map<List<ServiceResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListServiceEmpty, listResponse);
                    return result;
                }

                pagin.PageSize = request.per_page;
                pagin.TotalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListServiceSuccess, listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<ServicesResponse>> CreateService(CreateServiceRequest request)
        {
            var result = new OperationResult<ServicesResponse>();

            try
            {
                // Check if TruckCategoryId exists if provided
                if (request.TruckCategoryId.HasValue)
                {
                    var truckCategoryExists = await _unitOfWork.TruckCategoryRepository
                        .GetByIdAsync(request.TruckCategoryId.Value);

                    if (truckCategoryExists == null)
                    {
                        result.AddError(StatusCode.BadRequest, "The specified TruckCategoryId does not exist.");
                        return result;
                    }
                }

                // Map request to Service domain model
                var service = _mapper.Map<MoveMate.Domain.Models.Service>(request);

                // Set Tier based on InverseParentService count
                service.Tier = request.InverseParentService.Count < 1 ? 1 : 0;

                // If Tier is 1, validate that ParentServiceId refers to a Tier 0 service and that Types match
                if (service.Tier == 1)
                {
                    if (!request.ParentServiceId.HasValue)
                    {
                        result.AddError(StatusCode.BadRequest, "A valid ParentServiceId is required for Tier 1 services.");
                        return result;
                    }

                    var parentService = await _unitOfWork.ServiceRepository
                        .GetTierZeroServiceByParentIdAsync(request.ParentServiceId.Value);

                    if (parentService == null)
                    {
                        result.AddError(StatusCode.BadRequest, "The specified ParentServiceId must refer to a Tier 0 service.");
                        return result;
                    }

                    // Validate that the Type of the request matches the Type of the ParentService
                    if (parentService.Type != request.Type)
                    {
                        result.AddError(StatusCode.BadRequest, "The Type of the service must match the Type of its ParentService.");
                        return result;
                    }

                    // Set ParentServiceId if valid
                    service.ParentServiceId = request.ParentServiceId;
                }

                // Validate that each item in InverseParentService has the same Type as the main service
                foreach (var inverseService in request.InverseParentService)
                {
                    if (inverseService.Type != request.Type)
                    {
                        result.AddError(StatusCode.BadRequest, "Each inverseParentService item must have the same Type as the main service.");
                        return result;
                    }
                }

                // Add and save the new service
                await _unitOfWork.ServiceRepository.AddAsync(service);
                await _unitOfWork.SaveChangesAsync();

                // Map to response and add success status
                var response = _mapper.Map<ServicesResponse>(service);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.CreateService, response);

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }




        public async Task<OperationResult<bool>> DeleteService(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var service = await _unitOfWork.ServiceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                }

                service.IsActived = false;

                await _unitOfWork.SaveChangesAsync();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteService, true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }
    }
}