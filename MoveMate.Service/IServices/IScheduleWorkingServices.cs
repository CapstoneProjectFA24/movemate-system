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
    public interface IScheduleWorkingServices
    {
        public Task<OperationResult<List<ScheduleWorkingResponse>>> GetAllScheduleWorking(GetAllScheduleWorking request);
        public Task<OperationResult<ScheduleWorkingResponse>> GetScheduleWorkingById(int id);
        Task<OperationResult<ScheduleWorkingResponse>> UpdateScheduleWorking(int id, UpdateScheduleWorkingRequest request);
        Task<OperationResult<ScheduleWorkingResponse>> CreateScheduleWorking(CreateScheduleWorkingRequest request);
        Task<OperationResult<bool>> DeleteScheduleWorking(int id);
    }
}
