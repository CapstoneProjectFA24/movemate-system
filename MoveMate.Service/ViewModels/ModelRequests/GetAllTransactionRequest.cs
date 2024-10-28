using LinqKit;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Service.Commons.Page;

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

            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();
                queryExpression = queryExpression.And(tran => tran.Resource.ToLower().Contains(Search));
            }

            if (!string.IsNullOrWhiteSpace(PaymentMethod))
            {
                PaymentMethod = PaymentMethod.Trim().ToLower();
                queryExpression = queryExpression.And(tran => tran.PaymentMethod.ToLower().Contains(PaymentMethod));
            }

            if (UserId.HasValue)
            {
                queryExpression = queryExpression.And(tran =>
                    (tran.Wallet != null && tran.Wallet.UserId == UserId) || 
                    (tran.Payment != null && tran.Payment.BookingId.HasValue && tran.Payment.Booking != null && tran.Payment.Booking.UserId == UserId) 
                );
            }

            if (CreatedAt.HasValue)
            {
                queryExpression = queryExpression.And(tran => tran.CreatedAt.HasValue && tran.CreatedAt.Value.Date == CreatedAt.Value.Date);
            }

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
