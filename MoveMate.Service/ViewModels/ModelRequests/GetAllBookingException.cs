using LinqKit;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class GetAllBookingException : PaginationRequestV2<BookingTracker>
    {
        public string? Search { get; set; }
        public string? Type { get; set; }
        public int? UserId { get; set; }
        public int? BookingId { get; set; }
       
        public override Expression<Func<BookingTracker, bool>> GetExpressions()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<BookingTracker>(true);
                queryExpression.Or(cus => cus.Description.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }
            if (!string.IsNullOrWhiteSpace(Type))
            {

                var statuses = Type.Split('.')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
                Expression = Expression.And(b => b.Type == Type);
            }
            if (UserId.HasValue)
            {
                Expression = Expression.And(b => b.Booking.UserId == UserId);
            }
            if (BookingId.HasValue)
            {
                Expression = Expression.And(b => b.BookingId == BookingId);
            }


            Expression = Expression.And(u => u.Status == StatusTrackerEnums.WAITING.ToString());

            return Expression;
        }
    }
}
