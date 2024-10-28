using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
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
                    orderBy: request.GetOrder(),
                    includeProperties: "InverseParentService"

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
                if (request.Type == TypeServiceEnums.TRUCK.ToString() && !request.TruckCategoryId.HasValue)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TruckCategoryRequire);
                    return result;
                }
                if (request.TruckCategoryId.HasValue && request.Type != TypeServiceEnums.TRUCK.ToString())
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TypeTruckRequire);
                    return result;
                }

                if (request.TruckCategoryId.HasValue)
                {
                    var truckCategoryExists = await _unitOfWork.TruckCategoryRepository
                        .GetByIdAsync(request.TruckCategoryId.Value);

                    if (truckCategoryExists == null)
                    {
                        result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruckCategory);
                        return result;
                    }
                }
                var service = _mapper.Map<MoveMate.Domain.Models.Service>(request);
                service.Tier = request.InverseParentService.Count < 1 ? 1 : 0;

                if (service.Tier == 1)
                {
                    if (!request.ParentServiceId.HasValue)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ParentServiceIdTier1);
                        return result;
                    }

                    var parentService = await _unitOfWork.ServiceRepository
                        .GetTierZeroServiceByParentIdAsync(request.ParentServiceId.Value);

                    if (parentService == null)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ParentServiceIdTier0);
                        return result;
                    }
                    if (parentService.Type != request.Type)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.SynchronizeType);
                        return result;
                    }

                    if (request.Type == TypeServiceEnums.TRUCK.ToString() && request.TruckCategoryId.HasValue)
                    {
                        var existingService = await _unitOfWork.ServiceRepository
                        .FindByParentTypeAndTruckCategoryAsync(request.ParentServiceId.Value, request.Type, (int)request.TruckCategoryId);

                        if (existingService != null)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ServiceExisted);
                            return result;
                        }
                    }
                    

                    service.ParentServiceId = request.ParentServiceId;
                }
                foreach (var inverseService in request.InverseParentService)
                {
                    if (inverseService.Type != request.Type)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InverseParentServiceType);
                        return result;
                    }
                }

                await _unitOfWork.ServiceRepository.AddAsync(service);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<ServicesResponse>(service);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateService, response);

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
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundService);
                    return result;
                }
                if (service.IsActived == false)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ServiceAlreadyDeleted);
                    return result;
                }

                service.IsActived = false;

                _unitOfWork.ServiceRepository.Update(service);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteService, true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }


        /// <summary>
        /// UpdateService sads
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<OperationResult<ServicesResponse>> UpdateService(int id, UpdateServiceRequest request)
        {
            var result = new OperationResult<ServicesResponse>();
            try
            {
                var service = await _unitOfWork.ServiceRepository.GetByIdAsync(id);
                if (service == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundService);
                    return result;
                }

                var parentService = await _unitOfWork.ServiceRepository.GetByIdAsync((int)service.ParentServiceId);
                if (parentService == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundParentService);
                    return result;
                }

                if (service.Tier == 0 && request.ParentServiceId.HasValue)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CannotUpdateParentForTierZero);
                    return result;
                }

                if (request.Type == TypeServiceEnums.TRUCK.ToString() && !request.TruckCategoryId.HasValue)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TruckCategoryRequire);
                    return result;
                }
                if (request.TruckCategoryId.HasValue && request.Type != TypeServiceEnums.TRUCK.ToString() && service.Type != TypeServiceEnums.TRUCK.ToString())
                {                 
                   result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TypeTruckRequire);
                   return result;                                    
                }
                if (request.Type != parentService.Type && !request.ParentServiceId.HasValue)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.SynchronizeType);
                    return result;
                }

                if ( request.TruckCategoryId.HasValue)
                {
                    if (request.Type == TypeServiceEnums.TRUCK.ToString())
                    {
                        var existingService = await _unitOfWork.ServiceRepository
                            .FindByParentTypeAndTruckCategoryAsync((int)service.ParentServiceId, request.Type, (int)request.TruckCategoryId);

                        if (existingService != null)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ServiceExisted);
                            return result;
                        }
                    }
                    else if (service.Type == TypeServiceEnums.TRUCK.ToString())
                    {
                        var existingService = await _unitOfWork.ServiceRepository
                            .FindByParentTypeAndTruckCategoryAsync((int)service.ParentServiceId, service.Type, (int)request.TruckCategoryId);

                        if (existingService != null)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ServiceExisted);
                            return result;
                        }
                    }
                   
                }


                if (request.ParentServiceId.HasValue)
                {
                    
                    var parentServiceReq = await _unitOfWork.ServiceRepository.GetByIdAsync(request.ParentServiceId.Value);
                    if (parentServiceReq == null)
                    {
                        result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundParentService);
                        return result;
                    }
                    if (request.Type != parentServiceReq.Type)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.SynchronizeType);
                        return result;
                    }
                    if (parentServiceReq.Tier != 0)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ParentServiceIdTier0);
                        return result;
                    }

                    if (request.Type == TypeServiceEnums.TRUCK.ToString() && request.TruckCategoryId.HasValue)
                    {
                        var existingService = await _unitOfWork.ServiceRepository
                        .FindByParentTypeAndTruckCategoryAsync(request.ParentServiceId.Value, request.Type, (int)request.TruckCategoryId);

                        if (existingService != null)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ServiceExisted);
                            return result;
                        }
                    }
                }

                ReflectionUtils.UpdateProperties(request, service);

                await _unitOfWork.ServiceRepository.SaveOrUpdateAsync(service);
                var saveResult = _unitOfWork.Save();
                if (saveResult > 0)
                {
                    service = await _unitOfWork.ServiceRepository.GetByIdAsync(service.Id);
                    var response = _mapper.Map<ServicesResponse>(service);

                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.ServiceUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ServiceUpdateFail);
                }

            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }
            return result;
        }
    }
}