using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface IHouseTypeServices
    {
        public Task<OperationResult<List<HouseTypeResponse>>> GetAll(GetAllHouseTypeRequest request);
        public Task<OperationResult<HouseTypesResponse>> GetById(int id);
    }
}