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
    public interface IFeeSettingService
    {
        public Task<OperationResult<List<PromotionResponse>>> GetAllFeeSetting(GetAllPromotionRequest request);
        public Task<OperationResult<PromotionResponse>> GetFeeSettingById(int id);
        Task<OperationResult<PromotionResponse>> UpdateFeeSetting(int id, UpdatePromotionRequest request);
        Task<OperationResult<PromotionResponse>> CreateFeeSetting(CreatePromotionRequest request);
        Task<OperationResult<bool>> DeleteFeeSetting(int id);
    }
}
