using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;

namespace MoveMate.Service.IServices;

public interface IAssignmentService
{
    public Task<OperationResult<AssignManualDriverResponse>> HandleAssignManualDriver(int bookingId);

}