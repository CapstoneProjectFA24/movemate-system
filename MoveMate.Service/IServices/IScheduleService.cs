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
    public interface IScheduleService
    {
        public Task<OperationResult<List<ScheduleDailyResponse>>> GetAll(GetAllScheduleDailyRequest request);
        public Task<OperationResult<ScheduleDailyResponse>> CreateSchedule(ScheduleRequest request);
    }
}
