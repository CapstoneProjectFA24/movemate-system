using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.API.Middleware;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    [ApiController]

    public class UserController : BaseController
    {
        private readonly IUserServices _userService;

        private readonly IFirebaseMiddleware _firebaseMiddleware;

        public UserController(IUserServices userService, IFirebaseMiddleware firebaseMiddleware)
        {
            _firebaseMiddleware = firebaseMiddleware;
            _userService = userService;
        }

        /// <summary>
        /// CHORE : Retrieves a paginated list of all users
        /// </summary>
        /// <param name="request">The request containing pagination and filter parameters.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Get List User Done</response>
        /// <response code="200">List User is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("")]
        [Authorize(Roles = "1")]
        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllUserRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _userService.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }



        /// <summary>
        /// CHORE : Retrieves a user info by token
        /// </summary>
        /// <param name="id">The ID of the house type to retrieve.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /userinfo/
        /// </remarks>
        /// <response code="200">User information retrieved successfully</response>
        /// <response code="404">User info not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("info")]
        [Authorize]
        public async Task<IActionResult> GetAddressByUserIdAsync()
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value).ToString();

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Invalid user ID in token." });
            }

            var result = await _userService.GetUserInfoByUserIdAsync(userId);
            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            return Ok(result);
        }




        /// <summary>
        /// CHORE : Update User by token
        /// </summary>
        /// <param name="updateUserRequest">Update account information </param>
        /// <returns>Returns the result of the user info</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "name": "string",
        ///         "password": "string",
        ///         "email": "hehe@gmail.com",
        ///         "phone": 0978635422
        ///     }   
        /// </remarks>    
        /// <response code="200">User updated successfully</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult> UpdateUserAsync([FromBody] UpdateUserRequest updateUserRequest)
        {
            try
            {
                IEnumerable<Claim> claims = HttpContext.User.Claims;
                Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
                var userId = int.Parse(accountId.Value).ToString();

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in token." });
                }

                await _userService.UpdateUserAsync(userId, updateUserRequest);
                return Ok("User updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
