using Microsoft.AspNetCore.Authorization;
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

        public PaymentController(IVnPayService vnPayService, IUserServices userService, IUnitOfWork unitOfWork, IPaymentServices paymentServices, PayOS payOs, IPayOsService payOsService, IMomoPaymentService momoPaymentService)
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
        /// FEATURE : Recharge Payment
        /// </summary>
        /// <returns>Returns the result of wallet</returns>
        [HttpGet("recharge-callback")]
        public async Task<IActionResult> RechagrePayment([FromQuery] VnPayPaymentCallbackCommand callback, CancellationToken cancellationToken)
        {
            var redirectUrl = $"{callback.returnUrl}?isSuccess={callback.IsSuccess.ToString().ToLower()}";
            var operationResult = await _vnPayService.RechagreExecute(Request.Query);
            if (operationResult.IsError || operationResult.Payload == null)
            {
                return Redirect(redirectUrl);
            }
            var response = operationResult.Payload; 
            if (response.VnPayResponseCode != "00")
            {
                return Redirect(redirectUrl);
            }
            var responsePayment = await _vnPayService.RechagrePayment(response);
            if (responsePayment.IsError)
            {
                return Redirect(redirectUrl);
            }
            
            return Redirect(redirectUrl);
        }





        /// <summary>
        /// FEATURE : Payment booking by PayOS
        /// </summary>
        /// <param name="paymentPayOS">Customer pay booking by PayOS</param>
        /// <returns>Returns the result of wallet</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "idBooking": 6
        ///     }   
        /// </remarks>
        /// <response code="200">Payment link created successfully</response>
        /// <response code="400">Booking status must be either WAITING or COMPLETED</response>
        /// <response code="404">User not found</response>
        /// <response code="404">Booking not found</response>
        /// <response code="500">An internal server error occurred</response>
        [HttpPost("payOS/create-payment-url")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentLinkPayOS(int bookingId, string returnUrl)
        {
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid user ID in token.", isError = true });
            }

            var userId = int.Parse(accountIdClaim.Value);
            var operationResult = await _payOsService.CreatePaymentLinkAsync(bookingId, userId, returnUrl);

            if (operationResult.IsError)
            {
                return HandleErrorResponse(operationResult.Errors);
            }

            return Ok(operationResult);
        }

        



        /// <summary>
        /// FEATURE: Payment with Momo
        /// </summary>
        /// <param name="bookingId">Booking ID for payment</param>
        /// <param name="returnUrl">URL to redirect after payment</param>
        /// <returns>Returns the result of the payment link creation</returns>
        /// <response code="200">Payment link created successfully</response>
        /// <response code="400">Booking status must be either WAITING or COMPLETED</response>
        /// <response code="404">User not found</response>
        /// <response code="404">Booking not found</response>
        /// <response code="500">An internal server error occurred</response>
        [HttpPost("momo/create-payment-url")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentWithMomo(int bookingId, string returnUrl)
        {
            // Extract user ID from claims
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid user ID in token.", isError = true });
            }

            var userId = int.Parse(accountIdClaim.Value);

            // Call the service method to create the payment link
            var operationResult = await _momoPaymentService.CreatePaymentWithMomoAsync(bookingId, userId, returnUrl);

            if (operationResult.IsError)
            {
                return HandleErrorResponse(operationResult.Errors);
            }

            return Ok(operationResult);
        }




        /// <summary>
        /// FEATURE: Payment with VNPay
        /// </summary>
        /// <param name="bookingId">Booking ID for payment</param>
        /// <param name="returnUrl">URL to redirect after payment</param>
        /// <returns>Returns the result of the payment link creation</returns>
        /// <response code="200">Payment link created successfully</response>
        /// <response code="400">Booking status must be either WAITING or COMPLETED</response>
        /// <response code="404">User not found</response>
        /// <response code="404">Booking not found</response>
        /// <response code="500">An internal server error occurred</response>
        [HttpPost("vnpay/create-payment-url")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentWithVnPay(int bookingId, string returnUrl)
        {
            // Extract user ID from claims
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid user ID in token.", isError = true });
            }

            var userId = int.Parse(accountIdClaim.Value);

            // Call the VNPay service method to create the payment link
            var operationResult = await _vnPayService.CreatePaymentAsync(bookingId, userId, returnUrl);

            if (operationResult.IsError)
            {
                return HandleErrorResponse(operationResult.Errors);
            }

            return Ok(operationResult);
        }


        /// <summary>
        /// FEATURE : Momo callback
        /// </summary>
        /// <returns></returns>
        [HttpGet("momo/callback")]
        public async Task<IActionResult> PaymentCallbackAsync([FromQuery] MomoPaymentCallbackCommand callback, CancellationToken cancellationToken)
        {

           
            if (callback == null)
            {
                return BadRequest(new { statusCode = 400, message = "Invalid callback data.", isError = true });
            }

            // Xử lý callback từ cả hai API
            if (callback.OrderInfo == "order")
            {
                // Xử lý phản hồi cho thanh toán đơn hàng
                return await HandleOrderPayment(callback, cancellationToken);
                //var redirectUrl = $"{callback.returnUrl}?isSuccess={callback.IsSuccess}";
                //return Redirect(redirectUrl);
            }
            else if (callback.OrderInfo == "wallet")
            {
               
                // Xử lý phản hồi cho nạp tiền vào ví
                var result = await _momoPaymentService.HandleWalletPaymentAsync(HttpContext, callback);

                if (result.IsError)
                {
                    return HandleErrorResponse(result.Errors);
                }

                // Thêm isSuccess vào returnUrl
                var redirectUrl = $"{callback.returnUrl}?isSuccess={callback.IsSuccess.ToString().ToLower()}";
                return Redirect(redirectUrl);
            }

            return NoContent();
        }

        private async Task<IActionResult> HandleOrderPayment(MomoPaymentCallbackCommand callback, CancellationToken cancellationToken)
        {
            // Thêm isSuccess vào returnUrl
            var redirectUrl = $"{callback.returnUrl}?isSuccess={callback.IsSuccess.ToString().ToLower()}";
            return Redirect(redirectUrl);
        }




        /// <summary>
        /// FEATURE : Momo callback
        /// </summary>
        /// <returns></returns>
        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> VnPayPaymentCallback(
        [FromQuery] VnPayPaymentCallbackCommand callback,
        CancellationToken cancellationToken)
        {
            return Redirect($"{callback.returnUrl}?isSuccess={callback.IsSuccess.ToString().ToLower()}");
        }

        /// <summary>
        /// TEST: Payment with selected method
        /// </summary>
        /// <param name="bookingId">Booking ID for payment</param>
        /// <param name="returnUrl">URL to redirect after payment</param>
        /// <param name="selectedMethod">The selected payment method (e.g., "Momo", "VnPay", "PayOS")</param>
        /// <returns>Returns the result of the payment link creation</returns>
        /// <response code="200">Payment link created successfully</response>
        /// <response code="400">Booking status must be either WAITING or COMPLETED</response>
        /// <response code="404">User not found</response>
        /// <response code="404">Booking not found</response>
        /// <response code="500">An internal server error occurred</response>
        [HttpPost("create-payment-url")]
        [Authorize]
        public async Task<IActionResult> CreatePayment(int bookingId, string returnUrl, string selectedMethod)
        {
            // Validate booking ID
            if (bookingId <= 0)
            {
                return HandleErrorResponse(new List<Error> { new Error { Code = MoveMate.Service.Commons.StatusCode.BadRequest, Message = "Booking ID is required and must be greater than zero." } });
            }

            // Validate return URL
            if (string.IsNullOrEmpty(returnUrl))
            {
                return HandleErrorResponse(new List<Error> { new Error { Code = MoveMate.Service.Commons.StatusCode.BadRequest, Message = "Return URL is required." } });
            }

            // Convert string to PaymentMethod enum
            if (!Enum.TryParse(typeof(PaymentMethod), selectedMethod, true, out var paymentMethod))
            {
                return HandleErrorResponse(new List<Error> { new Error { Code = MoveMate.Service.Commons.StatusCode.BadRequest, Message = "Payment method is required and must be a valid value." } });
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
            switch ((PaymentMethod)paymentMethod)
            {
                case PaymentMethod.Momo:
                    operationResult = await _momoPaymentService.CreatePaymentWithMomoAsync(bookingId, userId, returnUrl);
                    break;
                case PaymentMethod.VnPay:
                    operationResult = await _vnPayService.CreatePaymentAsync(bookingId, userId, returnUrl);
                    break;
                case PaymentMethod.PayOS:
                    operationResult = await _payOsService.CreatePaymentLinkAsync(bookingId, userId, returnUrl);
                    break;
                default:
                    return HandleErrorResponse(new List<Error> { new Error { Code = MoveMate.Service.Commons.StatusCode.BadRequest, Message = "Unsupported payment method selected." } });
            }

            // Handle potential errors from the operation result
            if (operationResult.IsError)
            {
                return HandleErrorResponse(operationResult.Errors);
            }

            // Return the successful operation result
            return Ok(operationResult);
        }



        [HttpGet("payos/callback")]
        public async Task<IActionResult> PayOsPaymentCallback([FromQuery] PayOsPaymentCallbackCommand callback, CancellationToken cancellationToken)
        {
            // Validate callback data
            if (callback == null)
            {
                return BadRequest(new { statusCode = 400, message = "Invalid callback data.", isError = true });
            }

            // Process the callback based on the transaction status
            var isSuccess = callback.ResultCode == 0;
            var returnUrl = $"{callback.returnUrl}?isSuccess={isSuccess.ToString().ToLower()}";

            if (callback.Type == "order")
            {
             

                return Redirect(returnUrl);
            }
            else if (callback.Type == "wallet")
            {
                // Handle wallet top-up processing here
                var result = await _payOsService.HandleWalletPaymentAsync(HttpContext, callback);

                if (result.IsError)
                {
                    return HandleErrorResponse(result.Errors);
                }

                return Redirect(returnUrl);
            }

            return NoContent();
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
