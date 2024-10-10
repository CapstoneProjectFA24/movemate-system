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
        /// 
        /// FEATURE: Get all service type system
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("system")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllFeeSetting request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _feeSettingServices.GetAll(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
