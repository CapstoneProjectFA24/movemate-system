using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.IRepository;
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
    public class VoucherService : IVoucherService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<VoucherService> _logger;

        public VoucherService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<VoucherService> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<List<VoucherResponse>>> GetAllVoucher(GetAllVoucherRequest request)
        {
            var result = new OperationResult<List<VoucherResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.VoucherRepository.GetWithCount(
                filter: request.GetExpressions(),
                pageIndex: request.page,
                pageSize: request.per_page,
                orderBy: request.GetOrder()
            );
                var listResponse = _mapper.Map<List<VoucherResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListVoucherEmpty, listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListVoucherSuccess, listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<VoucherResponse>> GetVourcherById(int id)
        {
            var result = new OperationResult<VoucherResponse>();
            try
            {
                var entity =
                    await _unitOfWork.VoucherRepository.GetByIdAsync(id);

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundVoucher);
                }
                else
                {
                    var productResponse = _mapper.Map<VoucherResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetVoucherSuccess, productResponse);
                }
                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        public async Task<OperationResult<VoucherResponse>> UpdateVoucher(int id, CreateVoucherRequest request)
        {
            var result = new OperationResult<VoucherResponse>();
            try
            {
                var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(id);
                if (voucher == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundVoucher);
                    return result;
                }
                if (request.PromotionCategoryId.HasValue)
                {
                    var promotion = await _unitOfWork.PromotionCategoryRepository.GetByIdAsync((int)request.PromotionCategoryId);
                    if (promotion == null)
                    {
                        result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundPromotion);
                        return result;
                    }
                    voucher.PromotionCategoryId = request.PromotionCategoryId;
                }
               
                voucher.Code = request.Code;
                voucher.Price = request.Price;

                _unitOfWork.VoucherRepository.Update(voucher);
                await _unitOfWork.SaveChangesAsync();

                // Fetch the updated truck with all images to ensure the response includes them
                voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(id);

                // Map updated truck to response model
                var response = _mapper.Map<VoucherResponse>(voucher);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.PromotionUpdateSuccess, response);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }
            return result;
        }

        public async Task<OperationResult<VoucherResponse>> CreateVoucher(CreateVoucherRequest request)
        {
            var result = new OperationResult<VoucherResponse>();
            try
            {
                
                var promotion = await _unitOfWork.PromotionCategoryRepository.GetByIdAsync((int)request.PromotionCategoryId);
                if (promotion == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundPromotion);
                    return result;
                }
                promotion.Quantity += 1;
                var voucher = _mapper.Map<Voucher>(request);

                await _unitOfWork.VoucherRepository.AddAsync(voucher);
                _unitOfWork.PromotionCategoryRepository.Update(promotion);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<VoucherResponse>(voucher);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateVoucher, response);

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<bool>> DeleteVouver(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(id);
                if (voucher == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundVoucher);
                    return result;
                }
                if (voucher.IsDeleted == true)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VoucherAlreadyDeleted);
                    return result;
                }
                voucher.IsDeleted = true;

                _unitOfWork.VoucherRepository.Update(voucher);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteVoucher, true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<VoucherResponse>> AssignVoucherToUser(int voucherId, int userId)
        {
            var result = new OperationResult<VoucherResponse>();
            try
            {
                var voucher = await _unitOfWork.VoucherRepository.GetByIdAsync(voucherId);
                if (voucher == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundVoucher);
                    return result;
                }

                var promotion = await _unitOfWork.PromotionCategoryRepository.GetByIdAsync((int)voucher.PromotionCategoryId);
                if (promotion == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundPromotion);
                    return result;
                }

                if (voucher.UserId.HasValue)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VoucherAlreadyAssigned);
                    return result;
                }

                if (promotion.Quantity <= 0)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.PromotionRunOut);
                }

                // Assign the UserId to the voucher
                voucher.UserId = userId;
                promotion.Quantity = promotion.Quantity - 1;

                _unitOfWork.PromotionCategoryRepository.Update(promotion);
                _unitOfWork.VoucherRepository.Update(voucher);
                await _unitOfWork.SaveChangesAsync();

                // Map updated voucher to response model
                var response = _mapper.Map<VoucherResponse>(voucher);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.AssignVoucherToUserSuccess, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning voucher to user");
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

    }

}
