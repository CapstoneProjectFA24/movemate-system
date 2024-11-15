using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using System.Security.Claims;

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
        /// CHORE: Assigned manual driver by booking id
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        [HttpPatch("assign-manual-driver/{bookingId}")]
        public async Task<IActionResult> AssignedManualDriver(int bookingId)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);
            var response = await _assignmentService.HandleAssignManualDriver(bookingId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
        
    }
}
