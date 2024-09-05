using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]

    public class UserController : BaseController
    {
        private readonly IUserServices _userService;

        public UserController(IUserServices userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 
        /// Get all users
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/get-all")]
        [Authorize(Roles = "1")]
        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllUserRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _userService.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
