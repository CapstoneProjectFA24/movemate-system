using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    public class FeeController : BaseController
    {
        private readonly IFeeSettingServices _feeSettingServices;

        public FeeController(IFeeSettingServices feeSettingServices)
        {
            _feeSettingServices = feeSettingServices;
        }

        /// <summary>
        /// FEATURE : Retrieves a paginated list of all fee setting type system.
        /// </summary>
        /// <param name="request">The request containing pagination and filter parameters.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Get List Fees Done</response>
        /// <response code="200">List Fees is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("system")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllFeeSetting request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _feeSettingServices.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}