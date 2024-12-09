using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.ThirdPartyService.Payment.Models;
using MoveMate.Service.Utils;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Cloud.Firestore.V1.StructuredAggregationQuery.Types.Aggregation.Types;

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

        public async Task<OperationResult<WalletResponse>> UpdateWallet(int userId, UpdateWalletRequest request)
        {
            var result = new OperationResult<WalletResponse>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                    return result;
                }

                var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userId);
                if (wallet == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                    return result;
                }

                ReflectionUtils.UpdateProperties(request, wallet);
                wallet.IsLocked = false;
                await _unitOfWork.WalletRepository.SaveOrUpdateAsync(wallet);
                await _unitOfWork.SaveChangesAsync();

                wallet = await _unitOfWork.WalletRepository.GetByIdAsync(wallet.Id);

                var response = _mapper.Map<WalletResponse>(wallet);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.UpdateWalletSuccess,
                        response);
                return result;

            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        public async Task<OperationResult<bool>> CheckBalance(int userId, double amount)
        {
            var result = new OperationResult<bool>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, includeProperties: "Wallet");
                if (user == null)
                {
                    result.AddResponseErrorStatusCode(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser, false);
                    return result;
                }

                var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userId);
                if (wallet == null)
                {
                    result.AddResponseErrorStatusCode(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet, false);
                    return result;
                }

                if (wallet.Balance < amount)
                {
                    result.AddResponseErrorStatusCode(StatusCode.Ok, MessageConstant.FailMessage.NotEnoughMoney, false);
                    return result;
                }
                else
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.EnoughMoney, true);
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        public async Task<OperationResult<WalletWithDrawResponse>> UserRequestWithDraw(int userId, double amount)
        {
            var result = new OperationResult<WalletWithDrawResponse>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                    return result;
                }
                var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userId);
                if(wallet == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                    return result;
                }
                if (wallet.Balance < amount)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotEnoughMoney);
                    return result;
                }
                var withdrawal = new Withdrawal();

                withdrawal.UserId = userId;
                withdrawal.Amount = amount;
                withdrawal.WalletId = wallet.Id;
                withdrawal.BalanceBefore = wallet.Balance;
                withdrawal.BalanceAfter = wallet.Balance - amount;
                withdrawal.Date = DateTime.Now;
                withdrawal.BankName = wallet.BankName;
                withdrawal.BankNumber = wallet.BankNumber;
                withdrawal.IsSuccess = false;
                withdrawal.IsCancel = false;
                
                await _unitOfWork.WithdrawalRepository.AddAsync(withdrawal);
                await _unitOfWork.SaveChangesAsync();
                withdrawal = await _unitOfWork.WithdrawalRepository.GetByIdAsync(withdrawal.Id);
                var response = _mapper.Map<WalletWithDrawResponse>(withdrawal);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.WithDrawMoney, response); 
                return result;
            }
            catch(Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        public async Task<OperationResult<WalletWithDrawResponse>> UserCancelRequestWithDraw(int withdrawId, int userId, UserCancelRequestWithDrawRequest request)
        {
            var result = new OperationResult<WalletWithDrawResponse>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                    return result;
                }
                var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userId);
                if (wallet == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                    return result;
                }
                var withdraw = await _unitOfWork.WithdrawalRepository.GetByIdAsync(withdrawId);
                if (withdraw == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWithDraw);
                    return result;
                }
                if (withdraw.UserId != userId)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.WithdrawNotFromUser);
                    return result;
                }
                if (withdraw.IsSuccess == true)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.WithdrawSuccess);
                    return result;
                }
                withdraw.IsCancel = true;
                withdraw.CancelReason = request.CancelReason;
                await _unitOfWork.WithdrawalRepository.SaveOrUpdateAsync(withdraw);
                await _unitOfWork.SaveChangesAsync();
                withdraw = await _unitOfWork.WithdrawalRepository.GetByIdAsync(withdrawId);
                var response = _mapper.Map<WalletWithDrawResponse>(withdraw);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.CancelWithDrawMoney, response);
                return result;

            }
            catch(Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        public async Task<OperationResult<WalletWithDrawResponse>> ManagerDeniedRequestWithDraw(int withdrawId, UserCancelRequestWithDrawRequest request)
        {
            var result = new OperationResult<WalletWithDrawResponse>();
            try
            {               
                
                var withdraw = await _unitOfWork.WithdrawalRepository.GetByIdAsync(withdrawId);
                if (withdraw == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWithDraw);
                    return result;
                }
                
                if (withdraw.IsSuccess == true)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.WithdrawSuccess);
                    return result;
                }
                withdraw.IsCancel = true;
                withdraw.CancelReason = request.CancelReason;
                await _unitOfWork.WithdrawalRepository.SaveOrUpdateAsync(withdraw);
                await _unitOfWork.SaveChangesAsync();
                withdraw = await _unitOfWork.WithdrawalRepository.GetByIdAsync(withdrawId);
                var response = _mapper.Map<WalletWithDrawResponse>(withdraw);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.CancelWithDrawMoney, response);
                return result;

            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        public async Task<OperationResult<WalletWithDrawResponse>> ManagerAccpectRequestWithDraw(int withdrawId)
        {
            var result = new OperationResult<WalletWithDrawResponse>();
            try
            {

                var withdraw = await _unitOfWork.WithdrawalRepository.GetByIdAsync(withdrawId);
                if (withdraw == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWithDraw);
                    return result;
                }
                if (withdraw.IsCancel == true)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.WithdrawCancel);
                    return result;
                }
                var wallet = await _unitOfWork.WalletRepository.GetByIdAsync((int)withdraw.WalletId);
                if (wallet == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                    return result;
                }
                if (wallet.Balance < withdraw.Amount)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotEnoughMoney);
                    return result;
                }
                withdraw.IsSuccess = true;

                var newGuid = Guid.NewGuid();
                var transaction = new MoveMate.Domain.Models.Transaction
                {                  
                    WalletId = wallet.Id,
                    Amount = withdraw.Amount,
                    Status = PaymentEnum.SUCCESS.ToString(),
                    TransactionType = PaymentMethod.WITHDRAW.ToString(),
                    TransactionCode = newGuid.ToString(),
                    CreatedAt = DateTime.Now,
                    Resource = Resource.Wallet.ToString(),
                    PaymentMethod = Resource.Wallet.ToString(),
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    IsCredit = false
                };
                wallet.Balance -= withdraw.Amount;

                var updateWallet = await UpdateWalletBalance(wallet.Id, (float)wallet.Balance);
                if (updateWallet.IsError)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateWalletBalance);
                    return result;
                }
                await _unitOfWork.WithdrawalRepository.SaveOrUpdateAsync(withdraw);
                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();
                withdraw = await _unitOfWork.WithdrawalRepository.GetByIdAsync(withdrawId);
                var response = _mapper.Map<WalletWithDrawResponse>(withdraw);
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.WithDrawMoneySuccess, response);
                return result;

            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

        public async Task<OperationResult<List<WalletWithDrawResponse>>> GetAllWithDraw(GetAllWithDrawalRequest request)
        {
            var result = new OperationResult<List<WalletWithDrawResponse>>();

            var pagin = new Pagination();

            var filter = request.GetExpressions();

            try
            {
                var entities = _unitOfWork.WithdrawalRepository.GetWithCount(
                filter: request.GetExpressions(),
                pageIndex: request.page,
                pageSize: request.per_page,
                orderBy: request.GetOrder()
            );
                var listResponse = _mapper.Map<List<WalletWithDrawResponse>>(entities.Data);

                if (listResponse == null || !listResponse.Any())
                {
                    result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListWithDrawalEmpty, listResponse);
                    return result;
                }

                pagin.pageSize = request.per_page;
                pagin.totalItemsCount = entities.Count;

                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.GetListWithDrawalSuccess, listResponse, pagin);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred in getAll Service Method");
                throw;
            }
        }
    }
}