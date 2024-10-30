using AutoMapper;
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
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class PromotionServices : IPromotionServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<PromotionServices> _logger;

        public PromotionServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PromotionServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<List<PromotionResponse>>> GetAllPromotion(GetAllPromotionRequest request)
        {
            var result = new OperationResult<List<PromotionResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.PromotionCategoryRepository.GetWithCount(
                filter: request.GetExpressions(),
                pageIndex: request.page,
                pageSize: request.per_page,
                orderBy: request.GetOrder(),
                includeProperties : "Vouchers"
            );
                var listResponse = _mapper.Map<List<PromotionResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListPromotionEmpty, listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListPromotionSuccess, listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<PromotionResponse>> GetPromotionById(int id)
        {
            var result = new OperationResult<PromotionResponse>();
            try
            {
                var entity =
                    await _unitOfWork.PromotionCategoryRepository.GetByIdAsync(id, includeProperties: "Vouchers");

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundPromotion);
                }
                else
                {
                    var productResponse = _mapper.Map<PromotionResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetPromotionCategorySuccess, productResponse);
                }
                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        public async Task<OperationResult<PromotionResponse>> UpdatePromotion(int id, UpdatePromotionRequest request)
        {
            var result = new OperationResult<PromotionResponse>();
            try
            {
                var promotion = await _unitOfWork.PromotionCategoryRepository.GetByIdAsync(id, includeProperties: "Vouchers");
                if (promotion == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundPromotion);
                    return result;
                }

                
                var service = await _unitOfWork.ServiceRepository.GetByIdAsync((int)request.ServiceId);
                if (service == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundService);
                    return result;
                }

                int assignedVoucherCount = promotion.Vouchers.Count(v => v.UserId.HasValue);

                // Validate that the new quantity is not less than the assigned vouchers
                if (request.Quantity < assignedVoucherCount)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.LessAssigned);
                    return result;
                }

                if (request.Quantity == assignedVoucherCount)
                {
                    // Remove all vouchers with null UserId
                    var unassignedVouchers = promotion.Vouchers.Where(v => !v.UserId.HasValue).ToList();
                    foreach (var voucher in unassignedVouchers)
                    {
                       _unitOfWork.VoucherRepository.Remove(voucher);
                    }
                }


                ReflectionUtils.UpdateProperties(request, promotion);
                int currentVoucherCount = promotion.Vouchers.Count;
                int vouchersToAdd = request.Quantity - currentVoucherCount;
                for (int i = 0; i < vouchersToAdd; i++)
                {
                    var newVoucher = new Voucher
                    {
                        PromotionCategoryId = promotion.Id,
                        Code = GenerateVoucherCode(), 
                        IsActived = false,
                        IsDeleted = false
                    };
                    await _unitOfWork.VoucherRepository.AddAsync(newVoucher);
                }

                _unitOfWork.PromotionCategoryRepository.Update(promotion);
                await _unitOfWork.SaveChangesAsync();

                // Fetch the updated truck with all images to ensure the response includes them
                promotion = await _unitOfWork.PromotionCategoryRepository.GetByIdAsync(id, includeProperties: "Vouchers");

                // Map updated truck to response model
                var response = _mapper.Map<PromotionResponse>(promotion);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.PromotionUpdateSuccess, response);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }
            return result;
        }


        

        public static string GenerateVoucherCode(int length = 10)
        {
            Random _random = new Random();
            string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var voucherCode = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                voucherCode.Append(_chars[_random.Next(_chars.Length)]);
            }

            return voucherCode.ToString();
        }
        public async Task<OperationResult<PromotionResponse>> CreatePromotion(CreatePromotionRequest request)
        {
            var result = new OperationResult<PromotionResponse>();
            try
            {
                
                var service = await _unitOfWork.PromotionCategoryRepository.GetByIdAsync((int)request.ServiceId);
                if (service == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundService);
                    return result;
                }

                if (!DateUtil.IsAtLeast24HoursApart((DateTime)request.StartDate, (DateTime)request.EndDate))
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.IsAtLeast24HoursApart);
                    return result;
                }

                List<Voucher> vouchers = _mapper.Map<List<Voucher>>(request.Vouchers);
                if (vouchers.Count != request.Quantity)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VoucherLessThanQuantity);
                    return result;
                }
                var promotion = _mapper.Map<PromotionCategory>(request);
                await _unitOfWork.PromotionCategoryRepository.AddAsync(promotion);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<PromotionResponse>(promotion);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreatePromotion, response);

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }




        public async Task<OperationResult<bool>> DeletePromotion(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var promotion = await _unitOfWork.PromotionCategoryRepository.GetByIdAsync(id);
                if (promotion == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundPromotion);
                    return result;
                }
                int assignedVoucherCount = promotion.Vouchers.Count(v => v.UserId.HasValue);
                if (assignedVoucherCount > 0)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.VoucherHasBeenAssigned);
                    return result;
                }
                if (promotion.IsDeleted == true)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.PromotionAlreadyDeleted);
                    return result;
                }
                promotion.IsDeleted = true;

                _unitOfWork.PromotionCategoryRepository.Update(promotion);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeletePromotionCategory, true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }
    }
}