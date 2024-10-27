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


        public Task<OperationResult<List<TruckResponse>>> GetAll(GetAllTruckRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<List<TruckCateResponse>>> GetAllCate()
        {
            // deplay
            BackgroundJob.Schedule(() => Console.WriteLine("delay---"), TimeSpan.FromMinutes(1));

            var result = new OperationResult<List<TruckCateResponse>>();

            var response = _unitOfWork.TruckCategoryRepository.GetWithPagination();

            var listResponse = _mapper.Map<List<TruckCateResponse>>(response.Results);

            var pagin = response.Pagination;

            if (listResponse == null || !listResponse.Any())
            {
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTruckEmpty, listResponse, pagin);
                return result;
            }

            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListTruckSuccess, listResponse, pagin);

            return result;
        }

        public async Task<OperationResult<TruckCateDetailResponse>> GetCateById(int id)
        {
            var result = new OperationResult<TruckCateDetailResponse>();

            var entity = _unitOfWork.TruckCategoryRepository.Get(t => t.Id == id);

            var images = await _unitOfWork.TruckCategoryRepository.GetFirstTruckImagesByCategoryIdAsync(id);

            var response = _mapper.Map<TruckCateDetailResponse>(entity.FirstOrDefault());
            response.TruckImgs = images.Select(img => new TruckImgResponse
            {
                ImageUrl = img,
            }).ToList();

            result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetTruckSuccess, response);
            return result;
        }

        public async Task<OperationResult<bool>> DeleteTruckImg(int id)
        {
            var result = new OperationResult<bool>();
            try
            {
                var truclImg = await _unitOfWork.TruckImgRepository.GetByIdAsync(id);
                if (truclImg == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundTruckImg);
                    return result;
                }
                if (truclImg.IsDeleted == true)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.TruckImgIsDeleted);
                    return result;
                }
                truclImg.IsDeleted = true;

                _unitOfWork.TruckImgRepository.Update(truclImg);
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
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateTruckImg, response);

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
                result.AddResponseStatusCode(StatusCode.Created, MessageConstant.SuccessMessage.CreateTruckCategory, response);

                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<TruckCategoryResponse>> UpdateTruckCategory(int id, TruckCategoryRequest request)
        {
            var result = new OperationResult<TruckCategoryResponse>();
            try
            {
                var truckCategory = await _unitOfWork.TruckCategoryRepository.GetByIdAsync(id);
                if(truckCategory == null)
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
                   
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.TruckCategoryUpdateSuccess,
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



    }
}