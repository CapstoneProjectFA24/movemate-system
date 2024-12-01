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
        [Authorize]
        // get all
        public async Task<IActionResult> GetAllNotTruck([FromQuery] GetAllServiceNotTruckRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;
            
            /*var hash = request.GetExpressions() + request.page.ToString() + request .per_page + request.GetOrder();
            
            var queryRequestHash = hash.GetHashCode();
            var keyService = RedisConstants.ServiceConstants.GetService + queryRequestHash;

            var response = await _redisService.GetDataAsync<OperationResult<List<ServicesResponse>>>(keyService);
            if (response == null)
            {
                response = await _services.GetAllNotTruck(request);
                if (!response.IsError)
                {
                    await _redisService.SetDataAsync(keyService, response, TimeSpan.FromMinutes(30));
                }
            }*/
            var response = await _services.GetAllNotTruck(request);

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
        [Authorize]
        // get all
        public async Task<IActionResult> GetAllServiceTruck([FromQuery] GetAllServiceTruckType request)
        {
            /*var hash = request.GetExpressions() + request.page.ToString() + request .per_page + request.GetOrder();
            
            var queryRequestHash = hash.GetHashCode();
            var keyService = RedisConstants.ServiceConstants.GetService + queryRequestHash;

            var response = await _redisService.GetDataAsync<OperationResult<List<ServiceResponse>>>(keyService);
            if (response == null)
            {
                response = await _services.GetAllServiceTruck(request);
                if (!response.IsError)
                {
                    await _redisService.SetDataAsync(keyService, response, TimeSpan.FromMinutes(30));
                }
            }*/
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
        /// - If "Status" contains `NOTTRUCK`, the list will include bookings with statuses different type `TRUCK` instead.
        /// </remarks>
        /// <response code="200">Get List Services Done</response>
        /// <response code="200-1">List Service is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] GetAllServiceRequest request, [FromQuery] CalServiceRequest requestBody)
        {

            _producer.SendingMessage<String>("hello");
            string queueKey = "myQueue";

            /*var hash = request.GetExpressions() + request.page.ToString() + request .per_page + request.GetOrder();
            
            var queryRequestHash = hash.GetHashCode();
            var keyService = RedisConstants.ServiceConstants.GetService + queryRequestHash;
            
            var response = await _redisService.GetDataAsync<OperationResult<List<ServicesResponse>>>(keyService);
            if (response == null)
            {
                response = await _services.GetAll(request);

                if (!response.IsError)
                {
                    await _redisService.SetDataAsync(keyService, response, TimeSpan.FromMinutes(30));
                }
            }*/
            var response = await _services.GetAll(request, requestBody);
            await _redisService.EnqueueAsync(queueKey, response.Payload);
            //var key = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + "mykey";
            //await _redisService.SetDataAsync(key, "Hello, World! my key");

            //await _redisService.EnqueueWithExpiryAsync("testqueue", "test");

            //await _redisService.SetDataAsync(key, response.Payload);
            //await _redisService.RemoveKeysWithPatternAsync("my");

            //var check = await _redisService.GetDataAsync<List<ServicesResponse>>(key);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }



        [HttpPost("service-dynamic")]
        [Authorize]
        public async Task<IActionResult> GetAllServiceDynamic([FromBody] CalServiceRequest request)
        {
            var response = await _services.CalService(request); 
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }




        /// <summary>
        /// CHORE : Creates a new service with the specified details.
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
        ///         "imageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1728489912/movemate/vs174go4uz7uw1g9js2e.jpg",
        ///         "discountRate": 0,
        ///         "amount": 12340,
        ///         "type": "PORTER",
        ///         "isQuantity": true,
        ///         "quantityMax": 0,
        ///         "parentServiceId": 1
        ///     }
        /// 
        /// **Validation Rules:**
        /// - `TruckCategoryId` is required if `Type` is set to `"Truck"`.
        /// - `ParentServiceId` must refer to an existing Tier 0 service if `tier` is `1`.
        /// - `InverseParentService` types must match the current service's type.
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
        /// CHORE : Delete service by setting the service's IsActive status to false.
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


        /// <summary>
        /// CHORE : Updates an existing service by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the service to update.</param>
        /// <param name="request">The request model containing updated service details.</param>
        /// <returns>An IActionResult containing the operation result, including the updated service details on success or error information on failure.</returns>
        /// <remarks>
        /// This endpoint updates a service's details by ID. It includes logic to ensure data integrity based on the service's type and tier. The service must already exist, and specific rules apply based on service type and whether a parent service ID is provided.
        /// 
        /// Sample Request:
        /// 
        ///     PUT /api/service/manager/{id}
        ///     {
        ///         "name": "Standard Moving Service",
        ///         "description": "Basic moving service",
        ///         "isActived": true,
        ///         "imageUrl": "https://res.cloudinary.com/dkpnkjnxs/image/upload/v1728489912/movemate/vs174go4uz7uw1g9js2e.jpg",
        ///         "discountRate": 10.0,
        ///         "amount": 250.00,
        ///         "type": "Moving",
        ///         "isQuantity": true,
        ///         "quantityMax": 5,
        ///         "truckCategoryId": 1,
        ///         "parentServiceId": 2
        ///     }
        ///     
        /// Important Notes:
        /// - If the service is a Tier 0 service, it cannot have a parent service assigned (`parentServiceId` should be null).
        /// - If `parentServiceId` is provided, the parent service must exist and have the same type as the current service.
        /// - For services of type "TRUCK," `truckCategoryId` is required. Providing a `truckCategoryId` with other types will result in a validation error.
        /// - Synchronization checks ensure that service types and tier restrictions are respected during updates.
        /// 
        /// Response Codes:
        /// - 200: Successfully updated service.
        /// - 400: Validation failure or tier restriction violation.
        /// - 404: Specified service or parent service not found.
        /// - 500: Internal server error due to unexpected issues.
        /// </remarks>
        /// <response code="200">Returns the updated service.</response>
        /// <response code="400">If validation errors or tier restrictions are violated.</response>
        /// <response code="404">If the specified service or parent service is not found.</response>
        /// <response code="500">If there is a server error.</response>
        [HttpPut("manager/{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceRequest request)
        {
            var response = await _services.UpdateService(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


    }
}