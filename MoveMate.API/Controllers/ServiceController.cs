using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ThirdPartyService.RabbitMQ;
using MoveMate.Service.ThirdPartyService.Redis;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class ServiceController : BaseController
    {
        private readonly IServiceDetails _serviceDetails;
        private readonly IServiceServices _services;
        private readonly IMessageProducer _producer;
        private readonly IRedisService _redisService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceDetails"></param>
        /// <param name="services"></param>
        /// <param name="producer"></param>
        /// <param name="redisService"></param>
        public ServiceController(IServiceDetails serviceDetails, IServiceServices services, IMessageProducer producer,
            IRedisService redisService)
        {
            _serviceDetails = serviceDetails;
            _services = services;
            _producer = producer;
            _redisService = redisService;
        }

        /// <summary>
        /// CHORE : Retrieves a paginated list of all service not type truck.
        /// </summary>
        /// <param name="request">The request containing pagination and filter parameters.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Get List Services Done</response>
        /// <response code="200">List Service is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("not-type-truck")]
        [Authorize]
        // get all
        public async Task<IActionResult> GetAllNotTruck([FromQuery] GetAllServiceNotTruckRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _services.GetAllNotTruck(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Retrieves a paginated list of all service type truck.
        /// </summary>
        /// <param name="request">The request containing pagination and filter parameters.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Get List Services Done</response>
        /// <response code="200">List Service has truck type is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("truck-category")]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var response = await _services.GetById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// FEATURE : Retrieves a paginated list of all services.
        /// </summary>
        /// <param name="request">The request containing pagination and filter parameters.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Get List Services Done</response>
        /// <response code="200-1">List Service is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] GetAllServiceRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _services.GetAll(request);

            _producer.SendingMessage<String>("hello");
            string queueKey = "myQueue";

            await _redisService.EnqueueAsync(queueKey,response.Payload);
            var key = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + "mykey";
            //await _redisService.SetDataAsync(key, "Hello, World! my key");

            await _redisService.EnqueueWithExpiryAsync("testqueue", "test");

            await _redisService.SetDataAsync(key, response.Payload);

            var check = await _redisService.GetDataAsync<List<ServicesResponse>>(key);
                
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}