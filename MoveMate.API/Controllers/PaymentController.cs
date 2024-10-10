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
using MoveMate.Service.ThirdPartyService.VNPay;
using System.Security.Claims;
using MoveMate.Service.ThirdPartyService.PayOs;
using MoveMate.Service.ThirdPartyService.Momo.Models;
using MoveMate.Service.ThirdPartyService.Momo;

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
        /// FEATURE : Recharge money into wallet.
        /// </summary>
        /// <param name="model">Customer recharge money into wallet in system </param>
        /// <returns>Returns the result of wallet</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "amount": 666666
        ///     }   
        /// </remarks>
        /// <response code="200">Payment URL generated successfully</response>
        /// <response code="404">User not found</response>
        /// <response code="404">Wallet not found</response>
        /// <response code="500">An internal server error occurred</response>
        [HttpPost("create-recharge-payment-url")]
        [Authorize]
        public async Task<IActionResult> Recharge([FromBody] VnPaymentRecharge model)
        {
            var accountId = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));

            if (accountId == null || string.IsNullOrEmpty(accountId.Value))
            {
                return Unauthorized(new { Message = "Invalid user ID in token." });
            }

            var userId = int.Parse(accountId.Value);
            var result = await _vnPayService.Recharge(HttpContext, userId, model.Amount);

            if (result.IsError)
            {
                return HandleErrorResponse(result.Errors);
            }
            return Ok(result);
        }




        /// <summary>
        /// FEATURE : Recharge Payment
        /// </summary>
        /// <returns>Returns the result of wallet</returns>
        [HttpGet("recharge-callback")]
        public async Task<IActionResult> RechagrePayment()
        {
            var operationResult = await _vnPayService.RechagreExecute(Request.Query);
            if (operationResult.IsError || operationResult.Payload == null)
            {
                return Redirect("http://localhost:3000/test-failed");
            }
            var response = operationResult.Payload; 
            if (response.VnPayResponseCode != "00")
            {
                return Redirect("http://localhost:3000/test-failed");
            }
            var responsePayment = await _vnPayService.RechagrePayment(response);
            if (responsePayment.IsError)
            {
                return Redirect("http://localhost:3000/test-failed");
            }
            
            return Redirect("http://localhost:3000/test-success");
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
        public async Task<IActionResult> CreatePaymentLinkPayOS(int bookingId)
        {
            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                return Unauthorized(new { statusCode = 401, message = "Invalid user ID in token.", isError = true });
            }

            var userId = int.Parse(accountIdClaim.Value);
            var operationResult = await _payOsService.CreatePaymentLinkAsync(bookingId, userId);

            if (operationResult.IsError)
            {
                return HandleErrorResponse(operationResult.Errors);
            }

            return Ok(operationResult);
        }




        /// <summary>
        /// FEATURE : Payment Momo
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
        /// FEATURE : Momo callback
        /// </summary>
        /// <returns></returns>
        [HttpGet("momo/callback")]
        public async Task<IActionResult> MomoPaymentCallback(
            [FromQuery] MomoPaymentCallbackCommand callback, CancellationToken cancellationToken)
        {
            return Redirect($"{callback.returnUrl}?isSuccess={callback.IsSuccess}");
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
