using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class PromotionController : BaseController
    {
        private readonly IPromotionServices _promotionServices;

        public PromotionController(IPromotionServices promotionServices)
        {
            _promotionServices = promotionServices;
        }

        /// <summary>
        /// 
        /// CHORE : Get all promotion
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPromotionRequest request)
        {
            var response = await _promotionServices.GetAllPromotion(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// 
        /// CHORE : Get all promotion has user, no user
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("promotions")]
        [Authorize]
        public async Task<IActionResult> GetAllPromotion()
        {
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { statusCode = 401, message = MessageConstant.FailMessage.UserIdInvalid, isError = true });
            }

            var userId = int.Parse(accountIdClaim.Value);
            var response = await _promotionServices.GetListPromotion(userId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE : Set promotion category's IsDeleted become true by promotion category Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePromotionCategoryById(int id)
        {
            var response = await _promotionServices.DeletePromotion(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE : Creates a new promotion entry with associated vouchers.
        /// </summary>
        /// <param name="request">The <see cref="CreatePromotionRequest"/> object containing promotion details and associated vouchers.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the outcome of the operation.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/promotions
        ///     {
        ///         "isPublic": true,
        ///         "startDate": "2024-10-29T00:00:00",
        ///         "endDate": "2024-11-30T23:59:59",
        ///         "discountRate": 10.0,
        ///         "discountMax": 50.0,
        ///         "requireMin": 100.0,
        ///         "discountMin": 10.0,
        ///         "name": "Winter Sale",
        ///         "description": "Special winter discount",
        ///         "type": "Seasonal",
        ///         "quantity": 100,
        ///         "startBookingTime": "2024-10-29T00:00:00",
        ///         "endBookingTime": "2024-11-30T23:59:59",
        ///         "isInfinite": false,
        ///         "serviceId": 1,
        ///         "vouchers": [
        ///             {
        ///                 "price": 50.0,
        ///                 "code": "WINTER50"
        ///             }
        ///         ]
        ///     }
        /// 
        /// </remarks>
        /// <response code="201">Promotion created successfully, including voucher details if provided.</response>
        /// <response code="400">Bad request, invalid or incomplete data provided.</response>
        /// <response code="404">Related service not found for the provided <see cref="ServiceId"/>.</response>
        /// <response code="500">Internal server error occurred while creating the promotion.</response>
        [HttpPost()]
        public async Task<IActionResult> CreatePromotion([FromBody] CreatePromotionRequest request)
        {
            var response = await _promotionServices.CreatePromotion(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE : Update truck category by truck category id
        /// </summary>
        /// <param name="request">The truck category request model.</param>
        /// <returns>A response containing the created truck category.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/truck/manage/truck-img
        ///     {
        ///         "isPublic": true,
        ///         "startDate": "2024-10-29T00:00:00",
        ///         "endDate": "2024-11-30T23:59:59",
        ///         "discountRate": 10.0,
        ///         "discountMax": 50.0,
        ///         "requireMin": 100.0,
        ///         "discountMin": 10.0,
        ///         "name": "Winter Sale",
        ///         "description": "Special winter discount",
        ///         "type": "Seasonal",
        ///         "quantity": 100,
        ///         "startBookingTime": "2024-10-29T00:00:00",
        ///         "endBookingTime": "2024-11-30T23:59:59",
        ///         "isInfinite": false,
        ///         "serviceId": 1
        ///     }
        /// </remarks>
        /// <response code="201">Returns the created truck category</response>
        /// <response code="500">If there is a server error</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromotionCategory(int id, [FromBody] UpdatePromotionRequest request)
        {
            var response = await _promotionServices.UpdatePromotion(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);

        }



        /// <summary>
        /// CHORE : Retrieves a promotion category by its ID.
        /// </summary>
        /// <param name="id">The ID of the promotion category to retrieve.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /promotioncategory/1
        /// </remarks>
        /// <response code="200">Get promotion category success</response>
        /// <response code="404">Promotion category not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPromotionCategoryById(int id)
        {
            var response = await _promotionServices.GetPromotionById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}