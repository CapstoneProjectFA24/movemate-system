using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.ThirdPartyService.PayOs;
using MoveMate.Service.ViewModels.ModelRequests;

namespace MoveMate.API.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly PayOS _payOS;

        public CheckoutController(PayOS payOS)
        {
            _payOS = payOS;
        }

        public IActionResult Index()
        {
            return View("index");
        }

        public IActionResult Success()
        {
            return View("success");
        }

        public IActionResult Cancel()
        {
            return View("cancel");
        }

        [HttpPost("create-payment-link")]
        public async Task<IActionResult> Checkout([FromForm] CreatePaymentLinkRequestBody requestBody)
        {
            try
            {
                var baseUrl = GetBaseUrl(Request);
                var productName = "Mì tôm hảo hảo ly";
                var description = "Thanh toan don hang";
                var returnUrl = $"{baseUrl}/success";
                var cancelUrl = $"{baseUrl}/cancel";
                var price = 2000;

                var orderCode = DateTime.Now.Ticks.ToString().Substring(6);

                var item = new ItemData { Name = productName, Quantity = 1, Price = price };
                var paymentData = new PaymentData
                {
                    OrderCode = orderCode,
                    Amount = price,
                    Description = description,
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    Item = item
                };

                var checkoutUrl = await _payOS.CreatePaymentLink(paymentData);

                return Redirect(checkoutUrl);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private string GetBaseUrl(HttpRequest request)
        {
            var scheme = request.Scheme;
            var serverName = request.Host.Value;
            var serverPort = request.Host.Port ?? (scheme == "https" ? 443 : 80);
            var contextPath = request.PathBase.Value;

            var url = $"{scheme}://{serverName}";
            if (serverPort != 80 && serverPort != 443)
            {
                url += $":{serverPort}";
            }
            url += contextPath;
            return url;
        }
    }


}
