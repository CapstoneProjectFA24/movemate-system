using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService;

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
    // get all
    public async Task<IActionResult> GetAllCate()
    {
        //IEnumerable<Claim> claims = HttpContext.User.Claims;

        var response = await _truckServices.GetAllCate();
        _googleMapsService.GetDistanceAndDuration("9.922823, 106.333055", "10.772132, 106.653129");

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
    [HttpGet("{id}")]
    // get all
    public async Task<IActionResult> GetCateById(int id)
    {
        //IEnumerable<Claim> claims = HttpContext.User.Claims;

        var response = await _truckServices.GetCateById(id);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}