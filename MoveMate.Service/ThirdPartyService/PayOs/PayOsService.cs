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

namespace MoveMate.Service.ThirdPartyService.PayOs
{
    public class PayOsService : IPayOsService
    {
        private UnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ILogger<PayOsService> _logger;
        private readonly PayOS _payOs;
        public PayOsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PayOsService> logger, PayOS payOS)
        {
            this._unitOfWork = (UnitOfWork)unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
            this._payOs = payOS;
        }

        public async Task<OperationResult<string>> CreatePaymentLinkAsync(int bookingId, int userId)
        {
            var operationResult = new OperationResult<string>();

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                operationResult.AddError(MoveMate.Service.Commons.StatusCode.NotFound, "User not found");
                return operationResult;
            }

            var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingId, userId);
            if (booking == null)
            {
                operationResult.AddError(MoveMate.Service.Commons.StatusCode.NotFound, "Booking not found");
                return operationResult;
            }

            if (booking.Status != "WAITING" && booking.Status != "COMPLETED")
            {
                operationResult.AddError(MoveMate.Service.Commons.StatusCode.BadRequest, "Booking status must be either WAITING or COMPLETED");
                return operationResult;
            }

            int amount = (booking.Status == "WAITING") ? (int)booking.Deposit : (int)booking.Total;

            try
            {
                long newGuid = Guid.NewGuid().GetHashCode();
                var paymentData = new PaymentData(
                    orderCode: newGuid,
                    amount: amount,
                    description: "Booking Payment",
                    items: null,
                    cancelUrl: "http://localhost:5210/api/v1/payments/payment/fail",
                    returnUrl: "http://localhost:5210/api/v1/payments/payment/success",
                    buyerName: user.Name,
                    buyerEmail: user.Email,
                    buyerPhone: user.Phone,
                    buyerAddress: null,
                expiredAt: null
                );

                var paymentResult = await _payOs.createPaymentLink(paymentData);
                var paymentUrl = paymentResult.checkoutUrl;

                operationResult = OperationResult<string>.Success(paymentUrl, MoveMate.Service.Commons.StatusCode.Ok, "Payment link created successfully");
                return operationResult;
            }
            catch (Exception ex)
            {
                operationResult.AddError(MoveMate.Service.Commons.StatusCode.ServerError, "An internal server error occurred");
                return operationResult;
            }
        }
    }
}
