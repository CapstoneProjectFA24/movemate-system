using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;

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
    }
}
