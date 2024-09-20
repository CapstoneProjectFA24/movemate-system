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

namespace MoveMate.Service.Services
{
    public class PaymentService : IPaymentServices
    {
        private UnitOfWork _unitOfWork;
        private readonly PayOS _payOS;

        public PaymentService(IUnitOfWork unitOfWork, PayOS payOS)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _payOS = payOS;
        }

        public async Task<OperationResult<PaymentData>> CreatePaymentBooking(int userId, int bookingId, int scheduleDetailId)
        {
            var result = new OperationResult<PaymentData>();

            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
                var scheduleDetail = await _unitOfWork.ScheduleDetailRepository.GetByIdAsync(scheduleDetailId);

                if (user == null || booking == null || scheduleDetail == null)
                {
                    result.AddError(StatusCode.InvalidInput, "Invalid user, booking, or schedule detail");
                    return result;
                }

                var total = booking.Total;
                var userName = user.Name;

                var paymentData = new PaymentData(
                    orderCode: (long)booking.Id,
                    amount: (int)total,
                    description: $"Booking {booking.Id}",
                    items: new List<ItemData>
                    {
                new ItemData("Booking Fee", (int)total, 1)
                    },
                    cancelUrl: "https://example.com/cancel-url",
                    returnUrl: "https://example.com/return-url",
                    signature: null,
                    buyerName: userName,
                    buyerEmail: user.Email,
                    buyerPhone: user.Phone,
                    buyerAddress: null,
                    expiredAt: null
                );

                // Giả sử createPaymentLink không có thuộc tính thành công
                var paymentResult = await _payOS.createPaymentLink(paymentData);

                // Kiểm tra kết quả theo cách nào đó, ví dụ như kiểm tra mã trạng thái HTTP
                if (paymentResult != null) // Thay thế điều kiện này bằng cách kiểm tra kết quả
                {
                    result.AddResponseStatusCode(StatusCode.Ok, "Payment created successfully", paymentData);
                }
                else
                {
                    result.AddError(StatusCode.UnknownError, "Payment creation failed");
                }
            }
            catch (Exception e)
            {
                result.AddUnknownError($"Error creating payment booking: {e.Message}");
            }

            return result;
        }


    }

}
