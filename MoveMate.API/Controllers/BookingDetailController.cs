using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;

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
        /// TEST: Driver update status booking details happy case 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("driver/update-status/{id}")]
        public async Task<IActionResult> DriverUpdateStatusBooking(int id)
        {
            var response = await _bookingServices.DriverUpdateStatusBooking(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// TEST: Driver update status booking details without round trip at begin 
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
        /// TEST: Porter update status booking details happy case 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("porter/update-status/{id}")]
        public async Task<IActionResult> PorterUpdateStatusBooking(int id)
        {
            var response = await _bookingServices.PorterUpdateStatusBooking(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// TEST: Porter update status booking details happy case 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("porter/update-round-trip/{id}")]
        public async Task<IActionResult> PorterUpdateRoundTripBooking(int id)
        {
            var response = await _bookingServices.PorterRoundTripBooking(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// TEST: Report when driver cannot arrived  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("report-fail/{id}")]
        public async Task<IActionResult> ReportFail(int id)
        {
            var response = await _bookingServices.ReportFail(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// TEST: Reviewer update status booking details by review offline  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/review-offline/update-status/{id}")]
        public async Task<IActionResult> ReviewerOfflineUpdateStatus(int id)
        {
            var response = await _bookingServices.ReviewerOfflineUpdateStatusBooking(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// TEST: Reviewer update status booking details by review offline  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/review-online/update-status/{id}")]
        public async Task<IActionResult> ReviewerOnlineUpdateStatus(int id)
        {
            var response = await _bookingServices.ReviewerOnlineUpdateStatusBooking(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// TEST: Reviewer update status booking details when booking completely reasonable  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/update-status/{id}")]
        public async Task<IActionResult> ReviewerUpdateCompletedBooking(int id)
        {
            var response = await _bookingServices.ReviewerCompletedBooking(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// TEST: Reviewer cancel booking details
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
