using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelResponses;

namespace MoveMate.Service.IServices
{
    public interface IPaymentServices
    {
        Task<OperationResult<bool>> PaymentByWallet(int userId, int bookingId, string returnUrl);
    }
    
}