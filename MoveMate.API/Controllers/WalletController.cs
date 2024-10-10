using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class WalletController : BaseController
    {
        private readonly IWalletServices _walletServices;

        public WalletController(IWalletServices walletServices)
        {
            _walletServices = walletServices;
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
    }

}
