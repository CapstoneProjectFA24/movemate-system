using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.PayOs
{
    public class ConfirmWebhookRequestBody
    {
        public string WebhookUrl { get; set; }

        public ConfirmWebhookRequestBody(string webhookUrl)
        {
            WebhookUrl = webhookUrl;
        }
    }
}
