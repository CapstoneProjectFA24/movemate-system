using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ThirdPartyService;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;

namespace MoveMate.API.Controllers;

[ApiController]
public class TruckCategoryController : BaseController
{
    private readonly ITruckServices _truckServices;
    private readonly IGoogleMapsService _googleMapsService;

    public TruckCategoryController(ITruckServices truckServices, IGoogleMapsService googleMapsService)
    {
        _truckServices = truckServices;
        _googleMapsService = googleMapsService;
    }

    /// <summary>
    /// CHORE : Retrieves a paginated list of all truck category.
    /// </summary>
    /// <param name="request">The request containing pagination and filter parameters.</param>
    /// <returns>An IActionResult containing the operation result.</returns>
    /// <remarks>
    /// </remarks>
    /// <response code="200">Get List Truck Category Done</response>
    /// <response code="200">List TruckCate is Empty!</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("")]
    [Authorize]
    // get all
    public async Task<IActionResult> GetAllCate([FromQuery] GetAllTruckCategoryRequest request)
    {
        //IEnumerable<Claim> claims = HttpContext.User.Claims;

        var response = await _truckServices.GetAllTruckCategory(request);
       // _googleMapsService.GetDistanceAndDuration("9.922823, 106.333055", "10.772132, 106.653129");

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    /// <summary>
    /// CHORE : Retrieves a truck category by its ID.
    /// </summary>
    /// <param name="id">The ID of the truck category to retrieve.</param>
    /// <returns>An IActionResult containing the operation result.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /housetype/1
    /// </remarks>
    /// <response code="200">Get Truck Category Detail Done</response>
    /// <response code="500">Internal server error occurred</response>
    //[HttpGet("{id}")]
    //[Authorize]
    //// get all
    //public async Task<IActionResult> GetCateById(int id)
    //{
    //    //IEnumerable<Claim> claims = HttpContext.User.Claims;

    //    var response = await _truckServices.GetCateById(id);

    //    return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    //}

    /// <summary>
    /// CHORE : Retrieves a truck category by its ID.
    /// </summary>
    /// <param name="id">The ID of the truck category to retrieve.</param>
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
    public async Task<IActionResult> GetTruckCategoryById(int id)
    {
        var response = await _truckServices.GetTruckCategoryById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    /// <summary>
    /// FEATURE: Set Truck image's IsDeleted become true by truck img Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("manager/truck-img/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteTruckImgById(int id)
    {
        var response = await _truckServices.DeleteTruckImg(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    /// <summary>
    /// Creates a new truck image entry.
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
    [HttpPost("manage/truck-img")]
    public async Task<IActionResult> CreateTruckImg([FromBody] CreateTruckImgRequest request)
    {
        var response = await _truckServices.CreateTruckImg(request);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    /// <summary>
    /// Delete truck category by setting the truck category's IsDeleted status to true.
    /// </summary>
    /// <param name="id">The ID of the truck category to be deleeted.</param>
    /// <returns>Returns a response indicating the success or failure of the ban operation.</returns>
    /// <response code="200">Returns if the user was successfully banned.</response>
    /// <response code="404">Returns if the user is not found.</response>
    /// <response code="500">Returns if a system error occurs.</response>
    [HttpDelete("manager/deleted/{id}")]
    public async Task<IActionResult> DeleteTruckCategory(int id)
    {
        var response = await _truckServices.DeleteTruckCategory(id);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    /// <summary>
    /// Creates a new truck category.
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
    [HttpPost("manager/truck-category")]
    public async Task<IActionResult> CreateTruckCategory([FromBody] TruckCategoryRequest request)
    {
        var response = await _truckServices.CreateTruckCategory(request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

    }


    /// <summary>
    /// Update truck category by truck category id
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
    [HttpPut("manager/truck-category/{id}")]
    public async Task<IActionResult> UpdateTruckCategory(int id, [FromBody] TruckCategoryRequest request)
    {
        var response = await _truckServices.UpdateTruckCategory(id, request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

    }

    /// <summary>
    /// CHORE : Retrieves a paginated list of all truck .
    /// </summary>
    /// <param name="request">The request containing pagination and filter parameters.</param>
    /// <returns>An IActionResult containing the operation result.</returns>
    /// <remarks>
    /// </remarks>
    /// <response code="200">Get List Truck Done</response>
    /// <response code="200">List Truck is Empty!</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("truck")]
    [Authorize]
    // get all
    public async Task<IActionResult> GetAllTruck([FromQuery] GetAllTruckRequest request)
    {
        //IEnumerable<Claim> claims = HttpContext.User.Claims;

        var response = await _truckServices.GetAllTruck(request);
        // _googleMapsService.GetDistanceAndDuration("9.922823, 106.333055", "10.772132, 106.653129");

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    /// <summary>
    /// CHORE : Retrieves a truck by its ID.
    /// </summary>
    /// <param name="id">The ID of the truck to retrieve.</param>
    /// <returns>An IActionResult containing the operation result.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /truckcategory/truck
    /// </remarks>
    /// <response code="200">Get House Type success</response>
    /// <response code="404">House type not found</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpGet("truck/{id}")]
    public async Task<IActionResult> GetTruckById(int id)
    {
        var response = await _truckServices.GetTruckById(id);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    /// <summary>
    /// Delete truck by setting the truck category's IsDeleted status to true.
    /// </summary>
    /// <param name="id">The ID of the truck to be deleeted.</param>
    /// <returns>Returns a response indicating the success or failure of the ban operation.</returns>
    /// <response code="200">Returns if the user was successfully banned.</response>
    /// <response code="404">Returns if the user is not found.</response>
    /// <response code="500">Returns if a system error occurs.</response>
    [HttpDelete("truck/{id}")]
    public async Task<IActionResult> DeleteTruck(int id)
    {
        var response = await _truckServices.DeleteTruck(id);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    /// <summary>
    /// Creates a new truck entry based on the provided details.
    /// </summary>
    /// <param name="request">An object containing details for creating the truck, including category, model, and specifications.</param>
    /// <returns>An IActionResult containing the response indicating the success or failure of the operation.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/truckcategory/truck
    ///     {
    ///         "truckCategoryId": 1,
    ///         "model": "Ford F-150",
    ///         "numberPlate": "ABC123",
    ///         "capacity": 5.5,
    ///         "isAvailable": true,
    ///         "brand": "Ford",
    ///         "color": "Blue",
    ///         "isInsurrance": true,
    ///         "userId": 3
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Truck created successfully.</response>
    /// <response code="400">Bad request, invalid data provided, or user is not a driver.</response>
    /// <response code="404">Specified user or truck category was not found.</response>
    /// <response code="409">User already owns a truck in the specified category.</response>
    /// <response code="500">Internal server error occurred during processing.</response>
    [HttpPost("truck")]
    public async Task<IActionResult> CreateTruck([FromBody] CreateTruckRequest request)
    {
        var response = await _truckServices.CreateTruck(request);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}