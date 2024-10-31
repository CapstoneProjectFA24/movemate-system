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

        public Task<OperationResult<GetFeeSettingResponse>> UpdateFeeSetting(int id, UpdatePromotionRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<GetFeeSettingResponse>> CreateFeeSetting(CreatePromotionRequest request)
        {
            throw new NotImplementedException();
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