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
    public interface IScheduleBooingServices
    {
        public Task<OperationResult<List<ScheduleBookingResponse>>> GetAllScheduleBooking(GetAllScheduleBookingRequest request);
        public Task<OperationResult<ScheduleBookingResponse>> GetScheduleBookingById(int id);
        Task<OperationResult<ScheduleBookingResponse>> UpdateScheduleBooking(int id, UpdateScheduleBookingRequest request);
        Task<OperationResult<ScheduleBookingResponse>> CreateScheduleBooking(CreateScheduleBookingRequest request);
        Task<OperationResult<bool>> DeleteScheduleBooking(int id);
    }
}
