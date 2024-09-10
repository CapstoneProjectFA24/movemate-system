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
    public interface IStaffDailyServices
    {
        public Task<OperationResult<List<StaffDailyResponse>>> GetAll(GetAllStaffDailyRequest request);
    }
}
