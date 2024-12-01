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
        public Task<OperationResult<List<PromotionResponse>>> GetAllPromotion(GetAllPromotionRequest request);
        public Task<OperationResult<PromotionResponse>> GetPromotionById(int id);
        Task<OperationResult<PromotionResponse>> UpdatePromotion(int id, UpdatePromotionRequest request);
        Task<OperationResult<PromotionResponse>> CreatePromotion(CreatePromotionRequest request);
        Task<OperationResult<bool>> DeletePromotion(int id);

        Task<OperationResult<GetAllPromotionResponse>> GetListPromotion(int userId);
    }
}