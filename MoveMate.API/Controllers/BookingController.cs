using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;

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
        /// Get all bookings
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
        
        
        /// <summary>
        ///
        /// register booking
        /// </summary>
        /// <returns></returns>
        ///
        
        // Post - register booking
        [HttpPost("register-booking")]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterBooking(BookingRegisterRequest request)
        {
            //var response =  await _bookingServices.RegisterBooking(request);
            return Ok(request);
        }
    }
}
