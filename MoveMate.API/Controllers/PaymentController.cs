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

        private readonly UnitOfWork _unitOfWork;

        public PaymentController(IVnPayService vnPayService, IUserServices userService, IUnitOfWork unitOfWork, IPaymentServices paymentServices, PayOS payOs, IPayOsService payOsService)
        {
            _vnPayService = vnPayService;
            _userService = userService;
            _unitOfWork = (UnitOfWork)unitOfWork;
            _paymentServices = paymentServices;
            _payOs = payOs;
            _payOsService = payOsService;
        }


        /// <summary>
        /// Recharge money into wallet.
        /// </summary>
        /// <param name="model">The details of the auction to create.</param>
        /// <returns>Returns the result of the auction creation.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST
        ///     {
        ///         "amount": 666666
        ///     }   
        /// </remarks>
        [HttpPost("payment/create_recharge-payment-url")]
        [Authorize]
        public async Task<IActionResult> Recharge([FromBody] VnPaymentRecharge model)
        {
            // Retrieve user ID from claims
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim accountId = claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));

            if (accountId == null || string.IsNullOrEmpty(accountId.Value))
            {
                return Unauthorized(new { Message = "Invalid user ID in token." });
            }

            var userId = int.Parse(accountId.Value);

            if (model == null)
            {
                return BadRequest("Invalid payment request model.");
            }

            try
            {
                var paymentUrl = await _vnPayService.Recharge(HttpContext, userId, model.Amount);
                return Ok(new { Url = paymentUrl });
            }
            catch (Exception ex)
            {
                string error = ErrorUtil.GetErrorString("Exception", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, error);
            }
        }




        /// <summary>
        /// Recharge Payment
        /// </summary>
        /// <returns></returns>
        [HttpGet("payment/recharge-callback")]
        public IActionResult RechagrePayment()
        {

            var response = _vnPayService.RechagreExecute(Request.Query).Result;

            if (response == null || response.VnPayResponseCode != "00")
            {
                return Redirect("http://localhost:3000/test-failed");
            }


            // Save payment
            var responsePayment = _vnPayService.RechagrePayment(response).Result;

            if (responsePayment.IsError)
            {
                return Redirect("http://localhost:3000/test-failed");
            }

            return Redirect("http://localhost:3000/test-success");
        }

        ///// <summary>
        ///// Create a payment for a booking.
        ///// </summary>
        ///// <param name="bookingId">The ID of the booking.</param>
        ///// <param name="scheduleDetailId">The ID of the schedule detail.</param>
        ///// <returns>Returns the result of the payment creation.</returns>
        //[HttpPost("payment/zalo/create-booking-payment")]
        //[Authorize]
        //public async Task<IActionResult> CreateBookingPayment(int bookingId, int scheduleDetailId)
        //{
        //    // Retrieve user ID from claims
        //    var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
        //    if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
        //    {
        //        return Unauthorized(new { Message = "Invalid user ID in token." });
        //    }

        //    var userId = int.Parse(accountIdClaim.Value);

        //    // Call the CreatePaymentBooking method
        //    var result = await _paymentServices.CreatePaymentBooking(userId, bookingId, scheduleDetailId);

        //    if (result.IsError)
        //    {
        //        return BadRequest(result);
        //    }

        //    return Ok(result);
        //}
      



        [HttpPost("payment/payOS/create")]
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
        /// Payment Fail
        /// </summary>
        /// <returns></returns>
        [HttpGet("payment/fail")]

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
        /// Get auction by Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("payment/success")]

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
