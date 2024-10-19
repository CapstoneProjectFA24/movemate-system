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
    public class GetAllBookingRequest : PaginationRequest<MoveMate.Domain.Models.Booking>
    {
        public string? Search { get; set; }
        public int? UserId { get; set; }
        public string? Status { get; set; }

        public override Expression<Func<MoveMate.Domain.Models.Booking, bool>> GetExpressions()
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
                Expression = Expression.And(u => u.User.Id == UserId);
            }


            if (!string.IsNullOrWhiteSpace(Status))
            {
                var statuses = Status.Split(',')
                    .Select(s => int.TryParse(s, out var statusValue) ? (int?)statusValue : null)
                    .Where(s => s.HasValue)
                    .Select(s => s.Value)
                    .ToArray();
            }

            Expression = Expression.And(u => u.IsDeleted == false);

            return Expression;
        }
    }
}