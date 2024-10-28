using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
using MoveMate.Service.Commons.Errors;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelRequests.Booking;
using MoveMate.Service.ViewModels.ModelResponses;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class BookingController : BaseController
    {
        private readonly IBookingServices _bookingServices;

        public BookingController(IBookingServices bookingServices)
        {
            _bookingServices = bookingServices;
        }

        /// <summary>
        /// 
        /// FEATURE: Get all bookings
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize]

        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllBookingRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _bookingServices.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        #region FEATURE: Register a new booking in the system.

        /// <summary>
        ///
        /// FEATURE: Register a new booking in the system.
        /// </summary>
        /// <returns>
        /// A response that contains details of the registered booking or error information in case of failure.
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST /api/v1/bookings/register-booking
        ///         {
        ///           "pickupAddress": "string",
        ///           "pickupPoint": "string",
        ///           "deliveryAddress": "string",
        ///           "deliveryPoint": "string",
        ///           "estimatedDistance": "3",
        ///           "houseTypeId": 1,
        ///           "note": "string",
        ///           "isReviewOnline": true,
        ///           "isRoundTrip": false,
        ///           "isManyItems": true,
        ///           "roomNumber": "3",
        ///           "floorsNumber": "3",
        ///           "bookingDetails": [
        ///             {
        ///               "serviceId": 12,
        ///               "quantity": 1
        ///             },
        ///             {
        ///               "serviceId": 2,
        ///               "quantity": 1
        ///             }
        ///           ],
        ///           "truckCategoryId": 4,
        ///           "bookingAt": "2024-10-30T05:26:28.452Z",
        ///           "resourceList": [
        ///             {
        ///               "type": "IMG",
        ///               "resourceUrl": "https://hoanghamobile.com/tin-tuc/wp-content/webp-express/webp-images/uploads/2024/03/anh-meme-hai.jpg.webp",
        ///               "resourceCode": "string"
        ///             }
        ///           ]
        ///         }
        /// </remarks>
        /// <response code="201">Add Booking Success!</response>
        /// <response code="400-v1">Filed is required.</response>
        /// <response code="400-v2">HouseType with id: {request.HouseTypeId} not found!.</response>
        /// <response code="400-v3">Add Booking Failed!.</response>
        /// <response code="400-v4">BookingAt is not null and whether the value is greater than or equal to the current tim.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Thrown if the request data is invalid or contains logic errors.</exception>
        /// <exception cref="Exception">Thrown in case of a system error.</exception>

        // Post - register booking
        [HttpPost("register-booking")]
        [Authorize]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> RegisterBooking(BookingRegisterRequest request)
        {
            
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value).ToString();

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Invalid user ID in token." });
            }

            var response = await _bookingServices.RegisterBooking(request, userId);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        #endregion

        

        

        /// <summary>
        ///
        /// FEATURE: valuation floor booking
        /// </summary>
        /// <returns></returns>
        ///

        // Post - valuation distance booking
        [HttpPost("valuation-booking")]
        [Authorize]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValuationBooking(BookingValuationRequest request)
        {
            var response = await _bookingServices.ValuationBooking(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: Get booking by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var response = await _bookingServices.GetById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: Set booking to Cancel by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("cancel-booking/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelBookingById(BookingCancelRequest request)
        {
            var response = await _bookingServices.CancelBooking(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


       

        /// <summary>
        /// TEST: Reviewer update booking by assignment ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/update-booking-by-assignmentId/{id}")]
        public async Task<IActionResult> UpdateServiceBookingByAssigmentId(int id, [FromBody] BookingServiceDetailsUpdateRequest request)
        {
            var response = await _bookingServices.UpdateBookingAsync(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// FEATURE: Reviewer update booking by booking ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/update-booking/{id}")]
        public async Task<IActionResult> UpdateServiceBooking(int id, [FromBody] BookingServiceDetailsUpdateRequest request)
        {
            var response = await _bookingServices.UpdateBookingByBookingIdAsync(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE: User confirm round trip  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("user/confirm-round-trip/{id}")]
        [Authorize]
        public async Task<IActionResult> UserConfirmRoundTrip(int id)
        {
            var response = await _bookingServices.UserConfirmRoundTrip(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: Reviewer change review at 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/review-at/{id}")]
        public async Task<IActionResult> ReviewerChangeReviewAt(int id ,[FromBody] ReviewAtRequest request)
        {
            var response = await _bookingServices.ReviewChangeReviewAt(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: User confirm 
        /// </summary>
        /// <remarks>
        /// This endpoint allows a user to update the status of an existing booking, based on the current status and specific conditions:
        /// - If the booking is in "WAITING" status, it can transition to "ASSIGNED."
        /// - If the booking is in "WAITING" and has not been reviewed online, it can transition to "DEPOSITING."
        /// - If the booking has been reviewed online, it can transition to "DEPOSITING" only if it is in "REVIEWED" status. 
        ///     In this case, an assignment status related to the booking is also updated from "SUGGESTED" to "REVIEWED."
        /// - If the booking is in "REVIEWED" status, it can transition to "COMING."
        ///
        /// The status update will be validated against these conditions. If any condition fails, an error message will be returned 
        /// indicating the reason for the rejection.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("user/confirm/{id}")]
        public async Task<IActionResult> UserConfirmChange(int id, [FromBody] StatusRequest request)
        {
            var response = await _bookingServices.UserConfirm(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}