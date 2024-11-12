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
    public class GetAllBookingTrackerReport : PaginationRequestV2<BookingTracker>
    {
        public string? Search { get; set; }
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

            if (BookingId.HasValue)
            {
                Expression = Expression.And(u => u.BookingId == BookingId.Value);
            }

            Expression = Expression.And(u => u.Type == TrackerEnums.REPORT.ToString());

            return Expression;
        }
    }
}
