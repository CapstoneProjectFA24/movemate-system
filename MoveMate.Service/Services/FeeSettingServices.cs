using AutoMapper;
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
    public class FeeSettingServices : IFeeSettingServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<FeeSettingServices> _logger;

        public FeeSettingServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<FeeSettingServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<List<GetFeeSettingResponse>>> GetAll(GetAllFeeSetting request)
        {
            var result = new OperationResult<List<GetFeeSettingResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.FeeSettingRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder()
                );
                var listResponse = _mapper.Map<List<GetFeeSettingResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListFeeSettingEmpty, listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListFeeSettingSuccess, listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }



        public async Task<OperationResult<GetFeeSettingResponse>> GetFeeSettingById(int id)
        {
            var result = new OperationResult<GetFeeSettingResponse>();
            try
            {
                var entity =
                    await _unitOfWork.FeeSettingRepository.GetByIdAsync(id);

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundFeeSetting);
                }
                else
                {
                    var productResponse = _mapper.Map<GetFeeSettingResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetFeeSettingSuccess,
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

        public async Task<OperationResult<GetFeeSettingResponse>> UpdateFeeSetting(int id, CreateFeeSettingRequest request)
        {
            var result = new OperationResult<GetFeeSettingResponse>();

            try
            {
                var feeSetting = await _unitOfWork.FeeSettingRepository.GetByIdAsync(id);
                if (feeSetting == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruckCategory);
                    return result;
                }


                if (request.HouseTypeId.HasValue)
                {
                    var houseType = await _unitOfWork.HouseTypeRepository.GetByIdAsync((int)request.HouseTypeId);
                    if (houseType == null)
                    {
                        result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundHouseType);
                        return result;
                    }
                }
                if (request.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServiceRepository.GetByIdAsync((int)request.ServiceId);
                    if (service == null)
                    {
                        result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundService);
                        return result;
                    }
                    if (service.Tier != 1)
                    {
                        result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.ServiceTier1);
                        return result;
                    }
                }
                if (request.Type == TypeFeeEnums.TRUCK.ToString())
                {
                    if (request.HouseTypeId.HasValue || (!request.HouseTypeId.HasValue && feeSetting.HouseTypeId.HasValue))
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.FeeTypeTruckFail);
                        return result;
                    }
                    if (request.Unit != UnitEnums.KM.ToString() || (string.IsNullOrEmpty(request.Unit) && feeSetting.Unit != UnitEnums.KM.ToString()))
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.FeeUnitKMFail);
                        return result;
                    }
                    if (request.ServiceId.HasValue)
                    {
                        var service = await _unitOfWork.ServiceRepository.GetByIdAsync((int)request.ServiceId);
                        if (service == null)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundService);
                            return result;
                        }
                        if (service.Type != TypeServiceEnums.TRUCK.ToString())
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ServiceTypeTruck);
                            return result;
                        }
                        if (!service.TruckCategoryId.HasValue)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.ServiceTruckCategory);
                            return result;
                        }
                    }

                }
                if (request.Type == TypeFeeEnums.PORTER.ToString())
                {
                    if (request.Unit == UnitEnums.KM.ToString() || (string.IsNullOrEmpty(request.Unit) && feeSetting.Unit == UnitEnums.KM.ToString()))
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.FeeUnitNotKMFail);
                        return result;
                    }
                }
                if (request.Type == TypeFeeEnums.SYSTEM.ToString())
                {
                    if (request.Unit != UnitEnums.PERCENT.ToString() || (string.IsNullOrEmpty(request.Unit) && feeSetting.Unit != UnitEnums.PERCENT.ToString()))
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.FeeUnitPercentFail);
                        return result;
                    }
                }
                if ((request.Unit == UnitEnums.FLOOR.ToString() && !request.FloorPercentage.HasValue) || (request.Unit == UnitEnums.FLOOR.ToString() && !request.FloorPercentage.HasValue && !feeSetting.FloorPercentage.HasValue))
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.FeeUnitFloorFail);
                    return result;
                }
                if (request.Type == TypeFeeEnums.PORTER.ToString() || request.Type == TypeFeeEnums.DRIVER.ToString() || request.Type == TypeFeeEnums.TRUCK.ToString())
                {
                    if (!request.ServiceId.HasValue)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotServiceFeeFail);
                        return result;
                    }
                }
                else
                {
                    if (request.ServiceId.HasValue)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ServiceFeeFail);
                        return result;
                    }
                }

                // Update properties and save
                ReflectionUtils.UpdateProperties(request, feeSetting);
                await _unitOfWork.FeeSettingRepository.SaveOrUpdateAsync(feeSetting);
                var saveResult = _unitOfWork.Save();

                if (saveResult > 0)
                {
                    feeSetting = await _unitOfWork.FeeSettingRepository.GetByIdAsync(feeSetting.Id);
                    var response = _mapper.Map<GetFeeSettingResponse>(feeSetting);

                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.TruckCategoryUpdateSuccess, response);
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


        public async Task<OperationResult<GetFeeSettingResponse>> CreateFeeSetting(CreateFeeSettingRequest request)
        {
            var result = new OperationResult<GetFeeSettingResponse>();

            try
            {
                if (request.HouseTypeId == 0)
                {
                    request.HouseTypeId = null;
                }

                if (request.HouseTypeId.HasValue)
                {
                    var houseType = await _unitOfWork.HouseTypeRepository.GetByIdAsync((int)request.HouseTypeId);
                    if (houseType == null)
                    {
                        result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundHouseType);
                        return result;
                    }
                }

                if (request.ServiceId == 0)
                {
                    request.ServiceId = null;
                }
                if (request.ServiceId.HasValue)
                {
                    var service = await _unitOfWork.ServiceRepository.GetByIdAsync((int)request.ServiceId);
                    if (service == null)
                    {
                        result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundService);
                        return result;
                    }
                    if (service.Tier != 1)
                    {
                        result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.ServiceTier1);
                        return result;
                    }
                }


                if (request.Type == TypeFeeEnums.TRUCK.ToString())
                {
                    if (request.HouseTypeId.HasValue)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.FeeTypeTruckFail);
                        return result;
                    }
                    if (request.Unit != UnitEnums.KM.ToString())
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.FeeUnitKMFail);
                        return result;
                    }
                    if (request.ServiceId.HasValue)
                    {
                        var service = await _unitOfWork.ServiceRepository.GetByIdAsync((int)request.ServiceId);
                        if (service == null)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundService);
                            return result;
                        }
                        if (service.Type != TypeServiceEnums.TRUCK.ToString())
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ServiceTypeTruck);
                            return result;
                        }
                        if (!service.TruckCategoryId.HasValue)
                        {
                            result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.ServiceTruckCategory);
                            return result;
                        }


                    }


                }
                if (request.Type == TypeFeeEnums.PORTER.ToString() || request.Type == TypeFeeEnums.DRIVER.ToString())
                {
                    if (request.Unit == UnitEnums.KM.ToString())
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.FeeUnitNotKMFail);
                        return result;
                    }
                }
                if (request.Type == TypeFeeEnums.SYSTEM.ToString())
                {
                    if (request.Unit != UnitEnums.PERCENT.ToString())
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.FeeUnitPercentFail);
                        return result;
                    }
                }

                if (request.Unit == UnitEnums.FLOOR.ToString())
                {
                    if (!request.FloorPercentage.HasValue)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.FeeUnitFloorFail);
                        return result;
                    }
                }
                if (request.Type == TypeFeeEnums.PORTER.ToString() || request.Type == TypeFeeEnums.DRIVER.ToString() || request.Type == TypeFeeEnums.TRUCK.ToString())
                {
                    if (!request.ServiceId.HasValue)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.NotServiceFeeFail);
                        return result;
                    }
                }
                else
                {
                    if (request.ServiceId.HasValue)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ServiceFeeFail);
                        return result;
                    }
                }
                if (request.Unit == UnitEnums.KM.ToString() || request.Unit == UnitEnums.FLOOR.ToString())
                {
                    if (request.RangeMin > request.RangeMax)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.RangeMinMax);
                        return result;
                    }
                    var feeSettings = await _unitOfWork.FeeSettingRepository.GetByServiceIdAsync((int)request.ServiceId);
                    
                    if (request.RangeMin != 0)
                    {
                        var isMatchingRange = feeSettings.Any(fee => fee.RangeMax == request.RangeMin);

                        if (!isMatchingRange)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.RangeFalse);
                            return result;
                        }
                    }
                    else
                    {
                        var isDuplicate = feeSettings.Any(fee => fee.RangeMin == request.RangeMin);
                        if (isDuplicate)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ExistFeeRange);
                            return result;
                        }
                    }
                }
                var feeSetting = _mapper.Map<FeeSetting>(request);

                await _unitOfWork.FeeSettingRepository.AddAsync(feeSetting);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<GetFeeSettingResponse>(feeSetting);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateTruckImg, response);
                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<bool>> DeleteActiveFeeSetting(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var feeSetting = await _unitOfWork.FeeSettingRepository.GetByIdAsync(id);
                if (feeSetting == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundFeeSetting);
                    return result;
                }

                if (feeSetting.IsActived == false)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.FeeSettingAlreadyDeleted);
                    return result;
                }

                feeSetting.IsActived = false;

                _unitOfWork.FeeSettingRepository.Update(feeSetting);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteFeeSetiing, true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }
    }
}