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

        public Task<OperationResult<List<PromotionResponse>>> GetAllTruck(GetAllPromotionRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<PromotionResponse>> GetTruckById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<PromotionResponse>> UpdateTruck(int id, UpdatePromotionRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<PromotionResponse>> CreateTruck(CreatePromotionRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<bool>> DeleteTruck(int id)
        {
            throw new NotImplementedException();
        }
    }
}