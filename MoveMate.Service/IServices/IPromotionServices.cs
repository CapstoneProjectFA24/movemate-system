using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface IPromotionServices
    {
        public Task<OperationResult<List<PromotionResponse>>> GetAllTruck(GetAllPromotionRequest request);
        public Task<OperationResult<PromotionResponse>> GetTruckById(int id);
        Task<OperationResult<PromotionResponse>> UpdateTruck(int id, UpdatePromotionRequest request);
        Task<OperationResult<PromotionResponse>> CreateTruck(CreatePromotionRequest request);
        Task<OperationResult<bool>> DeleteTruck(int id);
    }
}