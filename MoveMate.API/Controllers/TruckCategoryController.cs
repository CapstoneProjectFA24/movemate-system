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
    /// 
    /// get TruckCategory
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("")]
    // get all
    public async Task<IActionResult> GetAllCate()
    {
        //IEnumerable<Claim> claims = HttpContext.User.Claims;

        var response = await _truckServices.GetAllCate();
        //_googleMapsService.GetDistanceAndDuration("9.922823, 106.333055","10.772132, 106.653129");

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    /// <summary>
    /// 
    /// get by ID TruckCategory
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    // get all
    public async Task<IActionResult> GetCateById(int id)
    {
        //IEnumerable<Claim> claims = HttpContext.User.Claims;

        var response = await _truckServices.GetCateById(id);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}