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
            BackgroundJob.Schedule(()=> Console.WriteLine("delay---"), TimeSpan.FromMinutes(1));
            
            var result = new OperationResult<List<TruckCateResponse>>();
            
            var response = _unitOfWork.TruckCategoryRepository.GetWithPagination();
            
            var listResponse = _mapper.Map<List<TruckCateResponse>>(response.Results);
            
            var pagin = response.Pagination;
            
            if (listResponse == null || !listResponse.Any())
            {
                
                result.AddResponseStatusCode(StatusCode.Ok, "List TruckCate is Empty!", listResponse, pagin);
                return result;
            }
            
            result.AddResponseStatusCode(StatusCode.Ok, "Get List Truck Category Done", listResponse, pagin);
            
            return result;
        }

        public async  Task<OperationResult<TruckCateDetailResponse>> GetCateById(int id)
        {
            var result = new OperationResult<TruckCateDetailResponse>();
            
            var entity = _unitOfWork.TruckCategoryRepository.Get(t => t.Id == id);
            
            var images  = await  _unitOfWork.TruckCategoryRepository.GetFirstTruckImagesByCategoryIdAsync(id);
            
            var response = _mapper.Map<TruckCateDetailResponse>(entity.FirstOrDefault());
            response.TruckImgs = images.Select(img => new TruckImgResponse
            {
                ImageUrl = img,
            }).ToList();
           
            result.AddResponseStatusCode(StatusCode.Ok, "Get Truck Category Detail Done", response);
            return result;
        }
    }
}