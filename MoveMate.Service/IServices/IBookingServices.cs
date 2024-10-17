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
    public interface IBookingServices
    {
        public Task<OperationResult<List<BookingResponse>>> GetAll(GetAllBookingRequest request);
        public Task<OperationResult<BookingResponse>> GetById(int id);
        Task<OperationResult<BookingResponse>> RegisterBooking(BookingRegisterRequest request, string userId);
        Task<OperationResult<BookingValuationResponse>> ValuationDistanceBooking(BookingValuationRequest request);
        Task<OperationResult<BookingValuationResponse>> ValuationFloorBooking(BookingValuationRequest request);

        Task<OperationResult<BookingValuationResponse>> ValuationBooking(BookingValuationRequest request);

        Task<OperationResult<BookingResponse>> CancelBooking(BookingCancelRequest id);
        Task<OperationResult<BookingDetailsResponse>> DriverUpdateStatusBooking(int bookingId);
        Task<OperationResult<BookingDetailsResponse>> ReportFail(int bookingId);
        Task<OperationResult<BookingDetailsResponse>> DriverUpdateRoundTripBooking(int bookingId);
        Task<OperationResult<BookingDetailsResponse>> ReviewerOnlineUpdateStatusBooking(int bookingId);
        Task<OperationResult<BookingDetailsResponse>> ReviewerOfflineUpdateStatusBooking(int bookingId);
        Task<OperationResult<BookingDetailsResponse>> ReviewerCancelBooking(int bookingId);

        Task<OperationResult<BookingDetailsResponse>> ReviewerCompletedBooking(int bookingId);
        Task<OperationResult<BookingDetailsResponse>> PorterUpdateStatusBooking(int bookingId);
        Task<OperationResult<BookingDetailsResponse>> PorterRoundTripBooking(int bookingId);
        Task<OperationResult<BookingResponse>> UserConfirmRoundTrip(int bookingId);

    }
}