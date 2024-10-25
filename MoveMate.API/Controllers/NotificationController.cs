using Microsoft.AspNetCore.Mvc;
using MoveMate.API.Authorization;
using MoveMate.Service.Commons;
using MoveMate.Service.Exceptions;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly INotificationServices _notificationServices;

        public NotificationController(INotificationServices notificationServices)
        {
           _notificationServices = notificationServices;
        }

        #region Create new user device
        /// <summary>
        /// Create a new user device
        /// </summary>
        /// <param name="userDeviceRequest">An object contains the FCM token from firebase.</param>
        /// <returns>
        /// A success message about creating new user device
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         POST 
        ///         {
        ///             "FCMToken": "Example"
        ///         }
        /// </remarks>
        /// <response code="200">Create a new user device successfully.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="Exception">Throw Error about the system.</exception>
        [HttpPost("create-user-device")]
        public async Task<IActionResult> PostCreateUserDeviceAsync([FromBody] CreateNotificationRequest request)
        {

            IEnumerable<Claim> claims = Request.HttpContext.User.Claims;
            await _notificationServices.CreateUserDeviceAsync(request, claims);
            return Ok(new
            {
                Message = MessageConstant.SuccessMessage.Notification
            });
        }
        #endregion


        #region Delete user device
        /// <summary>
        /// Delete an existed user device
        /// </summary>
        /// <param name="userDeviceIdRequest">An object contains user device id.</param>
        /// <returns>
        /// A success message about deleting user device
        /// </returns>
        /// <remarks>
        ///     Sample request:
        ///
        ///         DELETE 
        ///         
        ///             UserDeviceId = 1
        ///         
        /// </remarks>
        /// <response code="200">Delete a user device successfully.</response>
        /// <response code="400">Some Error about request data and logic data.</response>
        /// <response code="404">Some Error about request data not found.</response>
        /// <response code="500">Some Error about the system.</response>
        /// <exception cref="Exception">Throw Error about the system.</exception>      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserDeviceAsync([FromRoute] UserDeviceIdRequest userDeviceIdRequest)
        {
            
            await _notificationServices.DeleteUserDeviceAsync(userDeviceIdRequest.Id);
            return Ok(new
            {
                Message = MessageConstant.SuccessMessage.DeletedUserDevice
            });
        }
        #endregion



    }
}
