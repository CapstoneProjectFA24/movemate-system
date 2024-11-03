using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelRequests.Booking;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface IBookingServices
    {
        public Task<OperationResult<List<BookingResponse>>> GetAll(GetAllBookingRequest request);
        public Task<OperationResult<BookingResponse>> GetById(int id);
        Task<OperationResult<BookingResponse>> RegisterBooking(BookingRegisterRequest request, string userId);     

        Task<OperationResult<BookingValuationResponse>> ValuationBooking(BookingValuationRequest request, string userId);

        Task<OperationResult<BookingResponse>> CancelBooking(BookingCancelRequest id);
        Task<OperationResult<AssignmentResponse>> DriverUpdateStatusBooking(int bookingId);
        Task<OperationResult<AssignmentResponse>> ReportFail(int bookingId, string failedReason);
        Task<OperationResult<AssignmentResponse>> DriverUpdateRoundTripBooking(int bookingId);      
        Task<OperationResult<AssignmentResponse>> ReviewerUpdateStatusBooking(int bookingId,TrackerByReviewOfflineRequest request);
        Task<OperationResult<AssignmentResponse>> ReviewerCancelBooking(int bookingId);
        Task<OperationResult<AssignmentResponse>> ReviewerCompletedBooking(int bookingId);
        Task<OperationResult<AssignmentResponse>> PorterUpdateStatusBooking(int bookingId, ResourceRequest request);
        Task<OperationResult<AssignmentResponse>> PorterRoundTripBooking(int bookingId, ResourceRequest request);
        Task<OperationResult<BookingResponse>> UserConfirmRoundTrip(int bookingId);

        Task<OperationResult<BookingResponse>> UserConfirm(int bookingId, StatusRequest request);
        Task<OperationResult<BookingResponse>> ReviewChangeReviewAt(int bookingId, ReviewAtRequest request);
        Task<OperationResult<BookingResponse>> UpdateBookingAsync(int assignmentId, BookingServiceDetailsUpdateRequest request);
        Task<OperationResult<BookingResponse>> UpdateBookingByBookingIdAsync(int id, BookingServiceDetailsUpdateRequest request);
        Task<OperationResult<AssignmentResponse>> AssignedLeader(int assignmentId);
    }
}