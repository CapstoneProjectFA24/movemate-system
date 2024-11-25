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
        /// FEATURE: Retrieves a list of bookings based on the provided filter criteria, including the ability to filter by status, user ID, and review statuses.
        /// </summary>
        /// <param name="request">The request containing filter parameters for booking statuses (such as HOLD, VALIDATION, etc.), user ID, and other query criteria.</param>
        /// <returns>
        /// A list of bookings that match the filter criteria. If an error occurs, an error response will be returned.
        /// </returns>
        /// <response code="200">Returns a list of bookings matching the filter criteria.</response>
        /// <response code="400">Returns an error response if the input parameters are invalid.</response>
        /// <response code="500">Returns an internal server error if an unexpected error occurs.</response>
        /// <remarks>
        /// The `Status` parameter can accept multiple status values separated by a period (e.g., "HOLD", "ASSIGNED", "PENDING"). The following transformations are applied:
        /// - If "Status" contains `HOLD`, the list will include bookings with statuses `ASSIGNED` and `PENDING` instead.
        /// - If "Status" contains `VALIDATION`, the list will include `WAITING`, `DEPOSITING`, and `REVIEWED`.
        /// - If "Status" contains `EVALUATING`, the list will include `REVIEWING`.
        /// - If "Status" contains `PROGRESSING`, the list will include `IN_PROGRESS`, `COMING`, and `PAUSED`.
        /// - If "Status" contains `DONE`, the list will include `COMPLETED`.
        /// - If "Status" contains `PAID`, the list will include `COMPLETED`.
        /// - If "Status" contains `NEW`, the list will include various statuses based on the review flag and whether the booking is online or not:
        ///     - If "IsReviewOnl" is `true`, the system will return bookings with statuses like `PENDING`, `ASSIGNED`, `REVIEWING`, `REVIEWED`, and `DEPOSITING`.
        ///     - If "IsReviewOnl" is `false`, the system will return bookings with statuses like `PENDING`, `ASSIGNED`, `WAITING`, and `DEPOSITING`.
        /// - If "Status" contains `ADVANCE`, the list will include different statuses based on whether the review flag is set:
        ///     - If "IsReviewOnl" is `true`, the system will return bookings with statuses like `IN_PROGRESS`, `COMING`, and `PAUSED`.
        ///     - If "IsReviewOnl" is `false`, the system will return bookings with statuses like `IN_PROGRESS`, `COMING`, `PAUSED`, `REVIEWING`, and `REVIEWED`.
        /// - If "Status" contains `COMPENSATION`, the list will include `REFUNDED`.
        /// - If "Status" contains `CANCELED`, the list will include `CANCEL`.
        /// </remarks>
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
        ///           "pickupAddress": "428/39-NHÀ-5 Đường Chiến Lược,Phường Bình Trị Đông A,Quận Bình Tân,Thành Phố Hồ Chí Minh",
        ///           "pickupPoint": "10.764632,106.599717",
        ///           "deliveryAddress": "KHU2 Tỉnh Lộ 932,Xã Phú Tâm,Huyện Châu Thành,Tỉnh Sóc Trăng",
        ///           "deliveryPoint": "9.699092,105.963458",
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
        ///               "resourceUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1732454517/movemate/images/xfsx9kwggqjik4dsklvx.jpg",
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
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value).ToString();

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Invalid user ID in token." });
            }

            var response = await _bookingServices.ValuationBooking(request, userId);

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
        public async Task<IActionResult> CancelBookingById(int id, BookingCancelRequest request)
        {
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { Message = MessageConstant.FailMessage.UserIdInvalid });
            }

            var userId = int.Parse(accountIdClaim.Value);
            var response = await _bookingServices.CancelBooking(id, userId, request);
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
        [HttpPatch("user/confirm-round-trip/{id}")]
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
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { Message = MessageConstant.FailMessage.UserIdInvalid });
            }

            var userId = int.Parse(accountIdClaim.Value);
            var response = await _bookingServices.UserConfirm(id, userId, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Updates the booking date and time for an existing booking.
        /// </summary>
        /// <remarks>
        /// This endpoint allows a user to update the scheduled time (`BookingAt`) of an existing booking based on the booking's current status and review settings:
        /// - If the booking is marked as reviewed online:
        ///   - The `BookingAt` can be updated if the booking status is one of the following: "PENDING," "ASSIGNED," "REVIEWING," or "REVIEWED."
        /// - If the booking is not reviewed online:
        ///   - The `BookingAt` can be updated if the booking status is one of the following: "PENDING," "ASSIGNED," "WAITING," or "DEPOSITING."
        /// - If the booking has been canceled or does not meet the criteria, an error message will be returned indicating that the update cannot proceed.
        /// </remarks>
        /// <param name="id">The ID of the booking to update.</param>
        /// <param name="request">The updated booking time in the `ChangeBookingAtRequest` model.</param>
        /// <returns>
        /// Returns an `IActionResult` containing the updated booking information if successful. If the booking is not found, invalid, or the update criteria are not met, an appropriate error response is returned.
        /// </returns>
        [HttpPut("user/change-bookingAt/{id}")]
        public async Task<IActionResult> UserChangeBookingAt(int id, [FromBody] ChangeBookingAtRequest request)
        {
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { Message = MessageConstant.FailMessage.UserIdInvalid });
            }

            var userId = int.Parse(accountIdClaim.Value);
            var response = await _bookingServices.UserChangeBooingAt(id,userId, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE: set Report booking by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("report-booking/{id}")]
        [Authorize]
        public async Task<IActionResult> ReportBookingById(int id, BookingCancelRequest request)
        {
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { Message = MessageConstant.FailMessage.UserIdInvalid });
            }

            var userId = int.Parse(accountIdClaim.Value);
            var response = await _bookingServices.CancelBooking(id, userId, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE: Staff confirm pay by cash
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPatch("{bookingId}")]
        [Authorize]
        public async Task<IActionResult> StaffConfirmPayByCash(int bookingId)
        {
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { statusCode = 401, message = MessageConstant.FailMessage.UserIdInvalid, isError = true });
            }

            var userId = int.Parse(accountIdClaim.Value);
            var response = await _bookingServices.StaffConfirmPayByCash(userId, bookingId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
        
        /// <summary>
        /// FEATURE: get old booking by bookingId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("old-booking/{id}")]
        [Authorize]
        public async Task<IActionResult> OldBookingById(int id)
        {
            var response = await _bookingServices.GetOldBookingById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE: Staff back booking to REVIEWING
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPatch("back/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> StaffBackToBooking(int bookingId)
        {
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { statusCode = 401, message = MessageConstant.FailMessage.UserIdInvalid, isError = true });
            }

            var userId = int.Parse(accountIdClaim.Value);
            var response = await _bookingServices.StaffBackToReview(userId, bookingId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}