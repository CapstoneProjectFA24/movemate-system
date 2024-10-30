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
        /// <param name="id">The ID of the user info to retrieve.</param>
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
        public async Task<IActionResult> GetAllUserInfoAsync([FromQuery] GetAllUserInfoRequest request)
        {

            var response = await _userService.GetUserInfoByUserIdAsync(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE : Retrieves a user by usser id
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /user/{id}
        /// </remarks>
        /// <response code="200">User information retrieved successfully</response>
        /// <response code="404">User info not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            var response = await _userService.GetUserById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
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
                    return Unauthorized(new { Message = MessageConstant.FailMessage.UserIdInvalid });
                }

                await _userService.UpdateUserAsync(userId, updateUserRequest);
                return Ok("User updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// FEATURE: Set User info's IsDeleted become true by User Info Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("user-info/delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserInfoByUserId(int id)
        {
            var response = await _userService.DeleteUserInfo(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Creates new user information in the system.
        /// </summary>
        /// <param name="request">An instance of <see cref="CreateUserInfoRequest"/> containing user information details.</param>
        /// <returns>A response indicating the result of the user information creation process.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/user-info
        ///     {
        ///         "userId": 1,
        ///         "type": "CAVET",
        ///         "imageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1729864500/movemate/eopqdqwqcblmzc5ymbeg.jpg",
        ///         "value": "324214221212312"
        ///     }
        /// </remarks>
        /// <response code="201">Returns details of the created user information.</response>
        /// <response code="400">If the input data is invalid or required fields are missing.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpPost("user-info")]
        public async Task<IActionResult> CreateUserInfo([FromBody] CreateUserInfoRequest request)
        {
            var response = await _userService.CreateUserInfo(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }


        /// <summary>
        /// CHORE : Update user info by user info id
        /// </summary>
        /// <param name="request">The user info request model.</param>
        /// <returns>A response containing the created user info.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/user-info/{id}
        ///     {
        ///         "type": "CAVET",
        ///         "imageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1729864500/movemate/eopqdqwqcblmzc5ymbeg.jpg",
        ///         "value": "324214221212312"
        ///     }
        /// </remarks>
        /// <response code="201">Returns the created truck category</response>
        /// <response code="500">If there is a server error</response>
        [HttpPut("user-info/{id}")]
        public async Task<IActionResult> UpdateUserInfo(int id, [FromBody] UpdateUserInfoRequest request)
        {
            var response = await _userService.UpdateUserInfoAsync(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }
    }
}