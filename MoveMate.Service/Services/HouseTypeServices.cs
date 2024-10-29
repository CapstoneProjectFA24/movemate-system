using AutoMapper;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class HouseTypeServices : IHouseTypeServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<HouseTypeServices> _logger;

        public HouseTypeServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HouseTypeServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<List<HouseTypeResponse>>> GetAll(GetAllHouseTypeRequest request)
        {
            var result = new OperationResult<List<HouseTypeResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.HouseTypeRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder()
                );
                var listResponse = _mapper.Map<List<HouseTypeResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListHouseTypeEmpty, listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListHouseTypeSuccess, listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<HouseTypesResponse>> GetById(int id)
        {
            var result = new OperationResult<HouseTypesResponse>();
            try
            {
                var entity =
                    await _unitOfWork.HouseTypeRepository.GetByIdAsyncV1(id);

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundHouseType);
                }
                else
                {
                    var productResponse = _mapper.Map<HouseTypesResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetHouseTypeIdSuccess, productResponse);
                }

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        public async Task<OperationResult<HouseTypeResponse>> UpdateHouseType(int id, UpdateHouseTypeRequest request)
        {
            var result = new OperationResult<HouseTypeResponse>();
            try
            {
                var houseType = await _unitOfWork.HouseTypeRepository.GetByIdAsync(id);
                if (houseType == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundHouseType);
                    return result;
                }

                ReflectionUtils.UpdateProperties(request, houseType);
                await _unitOfWork.HouseTypeRepository.SaveOrUpdateAsync(houseType);
                var saveResult = _unitOfWork.Save();

                // Check save result and return response
                if (saveResult > 0)
                {
                    houseType = await _unitOfWork.HouseTypeRepository.GetByIdAsync(houseType.Id);
                    var response = _mapper.Map<HouseTypeResponse>(houseType);

                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.HouseTypeUpdateSuccess,
                        response);
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.HouseTypeUpdateFail);
                }

            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }
            return result;
        }

        public async Task<OperationResult<HouseTypeResponse>> CreateHouseType(CreateHouseTypeRequest request)
        {
            var result = new OperationResult<HouseTypeResponse>();
            try
            {
                var houseType = _mapper.Map<HouseType>(request);
                await _unitOfWork.HouseTypeRepository.AddAsync(houseType);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<HouseTypeResponse>(houseType);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateHouseType, response);
                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }


        public async Task<OperationResult<bool>> DeleteHouseType(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var houseType = await _unitOfWork.HouseTypeRepository.GetByIdAsync(id);
                if (houseType == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundHouseType);
                    return result;
                }
                if (houseType.IsActived == false)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.HouseTypeAlreadyDeleted);
                    return result;
                }

                houseType.IsActived = false;

                _unitOfWork.HouseTypeRepository.Update(houseType);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteHouseType, true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }
    }
}