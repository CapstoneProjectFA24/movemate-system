using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class TruckController : BaseController
    {
        private readonly ITruckServices _truckServices;

        public TruckController(ITruckServices truckServices)
        {
            _truckServices = truckServices;
        }
        
        /// <summary>
        /// 
        /// get truck
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllTruckRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _truckServices.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
        
        
    }
}
