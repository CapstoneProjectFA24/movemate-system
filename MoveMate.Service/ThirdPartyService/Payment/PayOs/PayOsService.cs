using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.Services;
using Net.payOS.Types;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.Enums;

namespace MoveMate.Service.ThirdPartyService.Payment.PayOs
{
    public class PayOsService : IPayOsService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<PayOsService> _logger;
        private readonly PayOS _payOs;
        public PayOsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PayOsService> logger, PayOS payOS)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _payOs = payOS;
        }

        public async Task<OperationResult<string>> CreatePaymentLinkAsync(int bookingId, int userId, string returnUrl)
        {
            var operationResult = new OperationResult<string>();

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                operationResult.AddError(StatusCode.NotFound, "User not found");
                return operationResult;
            }

            var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, userId);
            if (booking == null)
            {
                operationResult.AddError(StatusCode.NotFound, "Booking not found");
                return operationResult;
            }

            if (booking.Status != "WAITING" && booking.Status != "COMPLETED")
                if (booking.Status != BookingEnums.DEPOSITING.ToString() && booking.Status != BookingEnums.COMPLETED.ToString())
                {
                    operationResult.AddError(StatusCode.BadRequest, "Booking status must be either WAITING or COMPLETED");
                    return operationResult;
                }

            int amount;
            if (booking.Status == BookingEnums.DEPOSITING.ToString())
            {
                amount = (int)booking.Deposit;
            }
            else
            {
                amount = (int)booking.TotalReal;
            }

            try
            {
                long newGuid = Guid.NewGuid().GetHashCode();
                var paymentData = new PaymentData(
                    orderCode: newGuid,
                    amount: amount,
                    description: "Booking Payment",
                    items: null,
                    cancelUrl: "https://movemate-dashboard.vercel.app/payment-status?isSuccess=false",
                    returnUrl: returnUrl,
                    buyerName: user.Name,
                    buyerEmail: user.Email,
                    buyerPhone: user.Phone,
                    buyerAddress: null,
                expiredAt: null
                );

                var paymentResult = await _payOs.createPaymentLink(paymentData);
                var paymentUrl = paymentResult.checkoutUrl;

                operationResult = OperationResult<string>.Success(paymentUrl, StatusCode.Ok, "Payment link created successfully");
                return operationResult;
            }
            catch (Exception ex)
            {
                operationResult.AddError(StatusCode.ServerError, "An internal server error occurred");
                return operationResult;
            }
        }
    }
}
