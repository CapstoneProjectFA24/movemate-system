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
        /// TEST: Push Booking to Firebase
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPost("{bookingId}")]
        public async Task<IActionResult> TriggerUpBookingFirebase(int bookingId)
        {
            var booking = _bookingServices.GetBookingByIdAsync(bookingId);
            _firebaseServices.SaveBooking(booking, bookingId, "bookings");
            return Ok();
        }

    }
}
