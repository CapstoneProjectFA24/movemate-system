using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] GetAllFeeSetting request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _feeSettingServices.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Retrieves a fee setting by its ID.
        /// </summary>
        /// <param name="id">The ID of the fee setting to retrieve.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /feesetting/1
        /// </remarks>
        /// <response code="200">Get fee setting success</response>
        /// <response code="404">Fee setting not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeeSettingById(int id)
        {
            var response = await _feeSettingServices.GetFeeSettingById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Set fee setting's IsActive become false by fee setting Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFeeSettingById(int id)
        {
            var response = await _feeSettingServices.DeleteActiveFeeSetting(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}