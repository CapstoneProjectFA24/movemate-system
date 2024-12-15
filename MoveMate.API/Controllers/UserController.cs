using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.API.Middleware;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;
using System.Security.Claims;
using MoveMate.Service.ThirdPartyService.Cloudinary;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserServices _userService;

        private readonly IFirebaseMiddleware _firebaseMiddleware;
        private readonly CloudinaryService _cloudinaryService;

        public UserController(IUserServices userService, IFirebaseMiddleware firebaseMiddleware, CloudinaryService cloudinaryService)
        {
            _firebaseMiddleware = firebaseMiddleware;
            _cloudinaryService = cloudinaryService;
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
        [Authorize(Roles = "1,6")]
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
        /// CHORE : Update user information
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>Returns a response indicating the success or failure of the update user.</returns>
        /// <response code="200">Returns if the user was successfully updated.</response>
        /// <response code="404">Returns if the user is not found.</response>
        /// <response code="500">Returns if a system error occurs.</response>
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateAccountRequest request)
        {
            var response = await _userService.UpdateAccountAsync(userId, request);

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

        /// <summary>
        /// Allows users to submit a report for an exception or issue.
        /// </summary>
        /// <param name="request">The request model containing exception details.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/user-info/report
        ///     {
        ///         "bookingId": 1,
        ///         "location": "28 Hẻm 635 Hương Lộ 2,Phường Bình Trị Đông,Quận Bình Tân,Thành Phố Hồ Chí Minh",
        ///         "point": "10.767782,106.611362",
        ///         "description": "Accidentally broke it",
        ///         "title": "Report title",
        ///         "estimatedAmount": 2243230,
        ///         "isInsurance": false,
        ///         "resourceList": [
        ///             {
        ///                 "type": "Image",
        ///                 "resourceUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1731766020/movemate/images/kh6logaucsyb6bydkbpu.jpg",
        ///                 "resourceCode": "string"
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <response code="200">If the report is successfully processed.</response>
        /// <response code="400">If the request contains invalid data.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("report")]
        public async Task<IActionResult> UserReport([FromBody] ExceptionRequest request)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);
            var response = await _userService.UserReportException(userId, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }

        /// <summary>
        /// CHORE : Creates a new user with information provided in the admin's request.
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


            var response = await _userService.CreateUser(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Bans a user by setting the user's banned status to true.
        /// </summary>
        /// <param name="id">The ID of the user to be banned.</param>
        /// <returns>Returns a response indicating the success or failure of the ban operation.</returns>
        /// <response code="200">Returns if the user was successfully banned.</response>
        /// <response code="404">Returns if the user is not found.</response>
        /// <response code="500">Returns if a system error occurs.</response>
        [HttpPut("admin/ban-user/{id}")]
        public async Task<IActionResult> BanUser(int id)
        {
            var response = await _userService.BanUser(id);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
        /// <summary>
        /// CHORE : Unban for user by setting the user's banned status to fasle.
        /// </summary>
        /// <param name="userId">The ID of the user to be banned.</param>
        /// <returns>Returns a response indicating the success or failure of the ban operation.</returns>
        /// <response code="200">Returns if the user was successfully banned.</response>
        /// <response code="404">Returns if the user is not found.</response>
        /// <response code="500">Returns if a system error occurs.</response>
        [HttpPut("admin/unban-user/{userId}")]
        public async Task<IActionResult> UnbanUser(int userId)
        {
            var response = await _userService.UnBannedUser(userId);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
        /// <summary>
        /// CHORE : Delete a user by setting the user's deleted status to true.
        /// </summary>
        /// <param name="userId">The ID of the user to be banned.</param>
        /// <returns>Returns a response indicating the success or failure of the ban operation.</returns>
        /// <response code="200">Returns if the user was successfully banned.</response>
        /// <response code="404">Returns if the user is not found.</response>
        /// <response code="500">Returns if a system error occurs.</response>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var response = await _userService.DeleteUser(userId);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE: Creates a new staff member with the information provided in the request.
        /// </summary>
        /// <param name="request">A <see cref="CreateStaffRequest"/> object containing the staff details.</param>
        /// <returns>Returns a response indicating the success or failure of the staff creation operation.</returns>
        /// <remarks>
        /// **Example Request:**
        /// 
        ///     POST /api/staff
        ///     {
        ///         "roleId": 4,
        ///         "name": "Jane Doe",
        ///         "phone": "0987654321",
        ///         "password": "SecurePassword123",
        ///         "gender": "Female",
        ///         "email": "janedoe@example.com",
        ///         "avatarUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg",
        ///         "dob": "1992-08-20T00:00:00Z",
        ///         "userInfo": [
        ///             {
        ///                 "Type": "CITIZEN_IDENTIFICATION_CARD",
        ///                 "imageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg",
        ///                 "Image": "file img",
        ///                 "value": "CID123456"
        ///             },
        ///             {
        ///                 "Type": "HEALTH_CERTIFICATE",
        ///                 "ImageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg",
        ///                 "Image": "file img",
        ///                 "Value": "HC789012"
        ///              },
        ///              {
        ///                 "Type": "DRIVER_LICENSE",
        ///                 "ImageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg",
        ///                 "Image": "file img",
        ///                 "Value": "DL345678"
        ///              },
        ///              {
        ///                 "Type": "CRIMINAL_RECORD",
        ///                 "ImageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg",
        ///                 "Image": "file img",
        ///                 "Value": "CR901234"
        ///              },
        ///              {
        ///                 "Type": "CURRICULUM_VITAE",
        ///                 "ImageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg",
        ///                 "Image": "file img",
        ///                 "Value": "CV567890"
        ///              },
        ///              {
        ///                 "Type": "PORTRAIT",
        ///                 "ImageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1730660748/movemate/ggaaf2ckbqyxguosytwa.jpg",
        ///                 "Image": "file img",
        ///                 "Value": "Portrait of John Doe"
        ///              }
        ///         ]
        ///     }
        /// </remarks>
        /// <response code="200">Returns if the staff member was successfully created.</response>
        /// <response code="400">Returns if the provided information is invalid or incomplete.</response>
        /// <response code="500">Returns if an unexpected system error occurs.</response>
        [HttpPost("staff")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request)
        {
            var response = await _userService.CreateStaff(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }

        /// <summary>
        /// CHORE: Approves a staff member by updating their acceptance status.
        /// </summary>
        /// <param name="userId">The ID of the staff member to be accepted.</param>
        /// <returns>Returns a response indicating the success or failure of the acceptance operation.</returns>
        /// <remarks>
        /// **Example Request:**
        /// 
        ///     PUT /api/accept-staff/123
        /// 
        /// **Details:**
        /// - This endpoint updates the `IsAccepted` property of the staff member with the given `userId` to `true`.
        /// - Sends an acceptance email notification to the staff member upon successful update.
        /// </remarks>
        /// <response code="200">Returns if the staff member was successfully accepted.</response>
        /// <response code="404">Returns if the specified staff member is not found.</response>
        /// <response code="500">Returns if a system error occurs.</response>
        [HttpPut("accept-staff/{userId}")]
        public async Task<IActionResult> AcceptStaff(int userId)
        {
            var response = await _userService.AcceptUser(userId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }
        
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File cannot be null or empty.");
            }

            try
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(file);
                return Ok(new { ImageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while uploading the image.", Details = ex.Message });
            }
        }
    }
}