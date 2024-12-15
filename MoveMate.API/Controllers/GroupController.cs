using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class GroupController : BaseController
    {
        private readonly IGroupServices _groupService;


        public GroupController(IGroupServices groupServices)
        {
            _groupService = groupServices;
        }
        /// <summary>
        /// 
        /// FEATURE: Get all Group
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize]

        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllGroupRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _groupService.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Retrieves a group by its ID.
        /// </summary>
        /// <param name="id">The ID of the group to retrieve.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /group/1
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var response = await _groupService.GetGroupById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Set group's IsActive become false by group Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteGroupById(int id)
        {
            var response = await _groupService.DeleteGroup(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE : Creates a new group entry.
        /// </summary>
        /// <param name="request">The request payload containing group detail.</param>
        /// <returns>A response indicating success or failure of the operation.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/group/
        ///     {
        ///         "Name": "Team Alpha",
        ///         "DurationTimeActived": 30
        ///     }
        /// </remarks>
        [HttpPost("")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var response = await _groupService.CreateGroup(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE : Update group by group id
        /// </summary>
        /// <param name="request">The group request model.</param>
        /// <returns>A response containing the created truck category.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Put /api/group
        ///     {
        ///         "Name": "Team Alpha",
        ///         "DurationTimeActived": 30
        ///     }
        /// </remarks>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] UpdateGroupRequest request)
        {
            var response = await _groupService.UpdateGroup(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }

        [HttpPut("add-user")]
        public async Task<IActionResult> AddUserIntoGroup([FromBody] AddUserIntoGroup request)
        {
            var response = await _groupService.AddUserIntoGroup(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }

        [HttpGet("user-in-group/{groupId}")]
        public async Task<IActionResult> UserInGroupp(int groupId)
        {
            var response = await _groupService.GetUserIntoGroup(groupId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }
    }
}