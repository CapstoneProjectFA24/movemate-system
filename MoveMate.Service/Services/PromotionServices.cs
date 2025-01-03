﻿using AutoMapper;
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
using System.ComponentModel.DataAnnotations;
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
                if (request.StartDate.HasValue && !request.EndDate.HasValue)
                {
                    if (request.StartDate >= promotion.EndDate)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InvalidDates);
                        return result;
                    }

                    if (!DateUtil.IsAtLeast24HoursApart((DateTime)request.StartDate, (DateTime)promotion.EndDate))
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.IsAtLeast24HoursApart);
                        return result;
                    }
                }
                if (!request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    if (promotion.StartDate >= request.EndDate)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InvalidDates);
                        return result;
                    }

                    if (!DateUtil.IsAtLeast24HoursApart((DateTime)promotion.StartDate, (DateTime)request.EndDate))
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.IsAtLeast24HoursApart);
                        return result;
                    }
                }
                if (request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    if (request.StartDate >= request.EndDate)
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InvalidDates);
                        return result;
                    }

                    if (!DateUtil.IsAtLeast24HoursApart((DateTime)request.StartDate, (DateTime)request.EndDate))
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.IsAtLeast24HoursApart);
                        return result;
                    }
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
                var vouchers = await _unitOfWork.VoucherRepository.GetVoucherWithHighestPriceByPromotionIdAsync(promotion.Id);
                if (request.Quantity > assignedVoucherCount)
                {
                    int currentVoucherCount = promotion.Vouchers.Count;
                    int vouchersToAdd = (int)(request.Quantity - currentVoucherCount);
                    for (int i = 0; i < vouchersToAdd; i++)
                    {
                        var newVoucher = new Voucher
                        {
                            PromotionCategoryId = promotion.Id,
                            Code = GenerateVoucherCode(),
                            IsActived = false,
                            Price = vouchers.Price,
                            IsDeleted = false
                        };
                        await _unitOfWork.VoucherRepository.AddAsync(newVoucher);
                    }
                }   
                _unitOfWork.PromotionCategoryRepository.Update(promotion);
                await _unitOfWork.SaveChangesAsync();
                promotion = await _unitOfWork.PromotionCategoryRepository.GetByIdAsync(id, includeProperties: "Vouchers");

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
                
                var service = await _unitOfWork.ServiceRepository.GetByIdAsyncV1((int)request.ServiceId, includeProperties: "InverseParentService");
                if (service == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundService);
                    return result;
                }

                if (service.Tier == 0)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.SuperService);
                    return result;
                }

                if (request.StartDate >= request.EndDate)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.InvalidDates);
                    return result;
                }

                if (!DateUtil.IsAtLeast24HoursApart((DateTime)request.StartDate, (DateTime)request.EndDate))
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.IsAtLeast24HoursApart);
                    return result;
                }

               
                var promotion = _mapper.Map<PromotionCategory>(request);
                await _unitOfWork.PromotionCategoryRepository.AddAsync(promotion);
                await _unitOfWork.SaveChangesAsync();
                if (request.Quantity.HasValue && request.Quantity.Value > 0)
                {
                    var vouchers = new List<Voucher>();
                    for (int i = 0; i < request.Quantity.Value; i++)
                    {
                        var voucher = new Voucher
                        {
                            PromotionCategoryId = promotion.Id,
                            Price = request.Amount,
                            Code = GenerateVoucherCode(10), // Tạo mã voucher
                            IsActived = false,
                            IsDeleted = false
                        };
                        vouchers.Add(voucher);
                    }

                    await _unitOfWork.VoucherRepository.AddRangeAsync(vouchers);
                }
                _unitOfWork.Save();
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
                    result.AddResponseErrorStatusCode(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundPromotion, false);
                    return result;
                }
                int assignedVoucherCount = promotion.Vouchers.Count(v => v.UserId.HasValue);
                if (assignedVoucherCount > 0)
                {
                    result.AddResponseErrorStatusCode(StatusCode.BadRequest, MessageConstant.FailMessage.VoucherHasBeenAssigned, false);
                    return result;
                }
                if (promotion.IsDeleted == true)
                {
                    result.AddResponseErrorStatusCode(StatusCode.BadRequest, MessageConstant.FailMessage.PromotionAlreadyDeleted, false);
                    return result;
                }
                promotion.IsDeleted = true;

                _unitOfWork.PromotionCategoryRepository.Update(promotion);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeletePromotionCategory, true);
            }
            catch (Exception ex)
            {
                result.AddResponseErrorStatusCode(StatusCode.ServerError, MessageConstant.FailMessage.ServerError, false);
            }

            return result;
        }

        public async Task<OperationResult<GetAllPromotionResponse>> GetListPromotion(int userId, DateTime currentTime)
        {
            var result = new OperationResult<GetAllPromotionResponse>();
            var response = new GetAllPromotionResponse();

            try
            {
                // Promotions with vouchers linked to the given userId
                var promotionUser = await _unitOfWork.PromotionCategoryRepository
                    .GetPromotionsWithUserVoucherAsync(userId,currentTime, includeProperties: "Vouchers");

                // Sort promotions by StartDate (ascending order)
                promotionUser = promotionUser.OrderBy(pc => pc.StartDate).ToList();

                var promotionUserResponses = _mapper.Map<List<PromotionResponse>>(promotionUser);

                // Set IsGot to true for promotions with user vouchers
                foreach (var promotion in promotionUserResponses)
                {
                    promotion.IsGot = true;
                }
                response.PromotionUser.AddRange(promotionUserResponses);

                // Promotions without vouchers linked to the given userId
                var promotionNoUser = await _unitOfWork.PromotionCategoryRepository
                    .GetPromotionsWithNoUserVoucherAsync(userId, currentTime, includeProperties: "Vouchers");

                // Sort promotions by StartDate (ascending order)
                promotionNoUser = promotionNoUser.OrderBy(pc => pc.StartDate).ToList();

                var promotionNoUserResponses = _mapper.Map<List<PromotionResponse>>(promotionNoUser);

                response.PromotionNoUser.AddRange(promotionNoUserResponses);

                result.AddResponseStatusCode(
                    StatusCode.Ok,
                    MessageConstant.SuccessMessage.GetListPromotionSuccess,
                    response);

                return result;
            }
            catch (Exception ex)
            {
                // Handle exceptions and add error response
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }


    }
}