using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface IMomoService
    {
        Task<(bool success, string payUrl)> CreatePaymentLink(
     string requestId,
     string orderId,
     string amount,
     string orderInfo,
     string extraData = "",
     string lang = "vi");

    }
}
