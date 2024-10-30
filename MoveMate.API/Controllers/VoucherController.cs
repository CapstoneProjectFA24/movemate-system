using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels.ModelRequests;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class VoucherController : BaseController
    {
       private readonly IVoucherService _voucherService;


        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        /// <summary>
        /// CHORE : Retrieves a paginated list of all voucher.
        /// </summary>
        /// <param name="request">The request containing pagination and filter parameters.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Get List Voucher Done</response>
        /// <response code="200">List Voucher is Empty!</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllVoucherRequest request)
        {
            //IEnumerable<Claim> claims = HttpContext.User.Claims;

            var response = await _voucherService.GetAllVoucher(request);

            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }


        /// <summary>
        /// CHORE : Retrieves a voucher by its ID.
        /// </summary>
        /// <param name="id">The ID of the voucher to retrieve.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /voucher/1
        /// </remarks>
        /// <response code="200">Get Voucher success</response>
        /// <response code="404">Voucher not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherById(int id)
        {
            var response = await _voucherService.GetVourcherById(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : User get voucher
        /// </summary>
        /// <param name="id">The ID of the voucher to retrieve.</param>
        /// <returns>An IActionResult containing the operation result.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /voucher/1
        /// </remarks>
        /// <response code="200">Get Voucher success</response>
        /// <response code="404">Voucher not found</response>
        /// <response code="500">Internal server error occurred</response>
        [HttpPost("{id}")]
        public async Task<IActionResult> UserGetVoucherById(int id)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountIdClaim = claims.FirstOrDefault(x => x.Type.Equals("sid", StringComparison.OrdinalIgnoreCase));

            // Check if the claim is null or if the value cannot be parsed to an integer
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int userId))
            {
                return Unauthorized(new { Message = MessageConstant.FailMessage.UserIdInvalid });
            }
            var response = await _voucherService.AssignVoucherToUser(id, userId);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Creates a new voucher.
        /// </summary>
        /// <param name="request">The details of the voucher to create.</param>
        /// <returns>The created voucher's information.</returns>
        /// <remarks>
        /// Example request:
        ///
        ///     POST /api/vouchers
        ///     {
        ///         "PromotionCategoryId": 1,
        ///         "Price": 10.0,
        ///         "Code": "DISCOUNT2024"
        ///     }
        /// </remarks>
        /// <response code="201">Voucher created successfully</response>
        /// <response code="400">Invalid input or missing required fields</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("")]
        public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherRequest request)
        {
            var response = await _voucherService.CreateVoucher(request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Updates an existing voucher.
        /// </summary>
        /// <param name="id">The ID of the voucher to update.</param>
        /// <param name="request">The updated voucher details.</param>
        /// <returns>The updated voucher's information.</returns>
        /// <remarks>
        /// Example request:
        ///
        ///     PUT /api/vouchers/{id}
        ///     {
        ///         "Price": 15.0,
        ///         "Code": "UPDATED2024"
        ///     }
        /// </remarks>
        /// <response code="200">Voucher updated successfully</response>
        /// <response code="404">Voucher not found</response>
        /// <response code="400">Invalid input or missing required fields</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVoucher(int id, [FromBody] CreateVoucherRequest request)
        {
            var response = await _voucherService.UpdateVoucher(id, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// CHORE : Set voucher's IsDeleted become true by voucher Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            var response = await _voucherService.DeleteVouver(id);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}
