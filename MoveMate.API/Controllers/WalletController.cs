using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ThirdPartyService.Payment.Momo;
using MoveMate.Service.ThirdPartyService.Payment.PayOs;
using MoveMate.Service.ThirdPartyService.Payment.VNPay;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class WalletController : BaseController
    {
        private readonly IWalletServices _walletServices;
        private readonly IMomoPaymentService _momoPaymentService;
        private readonly IVnPayService _vpnPayService;
        private readonly IPayOsService _payOsService;
        public WalletController(IWalletServices walletServices, IMomoPaymentService momoPaymentService, IVnPayService vpnPayService, IPayOsService payOsService)
        {
            _walletServices = walletServices;
            _momoPaymentService = momoPaymentService;
            _vpnPayService = vpnPayService;
            _payOsService = payOsService;
        }

        /// <summary>
        /// FEATURE : Retrieves the wallet balance for the authenticated user by token
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the wallet information if successful, 
        /// or an error response if the user is not authenticated or if an error occurs during retrieval.
        /// </returns>
        /// <remarks>
        /// This endpoint requires user authentication and retrieves the wallet balance 
        /// based on the user ID extracted from the JWT token claims.
        /// 
        /// Sample request:
        /// 
        ///     GET /wallet/balance
        /// </remarks>
        /// <response code="200">Wallet retrieved successfully</response>
        /// <response code="404">Wallet not found</response>
        /// <response code="500">An unexpected error occurred</response>
        [HttpGet("balance")]
        [Authorize]
        public async Task<IActionResult> GetWalletByUserIdAsync()
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value).ToString();

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Invalid user ID in token." });
            }

            var result = await _walletServices.GetWalletByUserIdAsync(userId);
            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            return Ok(result);
        }


        /// <summary>
        /// Adds funds to the user's wallet.
        /// </summary>
        /// <param name="amount">The amount to add to the wallet.</param>
        /// <returns>Returns the result of adding funds to the wallet.</returns>
        /// <response code="200">Funds added successfully</response>
        /// <response code="400">Invalid amount provided</response>
        /// <response code="401">Invalid user ID in token</response>
        /// <response code="500">An internal server error occurred</response>
        [HttpPost("momo/recharge")]
        [Authorize]
        public async Task<IActionResult> AddFundsToWallet([FromQuery] double amount, string returnUrl)
        {
            // Extract user ID from claims
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { Message = "Invalid user ID in token." });
            }

            var userId = int.Parse(accountIdClaim.Value);

            // Call the Momo service method to add funds to the wallet
            var result = await _momoPaymentService.AddFundsToWalletAsync(userId, amount, returnUrl);

            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            return Ok(result);
        }


        /// <summary>
        /// Adds funds to the user's wallet.
        /// </summary>
        /// <param name="amount">The amount to add to the wallet.</param>
        /// <returns>Returns the result of adding funds to the wallet.</returns>
        /// <response code="200">Funds added successfully</response>
        /// <response code="400">Invalid amount provided</response>
        /// <response code="401">Invalid user ID in token</response>
        /// <response code="500">An internal server error occurred</response>
        [HttpPost("payOs/recharge")]
        [Authorize]
        public async Task<IActionResult> AddFundsToWalletPayOS([FromQuery] double amount, string returnUrl)
        {
            // Extract user ID from claims
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { Message = "Invalid user ID in token." });
            }

            var userId = int.Parse(accountIdClaim.Value);

            // Call the Momo service method to add funds to the wallet
            var result = await _payOsService.CreateRechargeLinkAsync(userId, amount, returnUrl);

            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            return Ok(result);
        }

        /// <summary>
        /// FEATURE : Recharge money into wallet.
        /// </summary>
        /// <param name="model">Customer recharge money into wallet in system </param>
        /// <returns>Returns the result of wallet</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "amount": 666666
        ///     }   
        /// </remarks>
        /// <response code="200">Payment URL generated successfully</response>
        /// <response code="404">User not found</response>
        /// <response code="404">Wallet not found</response>
        /// <response code="500">An internal server error occurred</response>
        [HttpPost("create-recharge-payment-url")]
        [Authorize]
        public async Task<IActionResult> Recharge([FromQuery] double amount, string returnUrl)
        {
            var accountId = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));

            if (accountId == null || string.IsNullOrEmpty(accountId.Value))
            {
                return Unauthorized(new { Message = "Invalid user ID in token." });
            }

            var userId = int.Parse(accountId.Value);
            var result = await _vpnPayService.Recharge(HttpContext, userId, amount, returnUrl);

            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }
            return Ok(result);
        }

    }

}
