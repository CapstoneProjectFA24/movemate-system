using MoveMate.Service.Commons;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MoveMate.Service.IServices
{
    public interface IPaymentServices
    {
    }
    
    Task<OperationResult<string>> CreatePaymentLinkAsync(int bookingId, int userId, string returnUrl);
    public Task<OperationResult<string>> CreateRechargeLinkAsync(int userId, double amount, string returnUrl);

    public Task<OperationResult<string>> HandleWalletPaymentAsync(HttpContext context,
        PayOsPaymentCallbackCommand command);

    public Task<OperationResult<string>> HandleOrderPaymentAsync(HttpContext context,
        PayOsPaymentCallbackCommand command);
}