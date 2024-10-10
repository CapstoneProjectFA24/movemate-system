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
       

        public HouseTypeController(IHouseTypeServices houseTypeServices, IHouseTypeSettingServices houseTypeSettingServices)
        {
            _houseTypeServices = houseTypeServices;
            _houseTypeSettingServices = houseTypeSettingServices;
        }

        /// <summary>
        /// 
        /// Get all house type
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]

        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllHouseTypeRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _houseTypeServices.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// Get house type by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHouseTypeById(int id)
        {
            var response = await _houseTypeServices.GetById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// Create a new house type setting.
        /// </summary>
        /// <param name="request">The details of the auction to create.</param>
        /// <returns>Returns the result of the auction creation.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "houseTypeId": 1,
        ///         "truckCategoryId": "1",
        ///         "numberOfFloors": "3",
        ///         "numberOfRooms": 10,
        ///         "numberOfTrucks": 2
        ///     }
        /// </remarks>
        [HttpPost("house-type-setting")]
        //[Authorize]
        public async Task<IActionResult> Create(CreateHouseTypeSetting request)
        {
            var response = await _houseTypeSettingServices.CreateEntity(request);
            if (response.IsError)
            {
                return HandleErrorResponse(response.Errors);
            }

            // Return 201 Created status with the success message
            return StatusCode((int)Service.Commons.StatusCode.Created, response);
        }
    }
}
