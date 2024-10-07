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
        /// Get Wallet by UserID 
        /// </summary>
        /// <returns></returns>
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
