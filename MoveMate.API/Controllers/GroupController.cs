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
        ///     GET /truckcategory/1
        /// </remarks>
        /// <response code="200">Get House Type success</response>
        /// <response code="404">House type not found</response>
        /// <response code="500">Internal server error occurred</response>
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
    }
}