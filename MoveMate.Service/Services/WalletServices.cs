using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Utils;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class WalletServices : IWalletServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<WalletServices> _logger;
        public WalletServices(IUnitOfWork unitOfWork, IMapper mapper, ILogger<WalletServices> logger)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
        }

        public async Task<OperationResult<WalletResponse>> GetWalletByUserId(int id)
        {
            var result = new OperationResult<WalletResponse>();

            try
            {
                // Query the user by name from the database asynchronously
                var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(id);

                if (wallet == null)
                {
                    // Handle case where user is not found
                    result.AddError(StatusCode.NotFound, $"Wallet '{id}' not found.");
                    return result;
                }

                // Map the User entity to UserResponse view model
                var walletResponse = _mapper.Map<WalletResponse>(wallet);

                // Set payload and message for successful retrieval
                result.Payload = walletResponse;
                result.Message = "Wallet retrieved successfully.";

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                // Log or debug the error
                Console.WriteLine($"Error occurred: {error}");

                // Add error to result
                result.AddError(StatusCode.ServerError,  error);
                return result;
            }
        }

        public async Task<OperationResult<Wallet>> GetWalletsByUserId(int id)
        {
            var result = new OperationResult<Wallet>();

            try
            {
                // Query the user by name from the database asynchronously
                var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(id);

                if (wallet == null)
                {
                    // Handle case where user is not found
                    result.AddError(StatusCode.NotFound, $"Wallet '{id}' not found.");
                    return result;
                }

                // Map the User entity to UserResponse view model
                var walletResponse = _mapper.Map<Wallet>(wallet);

                // Set payload and message for successful retrieval
                result.Payload = walletResponse;
                result.Message = "Wallet retrieved successfully.";

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                // Log or debug the error
                Console.WriteLine($"Error occurred: {error}");

                // Add error to result
                result.AddError(StatusCode.ServerError, error);
                return result;
            }
        }
        public async Task<OperationResult<WalletResponse>> UpdateWalletBalance(int walletId, float balance)
        {
            var result = new OperationResult<WalletResponse>();
            try
            {
                var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(walletId);
                if (wallet == null)
                {
                    result.AddError(StatusCode.NotFound, $"Wallet '{walletId}' not found.");
                    return result;
                }

                wallet.Balance = balance;
                wallet.UpdatedAt = DateTime.Now;
                await _unitOfWork.WalletRepository.UpdateAsync(wallet);

                var walletResponse = _mapper.Map<WalletResponse>(wallet); // Mapping nếu sử dụng AutoMapper
                return OperationResult<WalletResponse>.Success(walletResponse);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.NotFound, $"Wallet");
                return result;
            }
        }

    }
}
