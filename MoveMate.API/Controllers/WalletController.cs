using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ThirdPartyService.Payment.Models;
using MoveMate.Service.ThirdPartyService.Payment.Momo;
using MoveMate.Service.ThirdPartyService.Payment.PayOs;
using MoveMate.Service.ThirdPartyService.Payment.VNPay;
using MoveMate.Service.ThirdPartyService.Payment.Models;
using System.Security.Claims;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class WalletController : BaseController
    {
        private readonly IWalletServices _walletServices;
        private readonly IMomoPaymentService _momoPaymentService;
        private readonly IVnPayService _vpnPayService;
        private readonly IPayOsService _payOsService;

        public WalletController(IWalletServices walletServices, IMomoPaymentService momoPaymentService,
            IVnPayService vpnPayService, IPayOsService payOsService)
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
                return Unauthorized(new { Message = MessageConstant.FailMessage.UserIdInvalid });
            }

            var result = await _walletServices.GetWalletByUserIdAsync(userId);
            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            return Ok(result);
        }

        /// <summary>
        /// FEATURE : Adds funds to the user's wallet using the selected payment method.
        /// </summary>
        /// <param name="amount">The amount to add to the wallet.</param>
        /// <param name="returnUrl">The URL to return to after payment.</param>
        /// <param name="paymentMethod">The selected payment method ("momo", "payos", "vpnpay").</param>
        /// <returns>Returns the result of adding funds to the wallet.</returns>
        /// <response code="200">Funds added successfully</response>
        /// <response code="400">Invalid amount or payment method provided</response>
        /// <response code="401">Invalid user ID in token</response>
        /// <response code="500">An internal server error occurred</response>
        /// <remarks>
        /// Example payment method details:
        /// 
        /// - **Momo**: This method requires a user to have a Momo account linked. After selecting this option, you will be redirected to the Momo payment page for authorization.
        /// - **PayOS**: A secure link will be generated, redirecting the user to complete the transaction with PayOS.
        /// - **VnPay**: Requires a valid Vietnamese bank account. Upon selection, you will be redirected to VnPay's page for completing the payment.
        /// 
        /// For testing purposes, example payment details can be used:
        /// 
        /// **Momo**:
        /// - Card Number: 9704 0000 0000 0018
        /// - Cardholder Name: NGUYEN VAN A
        /// - Expiry Date: 03/07
        /// - OTP: Provided upon transaction
        /// 
        /// **VnPay**:
        /// - Card Number: 9704198526191432198
        /// - Cardholder Name: NGUYEN VAN A
        /// - Issue Date: 07/15
        /// - OTP: 123456
        /// 
        /// The `returnUrl` should be set to: https://movemate-dashboard.vercel.app/payment-status
        /// </remarks>
        [HttpPost("recharge")]
        [Authorize]
        public async Task<IActionResult> AddFundsToWallet(double amount, string returnUrl, string selectedMethod)
        {
            // Extract user ID from claims
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { Message = MessageConstant.FailMessage.UserIdInvalid });
            }

            var userId = int.Parse(accountIdClaim.Value);
            OperationResult<string> result;

            // Attempt to parse the paymentMethod string to the PaymentMethod enum
            if (!Enum.TryParse<MoveMate.Service.ThirdPartyService.Payment.Models.PaymentType>(selectedMethod, true,
                    out var parsedPaymentMethod))
            {
                return BadRequest(new { Message = MessageConstant.FailMessage.PaymentMethod });
            }

            // Call the appropriate service based on the payment method
            switch (parsedPaymentMethod)
            {
                case MoveMate.Service.ThirdPartyService.Payment.Models.PaymentType.Momo:
                    result = await _momoPaymentService.AddFundsToWalletAsync(userId, amount, returnUrl);
                    break;

                case MoveMate.Service.ThirdPartyService.Payment.Models.PaymentType.PayOS:
                    result = await _payOsService.CreateRechargeLinkAsync(userId, amount, returnUrl);
                    break;

                case MoveMate.Service.ThirdPartyService.Payment.Models.PaymentType.VnPay:
                    result = await _vpnPayService.Recharge(HttpContext, userId, amount, returnUrl);
                    break;

                default:
                    return BadRequest(new { Message = MessageConstant.FailMessage.UnsupportPayment });
            }

            // Check for errors in the result
            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }

            return Ok(result);
        }

        /// <summary>
        /// FEATURE: Update wallet information
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut("")]
        public async Task<IActionResult> UpdateWallet(UpdateWalletRequest request)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);
            var response = await _walletServices.UpdateWallet(userId, request);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: Check wallet balance 
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> CheckWallet([FromQuery]double amount)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);
            var response = await _walletServices.CheckBalance(userId, amount);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }

        /// <summary>
        /// FEATURE: User request withdraw
        /// </summary>
        /// <returns></returns>
        [HttpPost("with-draw")]
        public async Task<IActionResult> UserRequestWithdraw([FromQuery] double amount)
        {
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.First(x => x.Type.ToLower().Equals("sid"));
            var userId = int.Parse(accountId.Value);
            var response = await _walletServices.UserRequestWithDraw(userId, amount);
            return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
        }
    }
}