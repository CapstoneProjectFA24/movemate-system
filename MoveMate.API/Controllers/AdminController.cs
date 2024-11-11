using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class AdminController : BaseController
    {
        private readonly IUserServices _userServices;

        public AdminController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        /// <summary>
        /// Creates a new user with information provided in the admin's request.
        /// </summary>
        /// <param name="request">An <see cref="AdminCreateUserRequest"/> object containing the user information.</param>
        /// <returns>Returns a response containing the result of user creation.</returns>
        /// <remarks>
        /// **Example Request:**
        ///
        ///     POST /api/admin/createUser
        ///     {
        ///         "name": "Example",
        ///         "password": "Password123!",
        ///         "email": "abc@example.com",
        ///         "phone": "0911234345",
        ///         "roleId": 2,
        ///         "dob": "1990-01-01T00:00:00Z",
        ///         "gender": "Male",
        ///         "createdAt": "2023-10-01T12:00:00Z"
        ///     }
        /// </remarks>
        /// <response code="200">Returns if the user was successfully created.</response>
        /// <response code="400">Returns if the information is invalid or if an error occurs.</response>
        /// <response code="500">Returns if a system error occurs.</response>
        [HttpPost("admin/create-user")]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserRequest request)
        {
          

            var response = await _userServices.CreateUser(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// Bans a user by setting the user's banned status to true.
        /// </summary>
        /// <param name="id">The ID of the user to be banned.</param>
        /// <returns>Returns a response indicating the success or failure of the ban operation.</returns>
        /// <response code="200">Returns if the user was successfully banned.</response>
        /// <response code="404">Returns if the user is not found.</response>
        /// <response code="500">Returns if a system error occurs.</response>
        [HttpPost("admin/ban-user/{id}")]
        public async Task<IActionResult> BanUser(int id)
        {
            var response = await _userServices.BanUser(id);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }




    }
}
