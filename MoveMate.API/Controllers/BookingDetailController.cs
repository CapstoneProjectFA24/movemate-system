using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    public class BookingDetailController : BaseController
    {
        private readonly IBookingServices _bookingServices;

        public BookingDetailController(IBookingServices bookingServices)
        {
            _bookingServices = bookingServices;
        }



        /// <summary>
        /// CHORE: Driver update status booking details happy case 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("driver/update-status/{id}")]
        public async Task<IActionResult> DriverUpdateStatusBooking(int id, [FromBody] TrackerSourceRequest request)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);
            var response = await _bookingServices.DriverUpdateStatusBooking(userId, id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE: Driver update status booking details without round trip at begin 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("driver/update-round-trip/{id}")]
        public async Task<IActionResult> DriverUpdateRoundTripBooking(int id)
        {
            var response = await _bookingServices.DriverUpdateRoundTripBooking(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE: Porter update status booking details happy case 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("porter/update-status/{id}")]
        public async Task<IActionResult> PorterUpdateStatusBooking(int id, [FromBody] ResourceRequest request)
        {
            var response = await _bookingServices.PorterUpdateStatusBooking(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE: Porter update status booking details happy case 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("porter/update-round-trip/{id}")]
        public async Task<IActionResult> PorterUpdateRoundTripBooking(int id, [FromBody] ResourceRequest request)
        {
            var response = await _bookingServices.PorterRoundTripBooking(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE: Report when driver cannot arrived  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("report-fail/{id}")]
        public async Task<IActionResult> ReportFail(int id, [FromBody] string failedReason)
        {
            var response = await _bookingServices.ReportFail(id, failedReason);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// FEATURE: Reviewer update status booking details by review offline  
        /// </summary>
        /// <remarks>
        /// This endpoint allows a reviewer to progress through the booking review process based on current assignment and booking statuses. The status transitions vary based on the existing state:
        /// - If the current assignment status is "ASSIGNED" and the booking is online, the booking status changes to "REVIEWING"; 
        ///     if offline, the assignment status advances to "ENROUTE."
        /// - If the assignment is "ENROUTE," it progresses to "ARRIVED."
        /// - If the assignment is "SUGGESTED" and the booking is in "REVIEWING" status, a new tracker is created for offline resources, 
        ///     updating the assignment to "REVIEWED" and the booking to "REVIEWED". EstimatedDeliveryTime must be update right here if it wasn't 
        ///     updated before
        ///
        /// If the conditions for a transition are not met, an error message is returned.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/update-status/{id}")]
        public async Task<IActionResult> ReviewerUpdateStatus(int id , [FromBody] TrackerByReviewOfflineRequest request)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);

            var response = await _bookingServices.ReviewerUpdateStatusBooking(userId, id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }



        /// <summary>
        /// CHORE: Reviewer update status booking details when booking completely reasonable  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/complete-booking/{id}")]
        public async Task<IActionResult> ReviewerUpdateCompletedBooking(int id)
        {
            var response = await _bookingServices.ReviewerCompletedBooking(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE: Reviewer cancel booking details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/cancel-booking/{id}")]
        public async Task<IActionResult> ReviewerCancelBooking(int id)
        {
            var response = await _bookingServices.ReviewerCancelBooking(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
