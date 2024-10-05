using MoveMate.Service.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.PayOs
{
    public interface IPayOsService
    {
        Task<OperationResult<string>> CreatePaymentLinkAsync(int bookingId, int userId);
    }
}
