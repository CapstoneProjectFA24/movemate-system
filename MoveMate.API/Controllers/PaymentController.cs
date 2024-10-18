﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.API.Utils;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ViewModels;
using Net.payOS;
using Net.payOS.Types;
using System.Security.Claims;
using MoveMate.Service.ThirdPartyService.Payment.PayOs;
using MoveMate.Service.ThirdPartyService.Payment.Momo;
using MoveMate.Service.ThirdPartyService.Payment.VNPay;
using MoveMate.Service.ThirdPartyService.Payment.Momo.Models;
using MoveMate.Service.ThirdPartyService.Payment.VNPay.Models;
using MoveMate.Service.ThirdPartyService.Payment.Models;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IVnPayService _vnPayService;
        private readonly IUserServices _userService;
        private readonly IPaymentServices _paymentServices;
        private readonly PayOS _payOs;
        private readonly IPayOsService _payOsService;
        private readonly IMomoPaymentService _momoPaymentService;

        private readonly UnitOfWork _unitOfWork;

        public PaymentController(IVnPayService vnPayService, IUserServices userService, IUnitOfWork unitOfWork,
            IPaymentServices paymentServices, PayOS payOs, IPayOsService payOsService,
            IMomoPaymentService momoPaymentService)
        {
            _vnPayService = vnPayService;
            _userService = userService;
            _unitOfWork = (UnitOfWork)unitOfWork;
            _paymentServices = paymentServices;
            _payOs = payOs;
            _payOsService = payOsService;
            _momoPaymentService = momoPaymentService;
        }


        /// <summary>
        /// FEATURE : Payment with selected method
        /// </summary>
        /// <param name="bookingId">Booking ID for payment</param>
        /// <param name="returnUrl">URL to redirect after payment</param>
        /// <param name="selectedMethod">The selected payment method (e.g., "Momo", "VnPay", "PayOS")</param>
        /// <returns>Returns the result of the payment link creation</returns>
        /// <response code="200">Payment link created successfully</response>
        /// <response code="400">Booking status must be either DEPOSITING or COMPLETED</response>
        /// <response code="404">User not found</response>
        /// <response code="404">Booking not found</response>
        /// <response code="500">An internal server error occurred</response>
        /// <remarks>
        /// Payment method details:
        /// 
        /// (MOMO)
        /// - Card Number: 9704 0000 0000 0018
        /// - Cardholder Name: NGUYEN VAN A
        /// - Expiration Date: 03/07
        /// - OTP: Provided upon transaction
        /// 
        /// (VNP)
        /// - Card Number: 9704198526191432198
        /// - Cardholder Name: NGUYEN VAN A
        /// - Issue Date: 07/15
        /// - OTP: 123456
        ///
        /// The `returnUrl` parameter should be set to: https://movemate-dashboard.vercel.app/payment-status
        /// </remarks>
        [HttpPost("create-payment-url")]
        [Authorize]
        public async Task<IActionResult> CreatePayment(int bookingId, string returnUrl, string selectedMethod)
        {
            // Validate booking ID
            if (bookingId <= 0)
            {
                return HandleErrorResponse(new List<Error>
                {
                    new Error
                    {
                        Code = MoveMate.Service.Commons.StatusCode.BadRequest,
                        Message = "Booking ID is required and must be greater than zero."
                    }
                });
            }

            // Validate return URL
            if (string.IsNullOrEmpty(returnUrl))
            {
                return HandleErrorResponse(new List<Error>
                {
                    new Error
                    {
                        Code = MoveMate.Service.Commons.StatusCode.BadRequest, Message = "Return URL is required."
                    }
                });
            }

            // Convert string to PaymentMethod enum
            if (!Enum.TryParse(typeof(PaymentType), selectedMethod, true, out var paymentMethod))
            {
                return HandleErrorResponse(new List<Error>
                {
                    new Error
                    {
                        Code = MoveMate.Service.Commons.StatusCode.BadRequest,
                        Message = "Payment method is required and must be a valid value."
                    }
                });
            }

            // Retrieve the user ID from the claims
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid user ID in token.", isError = true });
            }

            var userId = int.Parse(accountIdClaim.Value);

            // Handle payment based on the selected method
            OperationResult<string> operationResult;
            switch ((PaymentType)paymentMethod)
            {
                case PaymentType.Momo:
                    operationResult =
                        await _momoPaymentService.CreatePaymentWithMomoAsync(bookingId, userId, returnUrl);
                    break;
                case PaymentType.VnPay:
                    operationResult = await _vnPayService.CreatePaymentAsync(bookingId, userId, returnUrl);
                    break;
                case PaymentType.PayOS:
                    operationResult = await _payOsService.CreatePaymentLinkAsync(bookingId, userId, returnUrl);
                    break;
                default:
                    return HandleErrorResponse(new List<Error>
                    {
                        new Error
                        {
                            Code = MoveMate.Service.Commons.StatusCode.BadRequest,
                            Message = "Unsupported payment method selected."
                        }
                    });
            }

            // Handle potential errors from the operation result
            if (operationResult.IsError)
            {
                return HandleErrorResponse(operationResult.Errors);
            }

            // Return the successful operation result
            return Ok(operationResult);
        }


        /// <summary>
        /// FEATURE : Recharge Payment
        /// </summary>
        /// <returns>Returns the result of wallet</returns>
        [HttpGet("recharge-callback")]
        [Authorize]
        public async Task<IActionResult> RechargeCallback([FromQuery] VnPayPaymentCallbackCommand callback,
            CancellationToken cancellationToken)
        {
            var operationResult = await _vnPayService.ProcessRechargePayment(Request.Query);
            var returnUrl = $"{callback.returnUrl}?isSuccess={(!operationResult.IsError).ToString().ToLower()}";

            if (operationResult.IsError)
            {
                return Redirect(returnUrl);
            }

            return Redirect(returnUrl);
        }


        /// <summary>
        /// FEATURE : Momo callback
        /// </summary>
        /// <returns></returns>
        [HttpGet("momo/callback")]
        [Authorize]
        public async Task<IActionResult> PaymentCallbackAsync([FromQuery] MomoPaymentCallbackCommand callback,
            CancellationToken cancellationToken)
        {
            if (callback == null)
            {
                return BadRequest(new { statusCode = 400, message = "Invalid callback data.", isError = true });
            }

            if (callback.OrderInfo == "order")
            {
                var result = await _momoPaymentService.HandleOrderPaymentAsync(HttpContext, callback);

                if (result.IsError)
                {
                    return HandleErrorResponse(result.Errors);
                }

                var redirectUrl = $"{callback.returnUrl}?isSuccess={callback.IsSuccess.ToString().ToLower()}";
                return Redirect(redirectUrl);
            }
            else if (callback.OrderInfo == "wallet")
            {
                var result = await _momoPaymentService.HandleWalletPaymentAsync(HttpContext, callback);
                if (result.IsError)
                {
                    return HandleErrorResponse(result.Errors);
                }

                var redirectUrl = $"{callback.returnUrl}?isSuccess={callback.IsSuccess.ToString().ToLower()}";
                return Redirect(redirectUrl);
            }

            return NoContent();
        }


        /// <summary>
        /// FEATURE : Momo callback
        /// </summary>
        /// <returns></returns>
        [HttpGet("vnpay-callback")]
        [Authorize]
        public async Task<IActionResult> VnPayPaymentCallback(
            [FromQuery] VnPayPaymentCallbackCommand callback,
            CancellationToken cancellationToken)
        {
            if (callback == null)
            {
                return BadRequest(new { statusCode = 400, message = "Invalid callback data.", isError = true });
            }

            var result = await _vnPayService.HandleOrderPaymentAsync(Request.Query, callback);

            if (result.IsError)
            {
                return Redirect($"{callback.returnUrl}?isSuccess={callback.IsSuccess.ToString().ToLower()}");
            }

            var redirectUrl = $"{callback.returnUrl}?isSuccess={callback.IsSuccess.ToString().ToLower()}";
            return Redirect(redirectUrl);
        }


        /// <summary>
        /// FEATURE : Recharge into wallet by PayOS
        /// </summary>
        /// <param name="paymentPayOS">Customer pay booking by PayOS</param>
        /// <returns>Returns the result of wallet</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "returnUrl": "https://movemate-dashboard.vercel.app/payment-status"
        ///     }   
        /// </remarks>
        /// <response code="200">Payment link created successfully</response>
        /// <response code="404">User not found</response>
        /// <response code="404">Wallet not found</response>
        /// <response code="500">An internal server error occurred</response>
        [HttpGet("payos/callback")]
        [Authorize]
        public async Task<IActionResult> PayOsPaymentCallback([FromQuery] PayOsPaymentCallbackCommand callback,
            CancellationToken cancellationToken)
        {
            // Validate callback data
            if (callback == null)
            {
                return BadRequest(new { statusCode = 400, message = "Invalid callback data.", isError = true });
            }

            bool IsSuccess = false;
            if (callback.IsSuccess == true)
            {
                IsSuccess = true;
            }       
            if (callback.Status == "CANCELED")
            {
                IsSuccess = false;
            }
            if (callback.Status == "PAID")
            {
                IsSuccess = true;
            }
            if (callback.IsSuccess == false)
            {
                IsSuccess = false;
            }
            var returnUrl = $"{callback.returnUrl}?isSuccess={IsSuccess.ToString().ToLower()}";

            if (callback.Type == "order")
            {
                var result = await _payOsService.HandleOrderPaymentAsync(HttpContext, callback);

                if (result.IsError)
                {
                    Redirect(returnUrl);
                }

                return Redirect(returnUrl);
            }

            if (callback.Type == "wallet")
            {
                // Handle wallet top-up processing here
                var result = await _payOsService.HandleWalletPaymentAsync(HttpContext, callback);

                if (result.IsError)
                {
                    return HandleErrorResponse(result.Errors);
                }

                return Redirect(returnUrl);
            }

            return Redirect(returnUrl);
        }


        /// <summary>
        /// TEST : Payment Fail
        /// </summary>
        /// <returns></returns>
        [HttpGet("fail")]
        public IActionResult PaymentFail()
        {
            var response = new OperationResult<bool>()
            {
                StatusCode = Service.Commons.StatusCode.BadRequest,
                Payload = false,
                Message = "Nạp tiền thất bại!"
            };
            return BadRequest(response);
        }

        /// <summary>
        /// TEST : Payment success
        /// </summary>
        /// <returns></returns>
        [HttpGet("success")]
        public IActionResult PaymentSuccess()
        {
            var response = new OperationResult<bool>()
            {
                StatusCode = Service.Commons.StatusCode.Ok,
                Payload = true,
                Message = "Thanh toán thành công!"
            };
            return Ok(response);
        }
    }
}