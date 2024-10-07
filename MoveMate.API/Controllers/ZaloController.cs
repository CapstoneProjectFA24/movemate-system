using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.ThirdPartyService.Zalo;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MoveMate.Service.ThirdPartyService.Zalo.Models;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class ZaloController : BaseController
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly ZaloPaySDK _paySdk;

        public ZaloController(ILogger<PaymentController> logger, ZaloPaySDK paySdk)
        {
            _logger = logger;
            _paySdk = paySdk;
        }

        // POST: api/payment/create-order
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrder createOrder)
        {
            var result = await _paySdk.CreateOrder(createOrder.BankCode);

            if (result is { ReturnCode: 1 })
            {
                // Return the order URL and additional info as a JSON response
                return Ok(new { orderUrl = result.OrderUrl, result });
            }

            _logger.LogError("Error creating order.");
            return BadRequest("Failed to create order.");
        }

        // GET: api/payment/query-order/{appTransId}
        [HttpGet("query-order/{appTransId}")]
        public async Task<IActionResult> QueryOrder(string appTransId)
        {
            var result = await _paySdk.QueryOrder(appTransId);
            if (result == null)
            {
                return NotFound("Order not found.");
            }

            return Ok(result);
        }

        // GET: api/payment/redirect
        [HttpGet("redirect")]
        public async Task<IActionResult> Redirect([FromQuery] RedirectOrder redirectOrder)
        {
            if (!_paySdk.ChecksumData(redirectOrder))
            {
                return BadRequest("Invalid checksum.");
            }

            var result = await _paySdk.QueryOrder(redirectOrder.Apptransid);
            if (result == null)
            {
                return NotFound("Order not found.");
            }

            return Ok(result);
        }

        // POST: api/payment/callback
        [HttpPost("callback")]
        public IActionResult Callback([FromBody] CallbackOrder callbackOrder)
        {
            if (!_paySdk.CheckMac(callbackOrder))
            {
                return BadRequest("Invalid MAC.");
            }

            // Handle successful callback logic
            return Ok("Callback successful.");
        }

        // Handle any errors
        [HttpGet("error")]
        public IActionResult Error()
        {
            return Problem("An error occurred. Please try again later.");
        }
    }
}
