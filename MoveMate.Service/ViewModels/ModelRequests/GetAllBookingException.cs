using LinqKit;
using MoveMate.Domain.Enums;
using MoveMate.Service.Commons.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class GetAllBookingException : PaginationRequest<MoveMate.Domain.Models.Booking>
    {
        public string? Search { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public int? UserId { get; set; }

        public override Expression<Func<MoveMate.Domain.Models.Booking, bool>> GetExpressions()
        {
            return GetExpressionsWithRole(null);
        }

        public Expression<Func<MoveMate.Domain.Models.Booking, bool>> GetExpressionsWithRole(int? userRoleId)
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<MoveMate.Domain.Models.Booking>(true);
                queryExpression.Or(cus => cus.DeliveryPoint.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            if (!string.IsNullOrWhiteSpace(UserId.ToString()))
            {
                Expression = Expression.And(u => u.UserId == UserId);
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {

                var statuses = Type.Split('.')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
                Expression = Expression.And(b => b.BookingTrackers.Any(bt => bt.Type == Type));
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {

                var statuses = Status.Split('.')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
                Expression = Expression.And(b => b.BookingTrackers.Any(bt => bt.Status == Status));
            }
            Expression = Expression.And(u => u.IsDeleted == false);

            return Expression;
        }
    }
}
