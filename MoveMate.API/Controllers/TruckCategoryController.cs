using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;

namespace MoveMate.API.Controllers;

[ApiController]
public class TruckCategoryController : BaseController
{
    private readonly ITruckServices _truckServices;

    public TruckCategoryController(ITruckServices truckServices)
    {
        _truckServices = truckServices;
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