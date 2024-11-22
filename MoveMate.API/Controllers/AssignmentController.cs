using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using System.Security.Claims;
using MoveMate.Service.ViewModels.ModelRequests.Assignments;
using Microsoft.AspNetCore.Authorization;
using MoveMate.Service.Commons;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class AssignmentController : BaseController
    {
        private readonly IBookingServices _bookingServices;
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IBookingServices bookingServices, IAssignmentService assignmentService)
        {
            _bookingServices = bookingServices;
            _assignmentService = assignmentService;
        }

        /// <summary>
        /// FEATURE: Get all booking detail report
        /// </summary>
        /// <param name="request">The filter, pagination, and sorting options for retrieving booking detail reports.</param>
        /// <returns>
        /// Returns an HTTP response containing a list of booking detail reports, including pagination details, 
        /// or an error message if something goes wrong.
        /// </returns>
        /// <remarks>
        /// Example usage:
        /// GET /api/booking-detail-report?page=1&per_page=10&sortBy=BookingAt&order=desc
        /// - Filters can include properties defined in <see cref="GetAllBookingDetailReport"/>.
        /// - Pagination is determined by the `page` and `per_page` query parameters.
        /// - Sorting can be applied using `sortBy` and `order`.
        /// </remarks>
        [HttpGet("")]
        [Authorize]

        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllBookingDetailReport request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;
            
            var response = await _assignmentService.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: Assigned staff by assignment id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> AssignedStaff(int id)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);
            var response = await _bookingServices.AssignedLeader(userId, id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: Trigger Assigned auto by manual driver by booking id
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPatch("assign-auto-by-manual-driver/{bookingId}")]
        public async Task<IActionResult> AssignedAutoByManualDriver(int bookingId)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);
            var response = await _assignmentService.HandleAssignManualDriver(bookingId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: Trigger Assigned manual porter by booking id
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPatch("assign-auto-by-manual-porter/{bookingId}")]
        public async Task<IActionResult> AssignedAutoByManualPorter(int bookingId)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);
            var response = await _assignmentService.HandleAssignManualPorter(bookingId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: Assigned manual staff by booking id
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPatch("assign-manual-staff/{bookingId}")]
        public async Task<IActionResult> AssignedManualsTask(int bookingId, [FromBody] AssignedManualStaffRequest request)
        {
            var response = await _assignmentService.HandleAssignManualStaff(bookingId, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: Get available drivers by booking id
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpGet("available-driver/{bookingId}")]
        public async Task<IActionResult> GetAvailableDriver(int bookingId)
        {
            var response = await _assignmentService.GetAvailableDriversForBooking(bookingId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: Get available porters by booking id
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpGet("available-porter/{bookingId}")]
        public async Task<IActionResult> GetAvailablePorter(int bookingId)
        {
            var response = await _assignmentService.GetAvailablePortersForBooking(bookingId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
