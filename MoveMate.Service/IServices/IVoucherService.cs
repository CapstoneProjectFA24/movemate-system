using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface IVoucherService
    {
        public Task<OperationResult<List<VoucherResponse>>> GetAllVoucher(GetAllVoucherRequest request);
        public Task<OperationResult<VoucherResponse>> GetVourcherById(int id);

        Task<OperationResult<VoucherResponse>> AssignVoucherToUser(int voucherId, int userId);
        Task<OperationResult<VoucherResponse>> UpdateVoucher(int id, CreateVoucherRequest request);
        Task<OperationResult<VoucherResponse>> CreateVoucher(CreateVoucherRequest request);
        Task<OperationResult<bool>> DeleteVouver(int id);
    }
}
