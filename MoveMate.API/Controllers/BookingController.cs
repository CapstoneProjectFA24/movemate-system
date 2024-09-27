using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;
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
        [HttpGet("get-all")]
        
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
        ///           "estimatedDeliveryTime": "3",
        ///           "isRoundTrip": true,
        ///           "isManyItems": true,
        ///           "roomNumber": "1",
        ///           "floorsNumber": "2",
        ///           "serviceDetails": [
        ///             {
        ///               "id": 1,
        ///               "isQuantity": true,
        ///               "quantity": 1
        ///             }
        ///           ],
        ///           "truckCategoryId": 1,
        ///           "bookingAt": "2024-09-27T04:05:29.705Z",
        ///           "resourceList": [
        ///             {
        ///               "type": "IMG",
        ///               "resourceUrl": "https://hoanghamobile.com/tin-tuc/wp-content/webp-express/webp-images/uploads/2024/03/anh-meme-hai.jpg.webp",
        ///               "resourceCode": "string"
        ///             }
        ///           ]
        ///         }
        /// </remarks>
        /// <response code="200">Booking registered successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="BadRequestException">Thrown if the request data is invalid or contains logic errors.</exception>
        /// <exception cref="Exception">Thrown in case of a system error.</exception>
        
        // Post - register booking
        [HttpPost("register-booking")]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> RegisterBooking(BookingRegisterRequest request)
        {
            var response =  await _bookingServices.RegisterBooking(request);
            
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
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValuationDistanceBooking(BookingValuationRequest request)
        {
            var response =  await _bookingServices.ValuationDistanceBooking(request);
            
            
            
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
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValuationFloorBooking(BookingValuationRequest request)
        {
            var response =  await _bookingServices.ValuationFloorBooking(request);
            
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
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValuationBooking(BookingValuationRequest request)
        {
            var response =  await _bookingServices.ValuationBooking(request);
            
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }
        
    }
}
