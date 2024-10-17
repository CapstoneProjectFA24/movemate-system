using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MoveMate.Service.ViewModels.ModelRequests;
using Newtonsoft.Json;
using MoveMate.Service.ThirdPartyService.Payment.Zalo;
using MoveMate.Service.ThirdPartyService.Payment.Zalo.Models;

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
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest createOrder)
        {
            var paymentLink =
                await _paySdk.GeneratePaymentLink(createOrder.OrderId, createOrder.Amount, createOrder.BankCode);

            if (!string.IsNullOrEmpty(paymentLink))
            {
                _logger.LogInformation("Payment link generated: {Link}", paymentLink);
                return Ok(new { paymentLink });
            }

            _logger.LogError("Failed to create order.");
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