using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class AssignmentController : BaseController
    {
        private readonly IBookingServices _bookingServices;

        public AssignmentController(IBookingServices bookingServices)
        {
            _bookingServices = bookingServices;
        }

        /// <summary>
        /// CHORE: Assigned staff by assignment id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> AssignedStaff(int id)
        {
            var response = await _bookingServices.AssignedLeader(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
