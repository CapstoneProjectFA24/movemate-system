using MoveMate.Domain.Models;
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
    public Task<OperationResult<DriverInfoDTO>> GetAvailableDriversForBooking(int bookingId);
    public Task<OperationResult<DriverInfoDTO>> GetAvailablePortersForBooking(int bookingId);
    public Task<OperationResult<List<BookingDetailReport>>> GetAll(GetAllBookingDetailReport request);
    Task<OperationResult<BookingDetailWaitingResponse>> StaffReportFail(int assignmentId, FailReportRequest request);
    public Task<OperationResult<bool>> ReviewStaff(int userId, int assignmentId, ReviewStaffRequest request);
    Task<OperationResult<BookingResponse>> BonusStaff(int userId, int assignmentId, BonusStaffRequest request);
    Task<OperationResult<bool>> StaffCheckException(int userId, int bookingTrackerId, PorterCheckReportRequest request);
}