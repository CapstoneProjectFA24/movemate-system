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
        Task<OperationResult<PaymentData>> CreatePaymentBooking(int userId, int bookingId, int scheduleDetailId);
    }
}
