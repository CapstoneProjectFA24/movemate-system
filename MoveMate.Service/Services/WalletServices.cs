﻿using AutoMapper;
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


        public async Task<OperationResult<WalletResponse>> GetWalletByUserIdAsync(string userId)
        {
            var result = new OperationResult<WalletResponse>();
            try
            {
                var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(int.Parse(userId));

                if (wallet != null)
                {
                    var walletResponse = _mapper.Map<WalletResponse>(wallet);
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetWalletSuccess, walletResponse);
                    return result;
                }
            
                else
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the wallet.");
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
            }

            return result;
        }

        public async Task<OperationResult<WalletResponse>> UpdateWalletBalance(int walletId, float balance)
        {
            var result = new OperationResult<WalletResponse>();
            try
            {
                var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(walletId);
                if (wallet == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                    return result;
                }

                wallet.Balance = balance;
                wallet.UpdatedAt = DateTime.Now;
                await _unitOfWork.WalletRepository.UpdateAsync(wallet);

                var walletResponse = _mapper.Map<WalletResponse>(wallet);
                return OperationResult<WalletResponse>.Success(walletResponse);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                return result;
            }
        }
    }
}