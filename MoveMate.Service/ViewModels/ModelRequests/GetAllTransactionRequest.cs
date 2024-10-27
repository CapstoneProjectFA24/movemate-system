using LinqKit;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class GetAllTransactionRequest : PaginationRequestV2<Transaction>
    {
        public string? Search { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }

        public override Expression<Func<Transaction, bool>> GetExpressions()
        {
            var queryExpression = PredicateBuilder.New<Transaction>(true);

            // Tìm kiếm theo chuỗi "Search" nếu có
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();
                queryExpression = queryExpression.And(tran => tran.Resource.ToLower().Contains(Search));
            }

            // Tìm kiếm theo "PaymentMethod" nếu có
            if (!string.IsNullOrWhiteSpace(PaymentMethod))
            {
                PaymentMethod = PaymentMethod.Trim().ToLower();
                queryExpression = queryExpression.And(tran => tran.PaymentMethod.ToLower().Contains(PaymentMethod));
            }

            // Xử lý tìm kiếm theo "UserId" với các loại giao dịch khác nhau
            if (UserId.HasValue)
            {
                queryExpression = queryExpression.And(tran =>
                    (tran.Wallet != null && tran.Wallet.UserId == UserId) || // Giao dịch nạp tiền
                    (tran.Payment != null && tran.Payment.BookingId.HasValue && tran.Payment.Booking != null && tran.Payment.Booking.UserId == UserId) // Giao dịch thanh toán order
                );
            }

            // Tìm kiếm theo "CreatedAt" nếu có
            if (CreatedAt.HasValue)
            {
                queryExpression = queryExpression.And(tran => tran.CreatedAt.HasValue && tran.CreatedAt.Value.Date == CreatedAt.Value.Date);
            }

            // Tìm kiếm theo "Status" nếu có
            if (!string.IsNullOrWhiteSpace(Status))
            {
                var statuses = Status.Split(',')
                    .Select(s => int.TryParse(s, out var statusValue) ? (int?)statusValue : null)
                    .Where(s => s.HasValue)
                    .Select(s => s.Value)
                    .ToArray();

                queryExpression = queryExpression.And(tran => statuses.ToString().Contains(tran.Status));
            }

            return queryExpression;
        }
    }
}
