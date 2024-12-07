using MoveMate.Domain.Models;
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
    public interface IWalletServices
    {
        Task<OperationResult<WalletResponse>> GetWalletByUserIdAsync(string userId);
        Task<OperationResult<WalletResponse>> UpdateWalletBalance(int walletId, float balance);
        Task<OperationResult<WalletResponse>> UpdateWallet(int userId, UpdateWalletRequest request);
        Task<OperationResult<bool>> CheckBalance(int userId, double amount);
        Task<OperationResult<WalletWithDrawResponse>> UserRequestWithDraw(int userId, double amount);
    }
}