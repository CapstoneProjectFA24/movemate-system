using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelRequests.Booking;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;

namespace MoveMate.Service.IServices
{
    public interface IBookingServices
    {
        public Task<OperationResult<List<BookingResponse>>> GetAll(GetAllBookingRequest request);
        public Task<OperationResult<BookingResponse>> GetById(int id);
        Task<OperationResult<BookingResponse>> RegisterBooking(BookingRegisterRequest request, string userId);     

        Task<OperationResult<BookingValuationResponse>> ValuationBooking(BookingValuationRequest request, string userId);

        Task<OperationResult<BookingResponse>> CancelBooking(int id, int userId, BookingCancelRequest request);
        Task<OperationResult<AssignmentResponse>> DriverUpdateStatusBooking(int userId, int bookingId, TrackerSourceRequest request);
        Task<OperationResult<AssignmentResponse>> ReportFail(int bookingId, string failedReason);
        Task<OperationResult<AssignmentResponse>> DriverUpdateRoundTripBooking(int bookingId);      
        Task<OperationResult<AssignmentResponse>> ReviewerUpdateStatusBooking(int userId, int bookingId,TrackerByReviewOfflineRequest request);
        Task<OperationResult<AssignmentResponse>> ReviewerCancelBooking(int bookingId);
        Task<OperationResult<AssignmentResponse>> ReviewerCompletedBooking(int bookingId);
        Task<OperationResult<AssignmentResponse>> PorterUpdateStatusBooking(int userId, int bookingId, TrackerSourceRequest request);
        Task<OperationResult<AssignmentResponse>> PorterRoundTripBooking(int bookingId, ResourceRequest request);
        Task<OperationResult<BookingResponse>> UserConfirmRoundTrip(int bookingId);

        Task<OperationResult<BookingResponse>> UserConfirm(int bookingId,int userId, StatusRequest request);
        Task<OperationResult<BookingResponse>> ReviewChangeReviewAt(int bookingId, ReviewAtRequest request);
        Task<OperationResult<BookingResponse>> UpdateBookingAsync(int assignmentId, BookingServiceDetailsUpdateRequest request, bool isDriverUpdate = false);
        Task<OperationResult<BookingResponse>> UpdateBookingByBookingIdAsync(int id, BookingServiceDetailsUpdateRequest request);
        Task<OperationResult<AssignmentResponse>> AssignedLeader(int userId, int assignmentId);
        Task<OperationResult<BookingResponse>> UserChangeBooingAt(int booingId, int userId, ChangeBookingAtRequest request);
        
        Task<OperationResult<BookingDetailsResponse>> ManagerFix(int bookingDetailId, int userId);
        Task<OperationResult<BookingResponse>> TrackerReport(int userId, int bookingId, TrackerSourceRequest request);
        Task<OperationResult<BookingResponse>> DriverUpdateBooking(int userId,int bookingId, DriverUpdateBookingRequest request);
        Task<OperationResult<BookingResponse>> PorterUpdateBooking(int userId, int bookingId, PorterUpdateDriverRequest request);
    }
}