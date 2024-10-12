using Microsoft.AspNetCore.Http;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using MoveMate.Service.ThirdPartyService.Payment.VNPay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Payment.VNPay
{
    public interface IVnPayService
    {

        Task<OperationResult<RechagreResponseModel>> RechagreExecute(IQueryCollection collections);
        Task<OperationResult<string>> Recharge(HttpContext context, int userId, double amount, string returnUrl);
        Task<OperationResult<Transaction>> RechagrePayment(RechagreResponseModel response);
        public Task<OperationResult<string>> CreatePaymentAsync(int bookingId, int userId, string returnUrl);


    }
}
