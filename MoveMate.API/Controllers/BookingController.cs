﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
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
        ///           "serviceDetails": [
        ///             {
        ///               "serviceId": 52,
        ///               "quantity": 1
        ///             },
        ///             {
        ///               "serviceId": 35,
        ///               "quantity": 1
        ///             }
        ///           ],
        ///           "truckCategoryId": 1,
        ///           "bookingAt": "2024-10-16T05:26:28.452Z",
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
        /// TEST: valuation distance booking, test by vinh
        /// </summary>
        /// <returns></returns>
        ///

        // Post - valuation distance booking
        [HttpPost("valuation-distance-booking")]
        [Authorize]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValuationDistanceBooking(BookingValuationRequest request)
        {
            var response = await _bookingServices.ValuationDistanceBooking(request);


            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        ///
        /// TEST: valuation floor booking, test by vinh
        /// </summary>
        /// <returns></returns>
        ///

        // Post - valuation distance booking
        [HttpPost("valuation-floor-booking")]
        [Authorize]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValuationFloorBooking(BookingValuationRequest request)
        {
            var response = await _bookingServices.ValuationFloorBooking(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        ///
        /// FEATURE: valuation floor booking, dev by vinh
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
        /// TEST: User Update booking information
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/update-information-booking/{id}")]
        public async Task<IActionResult> UpdateInformationBooking(int id, [FromBody] BookingBasicInfoUpdateRequest request)
        {
            var response = await _bookingServices.UpdateBasicInfoAsync(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }



        /// <summary>
        /// TEST: User Update Fee Detail of booking
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/update-fee-booking/{id}")]
        public async Task<IActionResult> UpdateFeeBooking(int id, [FromBody] BookingFeeDetailsUpdateRequest request)
        {
            var response = await _bookingServices.UpdateFeeDetailsAsync(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// TEST: User Update Service Detail of booking 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("reviewer/update-service-booking/{id}")]
        public async Task<IActionResult> UpdateServiceBooking(int id, [FromBody] BookingServiceDetailsUpdateRequest request)
        {
            var response = await _bookingServices.UpdateBookingAsync(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// TEST: User confirm round trip  
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
    }
}