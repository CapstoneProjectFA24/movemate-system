﻿using Microsoft.AspNetCore.Http;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.VNPay
{
    public interface IVnPayService
    {
        //Task<string> CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        //Task<VnPaymentResponseModel> PaymentExecute(IQueryCollection collections);
        Task<OperationResult<RechagreResponseModel>> RechagreExecute(IQueryCollection collections);
        Task<OperationResult<string>> Recharge(HttpContext context, int userId, float amount);
        //Task<OperationResult<Transaction>> DepositPayment(VnPaymentResponseModel response);
        Task<OperationResult<Transaction>> RechagrePayment(RechagreResponseModel response);
        //Task<OperationResult<bool>> PayOrderWithWalletBalance(int orderId, int userId);
    }
}