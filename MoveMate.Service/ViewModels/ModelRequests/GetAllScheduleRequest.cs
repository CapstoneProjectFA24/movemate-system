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
    public class GetAllSchedule : PaginationRequestV2<ScheduleBooking>
    {
        public string? Search { get; set; }
        public string? Status { get; set; }

        public override Expression<Func<ScheduleBooking, bool>> GetExpressions()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<ScheduleBooking>(true);
                queryExpression.Or(cus => cus.Shard.ToString().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                var statuses = Status.Split('.')
                    .Select(s => s.Trim())
                    .ToArray();

                Expression = Expression.And(sb => sb.Assignments.Any(a => statuses.Contains(a.Status)));
            }

            Expression = Expression.And(u => u.IsActived == true);

            return Expression;
        }
    }
}