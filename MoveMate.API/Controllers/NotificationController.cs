using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly IFirebaseServices _firebaseServices;

        public NotificationController(IFirebaseServices firebaseServices)
        {
            _firebaseServices = firebaseServices;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            // Call the SendNotificationAsync method and get the result
            var result = await _firebaseServices.SendNotificationAsync(request.Token, request.Title, request.Body);

            // Check if the notification sending failed (assuming result is null or empty on failure)
            if (result.IsError)
            {
                // Return an error response
                return HandleErrorResponse(result.Errors);
            }

            // If the notification sending was successful, create the success response
            var response = new
            {
                message = new
                {
                    token = request.Token,
                    notification = new
                    {
                        title = request.Title,
                        body = request.Body
                    }
                }
            };

            // Return the success response with a 200 OK status
            return Ok(response);
        }


    }
}
