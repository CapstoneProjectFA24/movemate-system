using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class BookingTrackerController : BaseController
    {
        private readonly IBookingServices _bookingServices;

        public BookingTrackerController(IBookingServices bookingServices)
        {
            _bookingServices = bookingServices;
        }

        /// <summary>
        /// FEATURE: Manager confirm request and refund money back
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPut("refund/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> StaffConfirmRefundRequest(int bookingId, [FromBody] RefundRequest request)
        {
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { statusCode = 401, message = MessageConstant.FailMessage.UserIdInvalid, isError = true });
            }

            var userId = int.Parse(accountIdClaim.Value);
            var response = await _bookingServices.StaffConfirmRefundBooking(userId, bookingId, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: Manager get all booking has booking tracker WAITING
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllBookingException request)
        {          
            var response = await _bookingServices.GetBookingExcception(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

    }
}
