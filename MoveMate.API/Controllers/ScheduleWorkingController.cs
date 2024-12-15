using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class ScheduleWorkingController : BaseController
    {
        private readonly IScheduleWorkingServices _scheduleWorking;

        public ScheduleWorkingController(IScheduleWorkingServices scheduleWorking)
        {
            _scheduleWorking = scheduleWorking;
        }

        /// <summary>
        /// 
        /// CHORE : Get all schedule working
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllScheduleWorking request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _scheduleWorking.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Retrieves a schedule working by its ID.
        /// </summary>
        /// <param name="id">The ID of the schedule working to retrieve.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /scheduleworking/1
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleWorkingById(int id)
        {
            var response = await _scheduleWorking.GetScheduleWorkingById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Set shedule's IsActive become false by schedule Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScheduleWorking(int id)
        {
            var response = await _scheduleWorking.DeleteScheduleWorking(id);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Creates a new truck entry based on the provided details.
        /// </summary>
        /// <param name="request">An object containing details for creating the truck, including category, model, and specifications.</param>
        /// <returns>An IActionResult containing the response indicating the success or failure of the operation.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/truckcategory/truck
        ///     {
        ///         "Name": "Morning Shift",
        ///         "Type": "Full-Time",
        ///         "StartDate": "2024-12-01",
        ///         "EndDate": "2024-12-31"
        ///     }
        /// 
        /// </remarks>
        /// <response code="201">Truck created successfully.</response>
        /// <response code="400">Bad request, invalid data provided, or user is not a driver.</response>
        /// <response code="404">Specified user or truck category was not found.</response>
        /// <response code="409">User already owns a truck in the specified category.</response>
        /// <response code="500">Internal server error occurred during processing.</response>
        [HttpPost()]
        public async Task<IActionResult> CreateScheduleWorking([FromBody] CreateScheduleWorkingRequest request)
        {
            var response = await _scheduleWorking.CreateScheduleWorking(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateScheduleWorking(int id, [FromBody] UpdateScheduleWorkingRequest request)
        {

            var response = await _scheduleWorking.UpdateScheduleWorking(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        [HttpPut("add-schedule")]
        public async Task<IActionResult> AddScheduleIntoGroup([FromBody] AddScheduleIntoGroup request)
        {
            var response = await _scheduleWorking.AddGroupIntoSchedule(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }
    }
}
