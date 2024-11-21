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
    public class GetAllBookingDetail : PaginationRequestV2<BookingDetail>
    {
        public string? Search { get; set; }
        public int? UserId { get; set; }
        public string? Type { get; set; }

        public override Expression<Func<BookingDetail, bool>> GetExpressions()
        {

            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<BookingDetail>(true);
                queryExpression.Or(cus => cus.Name.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }
            if (UserId.HasValue)
            {
                Expression = Expression.And(detail =>
                    detail.Assignments.Any(assignment =>
                        assignment.UserId == UserId &&
                        assignment.StaffType == "REVIEWER" &&
                        assignment.BookingId == detail.BookingId));
            }

            Expression = Expression.And(detail => detail.Status == BookingDetailStatusEnums.WAITING.ToString());


            return Expression;
        }
    }
}
