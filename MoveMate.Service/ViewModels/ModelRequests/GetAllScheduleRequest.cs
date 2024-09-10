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
    public class GetAllSchedule : PaginationRequestV2<Schedule>
    {
        public string? Search { get; set; }
        public string? Status { get; set; }

        public override Expression<Func<Schedule, bool>> GetExpressions()
        {

            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<Schedule>(true);
                queryExpression.Or(cus => cus.StartTime.ToString().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                var statuses = Status.Split(',')
                                 .Select(s => int.TryParse(s, out var statusValue) ? (int?)statusValue : null)
                                 .Where(s => s.HasValue)
                                 .Select(s => s.Value)
                                 .ToArray();


            }

            Expression = Expression.And(u => u.IsActived == true);

            return Expression;
        }
    }
}
