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
        /// 
        /// Get all schedule
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]

        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllSchedule request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _scheduleServices.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// Get schedule by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var response = await _scheduleServices.GetById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
