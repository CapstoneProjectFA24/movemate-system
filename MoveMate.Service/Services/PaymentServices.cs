﻿using MoveMate.Service.IServices;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Repository.Repositories.UnitOfWork;
using Net.payOS.Types;
using MoveMate.Service.Commons;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Service.ViewModels.ModelResponses;
using MoveMate.Domain.Enums;
using MoveMate.Service.ThirdPartyService.Payment.Models;
using MoveMate.Service.ThirdPartyService.Firebase;
using MoveMate.Domain.Models;
using MoveMate.Service.Library;
using Parlot.Fluent;
using MoveMate.Service.ViewModels.ModelRequests;
using MoveMate.Service.Utils;
using MoveMate.Service.ThirdPartyService.RabbitMQ.DTO;
using MoveMate.Service.ThirdPartyService.RabbitMQ;

namespace MoveMate.Service.Services
{
    public class PaymentService : IPaymentServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;
        private readonly IFirebaseServices _firebaseServices;
        private readonly IWalletServices _walletService;
        private readonly IMessageProducer _producer;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PaymentService> logger, IFirebaseServices firebaseServices, IWalletServices walletService, IMessageProducer producer)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
            this._firebaseServices = firebaseServices;
            _walletService = walletService;
            _producer = producer;
        }

        public async Task<OperationResult<string>> PaymentByWallet(int userId, int bookingId, string returnUrl)
        {
            var result = new OperationResult<string>();
            try
            {

                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundUser);
                    return result;
                }
                var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, userId);
                if (booking == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.BookingCannotPay);
                    return result;
                }
                var wallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userId);
                if (wallet == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                    return result;
                }
                if (wallet.IsLocked == true)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.WalletLocked);
                    return result;
                }
                var assignmentDriver = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.DRIVER.ToString(), bookingId);
                var assignmentPorter = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.PORTER.ToString(), bookingId);


                if (assignmentPorter == null)
                {
                    if (booking.Status == BookingEnums.DEPOSITING.ToString())
                    {
                        //go to
                    }
                    else if (booking.Status == BookingEnums.IN_PROGRESS.ToString() &&
                             assignmentDriver.Status == AssignmentStatusEnums.COMPLETED.ToString())
                    {
                        //go to
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingStatus);
                        return result;
                    }
                }
                else
                {
                    if (booking.Status == BookingEnums.DEPOSITING.ToString())
                    {
                        //go to
                    }
                    else if (booking.Status == BookingEnums.IN_PROGRESS.ToString() &&
                             assignmentDriver.Status == AssignmentStatusEnums.COMPLETED.ToString() &&
                             assignmentPorter.Status == AssignmentStatusEnums.COMPLETED.ToString())
                    {
                        //go to
                    }
                    else
                    {
                        result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.BookingStatus);
                        return result;
                    }
                }


                int amount = 0;
                if (booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    if (wallet.Balance < booking.Deposit)
                    {
                        result.AddError(StatusCode.BadRequest, $"{returnUrl}?isSuccess=false&message={MessageConstant.FailMessage.BalanceNotEnough}");
                        return result;
                    }
                    else
                    {
                        amount = (int)booking.Deposit;
                    }
                }
                else if (booking.Status == BookingEnums.IN_PROGRESS.ToString())
                {
                    if (wallet.Balance < booking.TotalReal)
                    {
                        result.AddError(StatusCode.BadRequest, $"{returnUrl}?isSuccess=false&message={MessageConstant.FailMessage.BalanceNotEnough}");
                        return result;
                    }
                    else
                    {
                        amount = (int)booking.TotalReal;
                    }
                }

                wallet.Balance -= amount;

                var updateWallet = await _walletService.UpdateWalletBalance(wallet.Id, (float)wallet.Balance);
                if (updateWallet.IsError)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateWalletBalance);
                    return result;
                }



                var payment = new Domain.Models.Payment
                {
                    BookingId = bookingId,
                    Amount = amount,
                    Success = true,
                    Date = DateTime.Now
                };
                string category = "";
                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();
                string transType = "";
                if (booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    transType = Domain.Enums.PaymentMethod.DEPOSIT.ToString();
                    booking.TotalReal = booking.Total - amount;
                    category = CategoryEnums.DEPOSIT.ToString();
                }
                else if (booking.Status == BookingEnums.IN_PROGRESS.ToString())
                {
                    transType = Domain.Enums.PaymentMethod.PAYMENT.ToString();
                    booking.TotalReal -= amount;
                    category = CategoryEnums.PAYMENT_TOTAL.ToString();
                }




                var newGuid = Guid.NewGuid();
                var transaction = new MoveMate.Domain.Models.Transaction
                {
                    PaymentId = payment.Id,
                    WalletId = wallet.Id,
                    Amount = amount,
                    Status = PaymentEnum.SUCCESS.ToString(),
                    TransactionType = transType,
                    TransactionCode = newGuid.ToString(),
                    CreatedAt = DateTime.Now,
                    Resource = Resource.Wallet.ToString(),
                    PaymentMethod = Resource.Wallet.ToString(),
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    IsCredit = false
                };

                var userWithRoleId6 = await _unitOfWork.UserRepository.GetUserByRoleIdAsync();
                if (userWithRoleId6 != null)
                {
                    var walletManager = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userWithRoleId6.Id);
                    if (walletManager != null && walletManager.Tier == 0)
                    {
                        var additionalTransaction = new MoveMate.Domain.Models.Transaction
                        {
                            PaymentId = payment.Id,
                            WalletId = walletManager.Id,
                            Amount = amount,
                            Status = PaymentEnum.SUCCESS.ToString(),
                            TransactionType = Domain.Enums.PaymentMethod.RECEIVE.ToString(),
                            TransactionCode = "R" + Utilss.RandomString(7),
                            CreatedAt = DateTime.Now,
                            Resource = Resource.Wallet.ToString(),
                            PaymentMethod = Resource.Wallet.ToString(),
                            IsDeleted = false,
                            UpdatedAt = DateTime.Now,
                            IsCredit = true
                        };

                        await _unitOfWork.TransactionRepository.AddAsync(additionalTransaction);

                        // Update manager wallet balance
                        walletManager.Balance += amount;

                        var updateResult = await _walletService.UpdateWalletBalance(walletManager.Id, (float)walletManager.Balance);
                        if (updateResult.IsError)
                        {
                            result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateWalletBalance);
                            return result;
                        }
                    }
                }
                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                if (booking.IsReviewOnline == false && booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    booking.Status = BookingEnums.REVIEWING.ToString();
                    booking.IsDeposited = true;
                }
                else if (booking.IsReviewOnline == true && booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    booking.Status = BookingEnums.COMING.ToString();
                    booking.IsDeposited = true;
                }
                else if (booking.Status == BookingEnums.IN_PROGRESS.ToString() && booking.IsDeposited == true)
                {
                    booking.Status = BookingEnums.COMPLETED.ToString();
                }
                else
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.CanNotUpdateStatus);
                    return result;
                }

                _unitOfWork.BookingRepository.Update(booking);
                await _unitOfWork.SaveChangesAsync();
                var updatedBooking = await _unitOfWork.BookingRepository.GetByIdAsyncV1(bookingId, includeProperties:
                "BookingTrackers.TrackerSources,BookingDetails.Service,FeeDetails,Assignments,Vouchers");
                await _firebaseServices.SaveBooking(updatedBooking, updatedBooking.Id, "bookings");
                var url = $"{returnUrl}?isSuccess=true&amount={amount}&payDate={DateTime.Now}&bookingId={bookingId}&transactionCode={transaction.TransactionCode}&userId={userId}&paymentMethod={Resource.Wallet}&category={category}";
                result.AddResponseStatusCode(StatusCode.Ok, url, MessageConstant.SuccessMessage.PaymentSuccess);
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, $"{returnUrl}?isSuccess=false&message={MessageConstant.FailMessage.BalanceNotEnough}");
                return result;
            }
            return result;
        }

        public async Task<OperationResult<bool>> UserPayByCash(int userId, int bookingId)
        {
            var result = new OperationResult<bool>();
            try
            {
                var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, userId);
                if (booking == null)
                {
                    result.AddResponseErrorStatusCode(StatusCode.NotFound, MessageConstant.FailMessage.BookingCannotPay, false);
                    return result;
                }

                //if (booking.IsCredit == true)
                //{
                //    result.AddResponseErrorStatusCode(StatusCode.BadRequest, MessageConstant.FailMessage.PayByCash, false);
                //    return result;
                //}


                var assignmentDriver = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.DRIVER.ToString(), bookingId);
                var assignmentPorter = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.PORTER.ToString(), bookingId);

                //       if (booking.Status == BookingEnums.IN_PROGRESS.ToString() &&
                //assignmentDriver.Status == AssignmentStatusEnums.COMPLETED.ToString() &&
                //assignmentPorter.Status == AssignmentStatusEnums.COMPLETED.ToString() && booking.IsCredit == false)
                if (assignmentPorter == null)
                {
                    if (booking.Status == BookingEnums.IN_PROGRESS.ToString() &&
assignmentDriver.Status == AssignmentStatusEnums.COMPLETED.ToString())
                    {
                        booking.IsCredit = true;
                    }
                }
                else if (assignmentPorter != null)
                {
                    if (booking.Status == BookingEnums.IN_PROGRESS.ToString() &&
assignmentDriver.Status == AssignmentStatusEnums.COMPLETED.ToString() &&
assignmentPorter.Status == AssignmentStatusEnums.COMPLETED.ToString())
                    {
                        booking.IsCredit = true;
                    }
                }
                else
                {
                    result.AddResponseErrorStatusCode(StatusCode.BadRequest, MessageConstant.FailMessage.PayByCash, false);
                    return result;
                }



                await _unitOfWork.BookingRepository.SaveOrUpdateAsync(booking);
                await _unitOfWork.SaveChangesAsync();

                var noti = new NotiListDto()
                {
                    BookingId = booking.Id,
                    StaffType = assignmentDriver.StaffType,
                    Type = NotificationEnums.PAYMENT_BY_CASH.ToString(),
                };
                _producer.SendingMessage("movemate.notification_user", noti);

                await _firebaseServices.SaveBooking(booking, booking.Id, "bookings");


                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.PaymentSuccess, true);
                return result;

            }
            catch (Exception ex)
            {
                result.AddResponseErrorStatusCode(StatusCode.ServerError, MessageConstant.FailMessage.ServerError, false);
                return result;
            }
        }

        public async Task<OperationResult<TranferMoneyThroughWallet>> TranferMoneyThroughWallet(int userTranferId, int userReceiveId, double amount)
        {
            var result = new OperationResult<TranferMoneyThroughWallet>();
            var response = new TranferMoneyThroughWallet
            {
                UserTranfer = new WalletTranferResponse(),
                UserReceive = new WalletTranferResponse()
            };

            try
            {
                // Retrieve transfer user's wallet
                var userTranferWallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userTranferId);
                if (userTranferWallet == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                    return result;
                }

                response.UserTranfer.BalanceBefore = userTranferWallet.Balance;
                response.UserTranfer.UserId = userTranferId;
                response.UserTranfer.WalletId = userTranferWallet.Id;

                // Retrieve recipient user's wallet
                var userReceiveWallet = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userReceiveId);
                if (userReceiveWallet == null)
                {
                    result.AddError(StatusCode.NotFound, MessageConstant.FailMessage.NotFoundWallet);
                    return result;
                }

                response.UserReceive.BalanceBefore = userReceiveWallet.Balance;
                response.UserReceive.UserId = userReceiveId;
                response.UserReceive.WalletId = userReceiveWallet.Id;

                // Create and save transfer transaction
                var userTranferTransaction = new MoveMate.Domain.Models.Transaction
                {
                    WalletId = userTranferWallet.Id,
                    Amount = amount,
                    Status = PaymentEnum.SUCCESS.ToString(),
                    TransactionType = Domain.Enums.PaymentMethod.TRANFER.ToString(),
                    TransactionCode = "R" + Utilss.RandomString(7),
                    CreatedAt = DateTime.Now,
                    Resource = Resource.Wallet.ToString(),
                    PaymentMethod = Resource.Wallet.ToString(),
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    IsCredit = false
                };

                await _unitOfWork.TransactionRepository.AddAsync(userTranferTransaction);
                response.UserTranfer.TransactionResponse = _mapper.Map<TransactionResponse>(userTranferTransaction);

                // Update transfer user's wallet balance
                userTranferWallet.Balance -= amount;
                var updateResult = await _walletService.UpdateWalletBalance(userTranferWallet.Id, (float)userTranferWallet.Balance);
                if (updateResult.IsError)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateWalletBalance);
                    return result;
                }

                // Create and save recipient transaction
                var userReceiveTransaction = new MoveMate.Domain.Models.Transaction
                {
                    WalletId = userReceiveWallet.Id,
                    Amount = amount,
                    Status = PaymentEnum.SUCCESS.ToString(),
                    TransactionType = Domain.Enums.PaymentMethod.RECEIVE.ToString(),
                    TransactionCode = "R" + Utilss.RandomString(7),
                    CreatedAt = DateTime.Now,
                    Resource = Resource.Wallet.ToString(),
                    PaymentMethod = Resource.Wallet.ToString(),
                    IsDeleted = false,
                    UpdatedAt = DateTime.Now,
                    IsCredit = true
                };

                await _unitOfWork.TransactionRepository.AddAsync(userReceiveTransaction);
                response.UserReceive.TransactionResponse = _mapper.Map<TransactionResponse>(userReceiveTransaction);

                // Update recipient user's wallet balance
                userReceiveWallet.Balance += amount;
                var receiveResult = await _walletService.UpdateWalletBalance(userReceiveWallet.Id, (float)userReceiveWallet.Balance);
                if (receiveResult.IsError)
                {
                    result.AddError(StatusCode.BadRequest, MessageConstant.FailMessage.UpdateWalletBalance);
                    return result;
                }

                // Refresh wallet balances for response
                var userReceiveWalletResult = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userReceiveId);
                var userTranferWalletResult = await _unitOfWork.WalletRepository.GetWalletByAccountIdAsync(userTranferId);

                response.UserReceive.BalanceAfter = userReceiveWalletResult.Balance;
                response.UserTranfer.BalanceAfter = userTranferWalletResult.Balance;

                await _unitOfWork.SaveChangesAsync();
                result.AddResponseStatusCode(StatusCode.Ok, MessageConstant.SuccessMessage.TranferSuccess, response);
                return result;
            }
            catch (Exception ex)
            {
                result.AddError(StatusCode.ServerError, MessageConstant.FailMessage.ServerError);
                return result;
            }
        }

    }
}