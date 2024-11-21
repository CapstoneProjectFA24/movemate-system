using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelResponses;

namespace MoveMate.Service.IServices
{
    public interface IPaymentServices
    {
        Task<OperationResult<string>> PaymentByWallet(int userId, int bookingId, string returnUrl);
        Task<OperationResult<bool>> UserPayByCash(int userId, int bookingId);
    }
    
}