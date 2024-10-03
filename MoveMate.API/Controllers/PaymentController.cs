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

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IVnPayService _vnPayService;
        private readonly IUserServices _userService;
        private readonly IPaymentServices _paymentServices;
        private readonly PayOS _payOs;

        private readonly UnitOfWork _unitOfWork;

        public PaymentController(IVnPayService vnPayService, IUserServices userService, IUnitOfWork unitOfWork, IPaymentServices paymentServices, PayOS payOs)
        {
            _vnPayService = vnPayService;
            _userService = userService;
            _unitOfWork = (UnitOfWork)unitOfWork;
            _paymentServices = paymentServices;
            _payOs = payOs;
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
        [HttpPost("payment/payOs/create-payment-link")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentLink(int bookingid)
        {
            var operationResult = new OperationResult<string>();

            var accountIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower().Equals("sid"));
            if (accountIdClaim == null || string.IsNullOrEmpty(accountIdClaim.Value))
            {
                operationResult.AddError(MoveMate.Service.Commons.StatusCode.UnAuthorize, "Invalid user ID in token.");
                return HandleErrorResponse(operationResult.Errors);
            }

            var userId = int.Parse(accountIdClaim.Value);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                operationResult.AddError(MoveMate.Service.Commons.StatusCode.NotFound, $"Can't find User with Id: {userId}");
                return HandleErrorResponse(operationResult.Errors);
            }

            var booking = await _unitOfWork.BookingRepository.GetByBookingIdAndUserIdAsync(bookingid, userId);
            if (booking == null)
            {
                operationResult.AddError(MoveMate.Service.Commons.StatusCode.NotFound, $"Can't find Booking with Id: {bookingid}");
                return HandleErrorResponse(operationResult.Errors);
            }

            if (booking.Status != "WAITING" && booking.Status != "COMPLETED")
            {
                operationResult.AddError(MoveMate.Service.Commons.StatusCode.BadRequest, "Booking status must be either WAITING or COMPLETED.");
                return HandleErrorResponse(operationResult.Errors);
            }
            int amount = 0;
            if (booking.Status == "WAITING")
            {
                amount = (int)booking.Deposit; 
            }
            else if (booking.Status == "COMPLETED")
            {
                amount = (int)booking.Total; 
            }

            try
            {
                var paymentData = new PaymentData(
                    orderCode: 7,
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
                operationResult = OperationResult<string>.Success(paymentUrl, MoveMate.Service.Commons.StatusCode.Ok, "Payment link created successfully.");
                return Ok(operationResult);
            }
            catch (Exception ex)
            {
                // If an error occurs, add the error to OperationResult
                operationResult.AddError(MoveMate.Service.Commons.StatusCode.ServerError, "An internal server error occurred: " + ex.Message);
                return HandleErrorResponse(operationResult.Errors);
            }
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
