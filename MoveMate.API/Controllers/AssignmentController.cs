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
        /// 
        /// FEATURE: Get all booking detail report
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("booking-detail-waiting")]
        [Authorize]

        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllBookingDetailReport request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;
            
            var response = await _assignmentService.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// FEATURE: Staff report fail booking details
        /// </summary>
        /// <param name="id">Booking Details Id</param>
        /// <returns></returns>
        [HttpPut("report-booking-detail/{id}")]
        public async Task<IActionResult> StaffReportBookingDetail(int id, [FromBody] FailReportRequest request)
        {


            var response = await _assignmentService.StaffReportFail(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE: Assigned staff by assignment id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> AssignedStaff(int id)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);
            var response = await _bookingServices.AssignedLeader(userId, id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
        
        /// <summary>
        /// CHORE: Trigger Assigned auto by manual driver by booking id
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
        /// CHORE: Trigger Assigned manual porter by booking id
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
        /// CHORE: Assigned manual staff by booking id
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
        /// CHORE: Get available drivers by booking id
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
        /// CHORE: Get available porters by booking id
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
