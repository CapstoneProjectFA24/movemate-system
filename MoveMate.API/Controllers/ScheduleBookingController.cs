using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class ScheduleBookingController : BaseController
    {
        private readonly IScheduleBooingServices _scheduleBooingServices;

        public ScheduleBookingController(IScheduleBooingServices scheduleBooingServices)
        {
            _scheduleBooingServices = scheduleBooingServices;
        }

        /// <summary>
        /// CHORE : Retrieves a paginated list of all schedule booking.
        /// </summary>
        /// <param name="request">The request containing pagination and filter parameters.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Get List Schedule Booking Category Done</response>
        /// <response code="200">List Schedule Booking is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("")]
        public async Task<IActionResult> GetAllScheduleBooking([FromQuery] GetAllScheduleBookingRequest request)
        {
            var response = await _scheduleBooingServices.GetAllScheduleBooking(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
        
        
        
         /// <summary>
         /// CHORE : Retrieves a schedule booking by its ID.
         /// </summary>
         /// <param name="id">The ID of the schedule booking to retrieve.</param>
         /// <returns>An IActionResult containing the operation result.</returns>
         /// <remarks>
         /// Sample request:
         /// 
         ///     GET /truckcategory/1
         /// </remarks>
         /// <response code="200">Get schedule booking success</response>
         /// <response code="404">Schedule booking not found</response>
         /// <response code="500">Internal server error occurred</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleBoookingById(int id)
        {
            var response = await _scheduleBooingServices.GetScheduleBookingById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Set schedule booking's IsActived become false by schedule booking Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScheduleBookingById(int id)
        {
            var response = await _scheduleBooingServices.DeleteScheduleBooking(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
        /// <summary>
        /// Creates a new schedule booking entry.
        /// </summary>
        /// <param name="request">The request payload containing schedule booking details.</param>
        /// <returns>A response indicating success or failure of the operation.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/schedule/manage/schedule-booking
        ///     {
        ///         "Shard": "UniqueShardIdentifier",
        ///         "IsActived": true
        ///     }
        /// </remarks>
        /// <response code="201">Schedule booking created successfully.</response>
        /// <response code="400">Bad request, invalid data.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("schedule-booking")]
        public async Task<IActionResult> CreateScheduleBooking([FromBody] CreateScheduleBookingRequest request)
        {
            var response = await _scheduleBooingServices.CreateScheduleBooking(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// Updates an existing schedule booking by ID.
        /// </summary>
        /// <param name="id">The ID of the schedule booking to update.</param>
        /// <param name="request">The request payload containing updated schedule booking details.</param>
        /// <returns>A response indicating success or failure of the operation.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/schedule/manage/schedule-booking/{id}
        ///     {
        ///         "Shard": "UpdatedShardIdentifier"
        ///     }
        /// </remarks>
        /// <response code="200">Schedule booking updated successfully.</response>
        /// <response code="400">Bad request, invalid data.</response>
        /// <response code="404">Schedule booking not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("schedule-booking/{id}")]
        public async Task<IActionResult> UpdateScheduleBooking(int id, [FromBody] UpdateScheduleBookingRequest request)
        {
            var response = await _scheduleBooingServices.UpdateScheduleBooking(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
