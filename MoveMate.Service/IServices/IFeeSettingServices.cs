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
    public interface IFeeSettingServices
    {
        public Task<OperationResult<List<GetFeeSettingResponse>>> GetAll(GetAllFeeSetting request);
        public Task<OperationResult<GetFeeSettingResponse>> GetFeeSettingById(int id);
        Task<OperationResult<GetFeeSettingResponse>> UpdateFeeSetting(int id, UpdatePromotionRequest request);
        Task<OperationResult<GetFeeSettingResponse>> CreateFeeSetting(CreatePromotionRequest request);
        Task<OperationResult<bool>> DeleteActiveFeeSetting(int id);
    }
}