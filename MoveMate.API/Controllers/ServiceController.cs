using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class ServiceController : BaseController
    {
        private readonly IServiceDetails _serviceDetails;
        private readonly IServiceServices _services;

        public ServiceController(IServiceDetails serviceDetails, IServiceServices services)
        {
            _serviceDetails = serviceDetails;
            _services = services;
        }



        /// <summary>
        /// FEATURE : Retrieves a paginated list of all service not type truck.
        /// </summary>
        /// <param name="request">The request containing pagination and filter parameters.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Get List Services Done</response>
        /// <response code="200">List Service is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("not-type-truck")]

        // get all
        public async Task<IActionResult> GetAll([FromQuery] GetAllServiceRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _services.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE : Retrieves a paginated list of all service type truck.
        /// </summary>
        /// <param name="request">The request containing pagination and filter parameters.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Get List Services Done</response>
        /// <response code="200">List Service has truck type is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("truck-category")]

        // get all
        public async Task<IActionResult> GetAllServiceTruck([FromQuery] GetAllServiceTruckType request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _services.GetAllServiceTruck(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// FEATURE : Retrieves a service by its ID.
        /// </summary>
        /// <param name="id">The ID of the service to retrieve.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /service/1
        /// </remarks>
        /// <response code="200">Get Service by Id Success!</response>
        /// <response code="404">Service not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var response = await _services.GetById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
