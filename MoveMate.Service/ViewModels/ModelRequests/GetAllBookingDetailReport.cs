using Fluid.Ast;
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
    public class GetAllBookingDetailReport : PaginationRequestV2<BookingDetail>
    {
        public string? Search { get; set; }
        public int? UserId { get; set; }
        public string? Type { get; set; }

        public override Expression<Func<BookingDetail, bool>> GetExpressions()
        {
            var queryExpression = PredicateBuilder.New<BookingDetail>(true);

            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();
                queryExpression = queryExpression.And(cus => cus.Name.ToLower().Contains(Search));
            }

            if (UserId.HasValue)
            {
                queryExpression = queryExpression.And(detail =>
                    detail.BookingId.HasValue && 
                    detail.Booking.Assignments
                        .Any(assignment => assignment.UserId == UserId)
                );
            }

            queryExpression = queryExpression.And(detail => detail.Status == BookingDetailStatusEnums.WAITING.ToString());

            if (!string.IsNullOrWhiteSpace(Type))
            {
                queryExpression = queryExpression.And(u => u.Type == Type);
            }

            queryExpression = queryExpression.And(detail =>
       detail.Booking != null &&
       (detail.Booking.Status == BookingEnums.COMING.ToString() || detail.Booking.Status == BookingEnums.COMPLETED.ToString()
       || detail.Booking.Status == BookingEnums.IN_PROGRESS.ToString() || detail.Booking.Status == BookingEnums.PAUSED.ToString()));

            return queryExpression;
        }

    }
}
