using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleServices _scheduleServices;

        public ScheduleController(IScheduleServices scheduleServices)
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

        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllSchedule request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _scheduleServices.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Retrieves a schedule by its ID.
        /// </summary>
        /// <param name="id">The ID of the schedule to retrieve.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /shcedule/1
        /// </remarks>
        /// <response code="200">Get Schedule by Id Success!</response>
        /// <response code="404">Schedule not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var response = await _scheduleServices.GetById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
