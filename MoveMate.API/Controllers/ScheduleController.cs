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

        /// <summary>
        /// CHORE : Creates a new group entry.
        /// </summary>
        /// <param name="request">The request payload containing group detail.</param>
        /// <returns>A response indicating success or failure of the operation.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/group/
        ///     {
        ///         "date": "12/05/2024",
        ///         "scheduleWorkingId": 1,
        ///         "groupId": 2
        ///     }
        /// </remarks>
        [HttpPost("")]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleRequest request)
        {
            var response = await _scheduleServices.CreateSchedule(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}