using MoveMate.Service.Commons;
using MoveMate.Service.ThirdPartyService.Momo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Momo
{
    public interface IMomoPaymentService
    {
        public Task<OperationResult<string>> CreatePaymentWithMomoAsync(int bookingId, int userId, string returnUrl);
    }
}
