using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;

namespace MoveMate.Service.IServices
{
    public interface ITruckServices
    {
        

        Task<OperationResult<List<TruckCategoryResponse>>> GetAllTruckCategory(GetAllTruckCategoryRequest request);
        // public Task<OperationResult<TruckCateDetailResponse>> GetCateById(int id);
        public Task<OperationResult<TruckCategoryResponse>> GetTruckCategoryById(int id);
        public Task<OperationResult<TruckImageResponse>> CreateTruckImg(CreateTruckImgRequest request);

        public Task<OperationResult<bool>> DeleteTruckImg(int id);
        public Task<OperationResult<bool>> DeleteTruckCategory(int id);

        public Task<OperationResult<TruckCategoryResponse>> UpdateTruckCategory (int id, TruckCategoryRequest request);
        Task<OperationResult<TruckCategoryResponse>> CreateTruckCategory(TruckCategoryRequest request);

        public Task<OperationResult<List<TruckResponse>>> GetAllTruck(GetAllTruckRequest request);
        public Task<OperationResult<TruckResponse>> GetTruckById(int id);
        Task<OperationResult<TruckResponse>> UpdateTruck(int id, UpdateTruckRequest request);
        Task<OperationResult<TruckResponse>> CreateTruck(CreateTruckRequest request);
        Task<OperationResult<bool>> DeleteTruck(int id);
    }
}