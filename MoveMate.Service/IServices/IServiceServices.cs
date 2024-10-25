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
    public interface IServiceServices
    {
        public Task<OperationResult<List<ServicesResponse>>> GetAll(GetAllServiceRequest request);

        public Task<OperationResult<List<ServicesResponse>>> GetAllNotTruck(GetAllServiceNotTruckRequest request);

        public Task<OperationResult<List<ServiceResponse>>> GetAllServiceTruck(GetAllServiceTruckType request);
        public Task<OperationResult<ServicesResponse>> GetById(int id);

        Task<OperationResult<ServicesResponse>> CreateService(CreateServiceRequest request);
        Task<OperationResult<bool>> DeleteService(int id);
    }
}