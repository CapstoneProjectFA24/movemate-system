using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelRequests.Assignments;
using MoveMate.Service.ViewModels.ModelResponses;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;

namespace MoveMate.Service.IServices;

public interface IAssignmentService
{
    public Task<OperationResult<AssignManualDriverResponse>> HandleAssignManualDriver(int bookingId);
    public Task<OperationResult<AssignManualDriverResponse>> HandleAssignManualPorter(int bookingId);
    public Task<OperationResult<BookingResponse>> HandleAssignManualStaff(int bookingId,
        AssignedManualStaffRequest request);
    public Task<OperationResult<List<BookingDetailReport>>> GetAll(GetAllBookingDetailReport request);

}