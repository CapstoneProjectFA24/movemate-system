using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using FirebaseAdmin.Auth;
using MoveMate.Service.Utils;
using System.ComponentModel.DataAnnotations;
using Catel.Collections;

namespace MoveMate.Service.Services
{
    public class TruckServices : ITruckServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<TruckServices> _logger;

        public TruckServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TruckServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        //public async Task<OperationResult<List<TruckCateResponse>>> GetAllCate()
        //{
        //    // deplay
        //    BackgroundJob.Schedule(() => Console.WriteLine("delay---"), TimeSpan.FromMinutes(1));

        //    var result = new OperationResult<List<TruckCateResponse>>();

        //    var response = _unitOfWork.TruckCategoryRepository.GetWithPagination();

        //    var listResponse = _mapper.Map<List<TruckCateResponse>>(response.Results);

        //    var pagin = response.Pagination;

        //    if (listResponse == null || !listResponse.Any())
        //    {
        //        result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTruckEmpty, listResponse, pagin);
        //        return result;
        //    }

        //}

        public async Task<OperationResult<List<TruckCategoryResponse>>> GetAllTruckCategory(
            GetAllTruckCategoryRequest request)
        {
            var result = new OperationResult<List<TruckCategoryResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.TruckCategoryRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder()
                );
                var listResponse = _mapper.Map<List<TruckCategoryResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok,
                        MessageConstant.SuccessMessage.GetListTruckCategoryEmpty, listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTruckCategorySuccess,
                    listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        //public async Task<OperationResult<TruckCateDetailResponse>> GetCateById(int id)
        //{
        //    var result = new OperationResult<TruckCateDetailResponse>();

        //    var entity = _unitOfWork.TruckCategoryRepository.Get(t => t.Id == id);

        //    var images = await _unitOfWork.TruckCategoryRepository.GetFirstTruckImagesByCategoryIdAsync(id);

        //    var response = _mapper.Map<TruckCateDetailResponse>(entity.FirstOrDefault());
        //    response.TruckImgs = images.Select(img => new TruckImgResponse
        //    {
        //        ImageUrl = img,
        //    }).ToList();

        //    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetTruckSuccess, response);
        //    return result;
        //}

        public async Task<OperationResult<bool>> DeleteTruckImg(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var truckImg = await _unitOfWork.TruckImgRepository.GetByIdAsync(id);
                if (truckImg == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruckImg);
                    return result;
                }

                _unitOfWork.TruckImgRepository.Remove(truckImg);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteTruckImg, true);
            }
            catch
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<TruckImageResponse>> CreateTruckImg(CreateTruckImgRequest request)
        {
            var result = new OperationResult<TruckImageResponse>();

            try
            {
                var truck = await _unitOfWork.TruckRepository.GetByIdAsync((int)request.TruckId);
                if (truck == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruck);
                    return result;
                }

                var truckImg = _mapper.Map<TruckImg>(request);

                await _unitOfWork.TruckImgRepository.AddAsync(truckImg);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<TruckImageResponse>(truckImg);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateTruckImg,
                    response);

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<bool>> DeleteTruckCategory(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var truck = await _unitOfWork.TruckCategoryRepository.GetByIdAsync(id);
                if (truck == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruckCategory);
                    return result;
                }

                if (truck.IsDeleted == true)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TruckCategoryAlreadyDeleted);
                    return result;
                }

                truck.IsDeleted = true;

                _unitOfWork.TruckCategoryRepository.Update(truck);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteTruckCategory, true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<TruckCategoryResponse>> CreateTruckCategory(TruckCategoryRequest request)
        {
            var result = new OperationResult<TruckCategoryResponse>();

            try
            {
                var truckCategory = _mapper.Map<TruckCategory>(request);

                await _unitOfWork.TruckCategoryRepository.AddAsync(truckCategory);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<TruckCategoryResponse>(truckCategory);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateTruckCategory,
                    response);

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<TruckCategoryResponse>> UpdateTruckCategory(int id,
            TruckCategoryRequest request)
        {
            var result = new OperationResult<TruckCategoryResponse>();
            try
            {
                var truckCategory = await _unitOfWork.TruckCategoryRepository.GetByIdAsync(id);
                if (truckCategory == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruckCategory);
                    return result;
                }

                ReflectionUtils.UpdateProperties(request, truckCategory);
                await _unitOfWork.TruckCategoryRepository.SaveOrUpdateAsync(truckCategory);
                var saveResult = _unitOfWork.Save();

                // Check save result and return response
                if (saveResult > 0)
                {
                    truckCategory = await _unitOfWork.TruckCategoryRepository.GetByIdAsync(truckCategory.Id);
                    var response = _mapper.Map<TruckCategoryResponse>(truckCategory);

                    result.AddResponseStatusCode(StatusCode.Ok,
                        MessageConstant.SuccessMessage.TruckCategoryUpdateSuccess,
                        response);
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

        public async Task<OperationResult<TruckCategoryResponse>> GetTruckCategoryById(int id)
        {
            var result = new OperationResult<TruckCategoryResponse>();
            try
            {
                var entity =
                    await _unitOfWork.TruckCategoryRepository.GetByIdAsync(id);

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruckCategory);
                }
                else
                {
                    var productResponse = _mapper.Map<TruckCategoryResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetTruckCategorySuccess,
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

        public async Task<OperationResult<List<TruckResponse>>> GetAllTruck(GetAllTruckRequest request)
        {
            var result = new OperationResult<List<TruckResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.TruckRepository.GetWithCount(
                    filter: request.GetExpressions(),
                    pageIndex: request.page,
                    pageSize: request.per_page,
                    orderBy: request.GetOrder(),
                    includeProperties: "TruckImgs"
                );
                var listResponse = _mapper.Map<List<TruckResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTruckEmpty,
                        listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTruckSuccess,
                    listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }

        public async Task<OperationResult<TruckResponse>> GetTruckById(int id)
        {
            var result = new OperationResult<TruckResponse>();
            try
            {
                var entity =
                    await _unitOfWork.TruckRepository.GetByIdAsync(id, includeProperties: "TruckImgs");

                if (entity == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruck);
                }
                else
                {
                    var productResponse = _mapper.Map<TruckResponse>(entity);
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetTruckSuccess,
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

        public async Task<OperationResult<TruckResponse>> UpdateTruck(int truckId, UpdateTruckRequest request)
        {
            var result = new OperationResult<TruckResponse>();

            // Validation of all fields in the request
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (!isValid)
            {
                foreach (var validationResult in validationResults)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.ValidateField);
                }
                return result;
            }

            try
            {
                var truck = await _unitOfWork.TruckRepository.GetByIdAsync(truckId, includeProperties: "TruckImgs");
                if (truck == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruck);
                    return result;
                }

                var truckCategory = await _unitOfWork.TruckCategoryRepository.GetByIdAsync(request.TruckCategoryId);
                if (truckCategory == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruckCategory);
                    return result;
                }

                // Update fields except TruckImgs
                ReflectionUtils.UpdateProperties(request, truck, excludeProperties: new List<string> { "TruckImgs" });

                // Handle TruckImgs: Clear and add if empty, else update
                if (truck.TruckImgs == null || !truck.TruckImgs.Any())
                {
                    // Add new TruckImgs if current TruckImgs are empty
                    truck.TruckImgs = request.TruckImgs.Select(imgRequest => new TruckImg
                    {
                        TruckId = truck.Id,
                        ImageUrl = imgRequest.ImageUrl,
                        ImageCode = imgRequest.ImageCode
                    }).ToList();
                }
                else
                {
                    // Remove existing TruckImgs
                    foreach (var existingImg in truck.TruckImgs.ToList())
                    {
                        _unitOfWork.TruckImgRepository.Remove(existingImg);
                    }

                    // Add new TruckImgs from request
                    truck.TruckImgs.AddRange(request.TruckImgs.Select(imgRequest => new TruckImg
                    {
                        TruckId = truck.Id,
                        ImageUrl = imgRequest.ImageUrl,
                        ImageCode = imgRequest.ImageCode
                    }));
                }

                // Update truck entity
                _unitOfWork.TruckRepository.Update(truck);
                await _unitOfWork.SaveChangesAsync();

                // Get updated truck with TruckImgs
                truck = await _unitOfWork.TruckRepository.GetByIdAsync(truckId, includeProperties: "TruckImgs");
                var response = _mapper.Map<TruckResponse>(truck);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.TruckUpdateSuccess, response);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }






        public async Task<OperationResult<TruckResponse>> CreateTruck(CreateTruckRequest request)
        {
            var result = new OperationResult<TruckResponse>();

            try
            {
                var truckCategory =
                    await _unitOfWork.TruckCategoryRepository.GetByIdAsync((int)request.TruckCategoryId);
                if (truckCategory == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruckCategory);
                    return result;
                }

                var user = await _unitOfWork.UserRepository.GetByIdAsync((int)request.UserId);
                if (user == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUserInfo);
                    return result;
                }

                if (user.IsDriver == false)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.UserNotDriver);
                    return result;
                }

                var truckExist = await _unitOfWork.TruckRepository.FindByUserIdAsync((int)request.UserId);
                if (truckExist != null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.UserHaveTruck);
                    return result;
                }

                List<TruckImg> truckImgList = _mapper.Map<List<TruckImg>>(request.TruckImgs);

                var truck = _mapper.Map<Truck>(request);

                await _unitOfWork.TruckRepository.AddAsync(truck);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<TruckResponse>(truck);
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateTruckImg,
                    response);

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<bool>> DeleteTruck(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var truck = await _unitOfWork.TruckRepository.GetByIdAsync(id);
                if (truck == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruck);
                    return result;
                }

                if (truck.IsDeleted == true)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TruckAlreadyDeleted);
                    return result;
                }

                truck.IsDeleted = true;

                _unitOfWork.TruckRepository.Update(truck);
                _unitOfWork.Save();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.DeleteTruck, true);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }
    }
}