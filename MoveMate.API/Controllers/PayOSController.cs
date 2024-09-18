using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using MoveMate.Service.ThirdPartyService.PayOs;
using Newtonsoft.Json;
using static MoveMate.Service.ThirdPartyService.PayOs.Tyoe;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class PayOSController : BaseController
    {
        private readonly PayOS _payOS;

        public PayOSController(PayOS payOS)
        {
            _payOS = payOS;
        }

        [HttpPost("payos_transfer_handler")]
        public IActionResult PayosTransferHandler([FromBody] object body)
        {
            try
            {
                var webhookBody = JsonConvert.DeserializeObject<Webhook>(body.ToString());
                var data = _payOS.VerifyPaymentWebhookData(webhookBody);
                Console.WriteLine(data);
                return Ok(new { error = 0, message = "Webhook delivered", data });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = -1, message = e.Message, data = "" });
            }
        }
    }
}
