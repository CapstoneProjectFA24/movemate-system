using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService _scheduleServices;

        public ScheduleController(IScheduleService scheduleServices)
        {
            _scheduleServices = scheduleServices;
        }

        /// <summary>
        /// CHORE : Retrieves a paginated list of all schedule.
        /// </summary>
        /// <param name="request">The request containing pagination and filter parameters.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Get List Schedule Done</response>
        /// <response code="200">List Schedule is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("")]
        [Authorize]

        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllScheduleDailyRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _scheduleServices.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        
    }
}