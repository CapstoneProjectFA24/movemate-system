using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class ServiceDetailController : BaseController
    {
        private readonly IServiceDetails _serviceDetails;

        public ServiceDetailController(IServiceDetails serviceDetails)
        {
            _serviceDetails = serviceDetails;
        }
        
        
        /// <summary>
        /// 
        /// Get all users
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/get-all")]
        // get all
        public async Task<IActionResult> GetAll()
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;
            // bibi dsds
            //var response = await _serviceDetails.GetAll();
            //var response = true;
            
            //return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
            return Ok("Service Details");
        }
    }
}
