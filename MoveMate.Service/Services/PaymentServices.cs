using MoveMate.Service.IServices;
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

namespace MoveMate.Service.Services
{
    public class PaymentService : IPaymentServices
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;
        private readonly IFirebaseServices _firebaseServices;
        private readonly IWalletServices _walletService;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PaymentService> logger, IFirebaseServices firebaseServices, IWalletServices walletService)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
            this._firebaseServices = firebaseServices;
            _walletService = walletService;
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
                var assignmentDriver = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.DRIVER.ToString(), bookingId);
                var assignmentPorter = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.PORTER.ToString(), bookingId);
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

                int amount = 0;
                if (booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    if (wallet.Balance < booking.Deposit)
                    {
                        result.AddError(StatusCode.BadRequest, $"{returnUrl}?isSuccess=false&message={MessageConstant.FailMessage.BalanceNotEnough}");
                        return result;
                    } else
                    {
                        amount = (int)booking.Deposit;
                    }  
                }
                else if (booking.Status == BookingEnums.IN_PROGRESS.ToString() && assignmentDriver.Status == AssignmentStatusEnums.COMPLETED.ToString() && assignmentPorter.Status == AssignmentStatusEnums.COMPLETED.ToString())
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
                else if (booking.Status == BookingEnums.IN_PROGRESS.ToString() && assignmentDriver.Status == AssignmentStatusEnums.COMPLETED.ToString() && assignmentPorter.Status == AssignmentStatusEnums.COMPLETED.ToString())
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
                            Amount = amount,
                            Status = PaymentEnum.SUCCESS.ToString(),
                            TransactionType = Domain.Enums.PaymentMethod.RECEIVE.ToString(),
                            TransactionCode = "R" + Utilss.RandomString(7),
                            CreatedAt = DateTime.Now,
                            Resource = Resource.VNPay.ToString(),
                            PaymentMethod = Resource.VNPay.ToString(),
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
                }
                else if (booking.IsReviewOnline == true && booking.Status == BookingEnums.DEPOSITING.ToString())
                {
                    booking.Status = BookingEnums.COMING.ToString();
                }
                else if (booking.Status == BookingEnums.IN_PROGRESS.ToString() && assignmentDriver.Status == AssignmentStatusEnums.COMPLETED.ToString() && assignmentPorter.Status == AssignmentStatusEnums.COMPLETED.ToString())
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
                await _firebaseServices.SaveBooking(booking, booking.Id, "bookings");
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

                if (booking.IsCredit == true)
                {
                    result.AddResponseErrorStatusCode(StatusCode.BadRequest, MessageConstant.FailMessage.PayByCash, false);
                    return result;
                }

        
                var assignmentDriver = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.DRIVER.ToString(), bookingId);
                var assignmentPorter = _unitOfWork.AssignmentsRepository.GetByStaffTypeAndIsResponsible(RoleEnums.PORTER.ToString(), bookingId);

                if (booking.Status == BookingEnums.IN_PROGRESS.ToString() &&
         assignmentDriver.Status == AssignmentStatusEnums.COMPLETED.ToString() &&
         assignmentPorter.Status == AssignmentStatusEnums.COMPLETED.ToString() && booking.IsCredit == false)
                {
                    booking.IsCredit = true;
                }
                else
                {
                    result.AddResponseErrorStatusCode(StatusCode.BadRequest, MessageConstant.FailMessage.PayByCash, false);
                    return result;
                }


                
                await _unitOfWork.BookingRepository.SaveOrUpdateAsync(booking);
                await _unitOfWork.SaveChangesAsync();

                var notificationUser =
        await _unitOfWork.NotificationRepository.GetByUserIdAsync(userId);
                if (notificationUser == null) { 
                }
                else if(!string.IsNullOrEmpty(notificationUser.FcmToken))
                {
                    var title = "Thông báo: Thanh toán bằng tiền mặt";
                    var body = $"Thông báo: Người dùng đã chọn thanh toán bằng tiền mặt cho đơn hàng {booking.Id}.";
                    var fcmToken = notificationUser.FcmToken;
                    var data = new Dictionary<string, string>
                    {
                        { "bookingId", booking.Id.ToString() },
                        { "status", booking.Status.ToString() },
                        { "message", "Người dùng đã chọn thanh toán bằng tiền mặt." }
                    };

                    // Send notification to Firebase
                    await _firebaseServices.SendNotificationAsync(title, body, fcmToken, data);
                }

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
    }
}