using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Firebase;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class CheckHealthController : BaseController
    {
        private readonly IFirebaseServices _firebaseServices;
        private readonly IBookingServices _bookingServices;

        public CheckHealthController(IFirebaseServices firebaseServices, IBookingServices bookingServices)
        {
            _firebaseServices = firebaseServices;
            _bookingServices = bookingServices;
        }
        
        /// <summary>
        /// TEST: CheckHealth system
        /// </summary>
        /// <returns>Hello, i am healthy!.</returns>
        [HttpGet("")]
        public async Task<IActionResult> CheckHealth()
        {
            return Ok("Hello, i am healthy!.");
        }

        /// <summary>
        /// TEST: Push Booking to Firebase
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPost("save-to-firebase/{bookingId}")]
        public async Task<IActionResult> TriggerUpBookingFirebase(int bookingId, bool saveOldBooking = false)
        {
            var booking = _bookingServices.GetBookingByIdAsync(bookingId);
            _firebaseServices.SaveBooking(booking, bookingId, "bookings");
            if (saveOldBooking)
            {
                _firebaseServices.SaveBooking(booking, bookingId, "old_bookings");
            }
            
            return Ok();
        }

    }
}
