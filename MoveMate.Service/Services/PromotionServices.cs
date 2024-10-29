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

                promotion.IsPublic = request.IsPublic;
                promotion.Name = request.Name;
                promotion.Description = request.Description;
                promotion.StartDate = request.StartDate;
                promotion.EndDate = request.EndDate;
                promotion.DiscountRate = request.DiscountRate;
                promotion.DiscountMax = request.DiscountMax;
                promotion.DiscountMin = request.DiscountMin;
                promotion.RequireMin = request.RequireMin;
                promotion.Type = request.Type;
                promotion.Quantity = request.Quantity;
                promotion.StartBookingTime = request.StartBookingTime;
                promotion.EndBookingTime = request.EndBookingTime;
                promotion.IsInfinite = request.IsInfinite;
                promotion.ServiceId = request.ServiceId;

                if (promotion.Vouchers.Any())
                {
                    foreach (var existingImg in promotion.Vouchers.ToList())
                    {
                        _unitOfWork.VoucherRepository.Remove(existingImg);
                    }
                    await _unitOfWork.SaveChangesAsync(); // Save deletion changes to the database
                }

                // Add new images from the request if provided
                if (request.Vouchers != null && request.Vouchers.Any())
                {
                    foreach (var imgRequest in request.Vouchers)
                    {
                        var truckImg = new Voucher
                        {
                            PromotionCategoryId = promotion.Id,
                            Code = imgRequest.Code,
                            Price = imgRequest.Price
                        };
                        promotion.Vouchers.Add(truckImg);
                    }
                }

                // Update truck and save changes
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

        public async Task<OperationResult<PromotionResponse>> CreatePromotion(CreatePromotionRequest request)
        {
            var result = new OperationResult<PromotionResponse>();
            try
            {
                var service = await _unitOfWork.PromotionCategoryRepository.GetByIdAsync((int)request.ServiceId);
                if (service == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundPromotion);
                    return result;
                }          
                List<Voucher> voucher = _mapper.Map<List<Voucher>>(request.Vouchers);
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