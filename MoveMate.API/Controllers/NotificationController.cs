using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// CHORE : Marks a notification as read for the manager user.
        /// </summary>
        /// <param name="id">The ID of the notification to be marked as read.</param>
        /// <returns>Returns a response indicating the success or failure of the operation.</returns>
        /// <response code="200">Returns if the notification was successfully marked as read.</response>
        /// <response code="400">Returns if the user is not a manager or if other bad request errors occur.</response>
        /// <response code="404">Returns if the notification is not found.</response>
        /// <response code="500">Returns if a system error occurs.</response>c
        [HttpPut("{id}")]
        public async Task<IActionResult> ManagerReadNoti(int id)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);
            var response = await _notificationService.ManagerReadMail(userId, id);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// 
        /// CHORE: Get all notification 
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllNotificationRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _notificationService.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// 
        /// CHORE: Get all notification 
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("report")]
        public async Task<IActionResult> GetAllReport([FromQuery] GetAllBookingTrackerReport request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _notificationService.GetAllBookingTracker(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}