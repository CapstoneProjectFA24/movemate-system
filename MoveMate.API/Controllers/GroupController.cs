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


        /// <summary>
        /// CHORE : Creates a new truck image entry.
        /// </summary>
        /// <param name="request">The request payload containing truck image details.</param>
        /// <returns>A response indicating success or failure of the operation.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/truck/manage/truck-img
        ///     {
        ///         "TruckId": 1,
        ///         "ImageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1729831911/movemate/hkvbh89uo8qoh6uzajac.jpg",
        ///         "ImageCode": "TRUCK123IMG"
        ///     }
        /// </remarks>
        /// <response code="201">Truck image created successfully.</response>
        /// <response code="400">Bad request, invalid data.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var response = await _groupService.CreateGroup(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE : Update truck category by truck category id
        /// </summary>
        /// <param name="request">The truck category request model.</param>
        /// <returns>A response containing the created truck category.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/truck/manage/truck-img
        ///     {
        ///         "categoryName": "Heavy Duty Truck",
        ///         "maxLoad": 12000,
        ///         "description": "A truck suitable for heavy loads.",
        ///         "imageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1728489912/movemate/vs174go4uz7uw1g9js2e.jpg",
        ///         "estimatedLength": "7.5m",
        ///         "estimatedWidth": "2.5m",
        ///         "estimatedHeight": "3.0m",
        ///         "summarize": "Designed for heavy-duty transport.",
        ///         "price": 15000,
        ///         "totalTrips": 100
        ///     }
        /// </remarks>
        /// <response code="201">Returns the created truck category</response>
        /// <response code="500">If there is a server error</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTruckCategory(int id, [FromBody] UpdateGroupRequest request)
        {
            var response = await _groupService.UpdateGroup(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUserIntoGroup([FromBody] AddUserIntoGroup request)
        {
            var response = await _groupService.AddUserIntoGroup(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }
    }
}