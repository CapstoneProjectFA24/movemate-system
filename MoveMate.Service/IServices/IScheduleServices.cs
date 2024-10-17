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
    public interface IScheduleServices
    {
        public Task<OperationResult<List<ScheduleResponse>>> GetAll(GetAllSchedule request);
        public Task<OperationResult<ScheduleResponse>> GetById(int id);
    }
}