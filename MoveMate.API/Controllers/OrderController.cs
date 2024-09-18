using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.ThirdPartyService.PayOs;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    [ApiController]
   
    public class OrderController : BaseController
    {
        private readonly PayOS _payOS;

        public OrderController(PayOS payOS)
        {
            _payOS = payOS;
        }

        [HttpPost("create")]
        public IActionResult CreatePaymentLink([FromBody] CreatePaymentLinkRequestBody requestBody)
        {
            try
            {
                var productName = requestBody.ProductName;
                var description = requestBody.Description;
                var returnUrl = requestBody.ReturnUrl;
                var cancelUrl = requestBody.CancelUrl;
                var price = requestBody.Price;

                var orderCode = DateTime.Now.Ticks.ToString().Substring(6);

                var item = new ItemData { Name = productName, Price = price, Quantity = 1 };
                var paymentData = new PaymentData
                {
                    OrderCode = orderCode,
                    Description = description,
                    Amount = price,
                    Item = item,
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                };

                var data = _payOS.CreatePaymentLink(paymentData);

                return Ok(new { error = 0, message = "success", data });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = -1, message = "fail", data = "" });
            }
        }

        [HttpGet("{orderId}")]
        public IActionResult GetOrderById(long orderId)
        {
            try
            {
                var order = _payOS.GetPaymentLinkInformation(orderId);
                return Ok(new { error = 0, message = "ok", data = order });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = -1, message = e.Message, data = "" });
            }
        }

        [HttpPut("{orderId}")]
        public IActionResult CancelOrder(int orderId)
        {
            try
            {
                var order = _payOS.CancelPaymentLink(orderId, null);
                return Ok(new { error = 0, message = "ok", data = order });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = -1, message = e.Message, data = "" });
            }
        }

        [HttpPost("confirm-webhook")]
        public IActionResult ConfirmWebhook([FromBody] Dictionary<string, string> requestBody)
        {
            try
            {
                var webhookUrl = requestBody["webhookUrl"];
                var str = _payOS.ConfirmWebhook(webhookUrl);
                return Ok(new { error = 0, message = "ok", data = str });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = -1, message = e.Message, data = "" });
            }
        }
    }
}
