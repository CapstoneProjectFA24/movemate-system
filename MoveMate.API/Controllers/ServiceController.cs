using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
using MoveMate.Service.Constants;
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
        public ServiceController(IServiceServices services, IMessageProducer producer,
            IRedisService redisService)
        {
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
            var queryRequestHash = request.GetExpressions().ToString().GetHashCode();
            var keyService = RedisConstants.ServiceConstants.GetService + queryRequestHash;

            var response = await _redisService.GetDataAsync<OperationResult<List<ServicesResponse>>>(keyService);
            if (response == null)
            {
                response = await _services.GetAllNotTruck(request);
                if (!response.IsError)
                {
                    await _redisService.SetDataAsync(keyService, response, TimeSpan.FromHours(10));
                }
            }

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
            var queryRequestHash = request.GetExpressions().ToString().GetHashCode();
            var keyService = RedisConstants.ServiceConstants.GetService + queryRequestHash;

            var response = await _redisService.GetDataAsync<OperationResult<List<ServiceResponse>>>(keyService);
            if (response == null)
            {
                response = await _services.GetAllServiceTruck(request);
                if (!response.IsError)
                {
                    await _redisService.SetDataAsync(keyService, response, TimeSpan.FromHours(10));
                }
            }

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

            _producer.SendingMessage<String>("hello");
            string queueKey = "myQueue";

            var queryRequestHash = request.GetExpressions().ToString().GetHashCode();
            var keyService = RedisConstants.ServiceConstants.GetService + queryRequestHash;
            
            var response = await _redisService.GetDataAsync<OperationResult<List<ServicesResponse>>>(keyService);
            if (response == null)
            {
                response = await _services.GetAll(request);

                if (!response.IsError)
                {
                    await _redisService.SetDataAsync(keyService, response, TimeSpan.FromHours(20));
                }
            }

            await _redisService.EnqueueAsync(queueKey, response.Payload);
            var key = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + "mykey";
            //await _redisService.SetDataAsync(key, "Hello, World! my key");

            await _redisService.EnqueueWithExpiryAsync("testqueue", "test");

            await _redisService.SetDataAsync(key, response.Payload);
            await _redisService.RemoveKeysWithPatternAsync("my");

            var check = await _redisService.GetDataAsync<List<ServicesResponse>>(key);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// Creates a new service with the specified details.
        /// </summary>
        /// <param name="request">An instance of <see cref="CreateServiceRequest"/> containing service information.</param>
        /// <returns>A response indicating the result of the service creation process.</returns>
        /// <remarks>
        /// **Example Request:**
        /// 
        ///     POST /api/admin/createService
        ///     {
        ///         "name": "Premium Service",
        ///         "description": "Premium moving service with additional features.",
        ///         "isActived": true,
        ///         "tier": 1,
        ///         "imageUrl": "https://example.com/image.jpg",
        ///         "discountRate": 10,
        ///         "amount": 100.0,
        ///         "parentServiceId": null,
        ///         "type": "Full-Service",
        ///         "isQuantity": true,
        ///         "quantityMax": 5,
        ///         "truckCategoryId": 2
        ///     }
        /// 
        /// **Request Properties:**
        /// - **name**: The name of the service (e.g., "Premium Service").
        /// - **description**: A brief description of the service (e.g., "Premium moving service with additional features.").
        /// - **isActived**: Indicates whether the service is active (true or false).
        /// - **tier**: The tier level of the service (e.g., 1).
        /// - **imageUrl**: URL to the service image (e.g., "https://example.com/image.jpg").
        /// - **discountRate**: The discount percentage applied to the service (e.g., 10).
        /// - **amount**: The base price of the service (e.g., 100.0).
        /// - **parentServiceId**: The ID of the parent service, if applicable (null if none).
        /// - **type**: The type of service being created (e.g., "Full-Service").
        /// - **isQuantity**: Indicates whether the service has a quantity limit (true or false).
        /// - **quantityMax**: The maximum quantity allowed for the service (e.g., 5).
        /// - **truckCategoryId**: The ID of the truck category if applicable (e.g., 2).
        /// </remarks>
        /// <response code="200">Returns if the service is successfully created, with the created service details.</response>
        /// <response code="400">Returns if the input data is invalid or the creation fails.</response>
        /// <response code="500">Returns if an internal server error occurs.</response>
        [HttpPost("manage/create-service")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
        {
            var response = await _services.CreateService(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// Delete service by setting the service's IsActive status to false.
        /// </summary>
        /// <param name="id">The ID of the service to be deleeted.</param>
        /// <returns>Returns a response indicating the success or failure of the ban operation.</returns>
        /// <response code="200">Returns if the user was successfully banned.</response>
        /// <response code="404">Returns if the user is not found.</response>
        /// <response code="500">Returns if a system error occurs.</response>
        [HttpDelete("manager/deleted-service/{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var response = await _services.DeleteService(id);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}