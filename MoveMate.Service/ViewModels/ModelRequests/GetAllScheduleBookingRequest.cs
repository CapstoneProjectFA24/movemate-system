using LinqKit;
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
    public class GetAllScheduleBookingRequest : PaginationRequestV2<ScheduleBooking>
    {
        public string? Search { get; set; }
        public int? BookingId { get; set; }
        public int? UserId { get; set; }

        public override Expression<Func<ScheduleBooking, bool>> GetExpressions()
        {
            // Start with a base expression
            var queryExpression = PredicateBuilder.New<ScheduleBooking>(true);

            // Search filtering
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();
                queryExpression = queryExpression.Or(cus => cus.Shard != null && cus.Shard.ToLower().Contains(Search));
            }

            if (BookingId.HasValue)
            {
                queryExpression = queryExpression.And(sb => sb.ScheduleBookingDetails.Any(sbd => sbd.BookingId == BookingId.Value));
            }

            // Filter by UserId if specified
            if (UserId.HasValue)
            {
                queryExpression = queryExpression.And(sb => sb.ScheduleBookingDetails.Any(sbd => sbd.UserId == UserId.Value));
            }

            // Ensure only active settings are included
            queryExpression = queryExpression.And(u => u.IsActived == true);

            return queryExpression;
        }
    }
}
