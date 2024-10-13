using Microsoft.AspNetCore.Http;
using MoveMate.Service.Commons;
using MoveMate.Service.ThirdPartyService.Payment.Momo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Payment.Momo
{
    public interface IMomoPaymentService
    {
        public Task<OperationResult<string>> CreatePaymentWithMomoAsync(int bookingId, int userId, string returnUrl);
        public Task<OperationResult<string>> AddFundsToWalletAsync(int userId, double amount, string returnUrl);
        public Task<OperationResult<string>> HandleWalletPaymentAsync(HttpContext context, MomoPaymentCallbackCommand command);
    }
}
