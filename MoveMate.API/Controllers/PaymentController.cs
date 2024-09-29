using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoveMate.API.Utils;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using MoveMate.Service.IServices;
using MoveMate.Service.Services;
using MoveMate.Service.ThirdPartyService.Momo;
using MoveMate.Service.ViewModels.ModelRequests;
using Service.IServices;
using System.Security.Claims;

namespace MoveMate.API.Controllers
{
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IVnPayService _vnPayService;
        private readonly IUserServices _userService;
        private readonly IMomoService _momoPaymentService;
        private readonly IZaloPayService _zaloPayService;

        private readonly UnitOfWork _unitOfWork;

        public PaymentController(IVnPayService vnPayService, IUserServices userService, IUnitOfWork unitOfWork, IZaloPayService zaloPayService, IMomoService momoPaymentService)
        {
            _vnPayService = vnPayService;
            _userService = userService;
            _unitOfWork = (UnitOfWork)unitOfWork;
            _zaloPayService = zaloPayService;
            _momoPaymentService = momoPaymentService;
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



        [HttpPost("create")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] CreatePaymentRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid payment request.");
            }

            // Explicit type deconstruction
            (bool success, string payUrlOrMessage) = await _momoPaymentService.CreatePaymentLink(
                request.RequestId,
                request.OrderId,
                request.Amount,
                request.OrderInfo,
                request.ExtraData
            );

            if (success && !string.IsNullOrEmpty(payUrlOrMessage))
            {
                return Ok(new { success = true, payUrl = payUrlOrMessage });
            }
            else
            {
                return BadRequest(new { success = false, message = payUrlOrMessage });
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



        /// <summary>
        /// Tạo đơn hàng thanh toán qua ZaloPay.
        /// </summary>
        /// <param name="bookingId">ID của đơn đặt hàng.</param>
        /// <returns>URL thanh toán ZaloPay.</returns>
        [HttpPost("create-order")]
        [Authorize]
        public async Task<IActionResult> CreateOrder(int bookingId)
        {
            // Lấy userId từ token (claims)
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            Claim userIdClaim = claims.FirstOrDefault(x => x.Type == "sid");

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return Unauthorized(new { Message = "Invalid user ID in token." });
            }

            int userId = int.Parse(userIdClaim.Value);

            try
            {
                // Tạo đơn hàng thanh toán qua ZaloPay
                var zaloPayOrderUrl = await _zaloPayService.CreateOrderAsync(userId, bookingId);

                return Ok(new { PaymentUrl = zaloPayOrderUrl });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Xử lý callback sau khi thanh toán từ ZaloPay.
        /// </summary>
        /// <returns>Kết quả thanh toán.</returns>
        [HttpGet("callback")]
        public IActionResult ZaloPayCallback()
        {
            // Xử lý callback ZaloPay (phần này sẽ phụ thuộc vào cấu trúc phản hồi từ ZaloPay)
            var result = Request.Query; // Lấy các tham số từ query string mà ZaloPay gửi về

            // Giả sử ta kiểm tra mã phản hồi thanh toán (response_code)
            var responseCode = result["response_code"];
            if (responseCode != "00")
            {
                return BadRequest(new { Message = "Payment failed." });
            }

            // Nếu thanh toán thành công, thực hiện các bước cần thiết (cập nhật đơn hàng, lưu trữ giao dịch, v.v.)
            // Sau đó trả về kết quả
            return Ok(new { Message = "Payment successful." });
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
