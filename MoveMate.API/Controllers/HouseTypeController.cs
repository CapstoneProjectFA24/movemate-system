using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Repository.Repositories.IRepository;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class HouseTypeController : BaseController
    {
        private readonly IHouseTypeServices _houseTypeServices;
        private readonly IHouseTypeSettingServices _houseTypeSettingServices;


        public HouseTypeController(IHouseTypeServices houseTypeServices,
            IHouseTypeSettingServices houseTypeSettingServices)
        {
            _houseTypeServices = houseTypeServices;
            _houseTypeSettingServices = houseTypeSettingServices;
        }

        /// <summary>
        /// FEATURE : Retrieves a paginated list of all house types.
        /// </summary>
        /// <param name="request">The request containing pagination and filter parameters.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Get List Auctions Done</response>
        /// <response code="200">List House Type is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("")]

        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllHouseTypeRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _houseTypeServices.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE : Retrieves a house type by its ID.
        /// </summary>
        /// <param name="id">The ID of the house type to retrieve.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /housetype/1
        /// </remarks>
        /// <response code="200">Get House Type success</response>
        /// <response code="404">House type not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHouseTypeById(int id)
        {
            var response = await _houseTypeServices.GetById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        ///// <summary>
        ///// CHORE : Creates a new house type setting.
        ///// </summary>
        ///// <param name="request">The details of the house type setting to create.</param>
        ///// <returns>An IActionResult indicating the result of the creation operation.</returns>
        ///// <remarks>
        ///// Sample request:
        ///// 
        /////     POST /housetype/house-type-setting
        /////     {
        /////         "houseTypeId": 1,
        /////         "truckCategoryId": "1",
        /////         "numberOfFloors": "3",
        /////         "numberOfRooms": 10,
        /////         "numberOfTrucks": 2
        /////     }
        ///// </remarks>
        ///// <response code="201">Add HouseTypeSetting Success!</response>
        ///// <response code="400">Add HouseTypeSetting Failed!</response>
        ///// <response code="404">House Type not found</response>
        ///// <response code="404">Truck Category not found</response>
        ///// <response code="500">An error occurred while creating the house type setting</response>
        //[HttpPost("house-type-setting")]
        ////[Authorize]
        //public async Task<IActionResult> Create(CreateHouseTypeSetting request)
        //{
        //    var response = await _houseTypeSettingServices.CreateEntity(request);
        //    if (response.IsError)
        //    {
        //        return HandleErrorResponse(response.Errors);
        //    }

        //    // Return 201 Created status with the success message
        //    return StatusCode((int)Service.Commons.StatusCode.Created, response);
        //}


        /// <summary>
        /// Delete house type by setting the house type's IsActived status to false.
        /// </summary>
        /// <param name="id">The ID of the house type to be deleeted.</param>
        /// <returns>Returns a response indicating the success or failure of the ban operation.</returns>
        /// <response code="200">Returns if the user was successfully banned.</response>
        /// <response code="404">Returns if the user is not found.</response>
        /// <response code="500">Returns if a system error occurs.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHouseType(int id)
        {
            var response = await _houseTypeServices.DeleteHouseType(id);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE : Creates a new house type.
        /// </summary>
        /// <param name="request">The house type request model.</param>
        /// <returns>A response containing the created house type.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/housetype/
        ///     {
        ///         "name": "Heavy Duty Truck",
        ///         "description": "A truck suitable for heavy loads."
        ///     }
        /// </remarks>
        /// <response code="201">Returns the created truck category</response>
        /// <response code="500">If there is a server error</response>
        [HttpPost()]
        public async Task<IActionResult> CreateHouseType([FromBody] CreateHouseTypeRequest request)
        {
            var response = await _houseTypeServices.CreateHouseType(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }


        /// <summary>
        /// CHORE : Update house type by house type id
        /// </summary>
        /// <param name="request">The house type request model.</param>
        /// <returns>A response containing the created house type.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/housetype/
        ///     {
        ///         "name": "Heavy Duty Truck",
        ///         "description": "A truck suitable for heavy loads."
        ///     }
        /// </remarks>
        /// <response code="201">Returns the created truck category</response>
        /// <response code="500">If there is a server error</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHouseType(int id, [FromBody] UpdateHouseTypeRequest request)
        {
            var response = await _houseTypeServices.UpdateHouseType(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }
    }
}