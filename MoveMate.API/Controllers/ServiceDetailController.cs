using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class ServiceDetailController : BaseController
    {
        private readonly IServiceDetails _serviceDetails;
        private readonly IServiceServices _services;

        public ServiceDetailController(IServiceDetails serviceDetails, IServiceServices services)
        {
            _serviceDetails = serviceDetails;
            _services = services;
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

        /// <summary>
        /// 
        /// Get all services
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]

        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllServiceRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _services.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// Get services by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("service/{id}")]
        public async Task<IActionResult> GetHouseTypeById(int id)
        {
            var response = await _services.GetById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
