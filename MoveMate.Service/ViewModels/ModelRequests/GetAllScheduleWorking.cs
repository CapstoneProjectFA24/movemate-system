using Google.Api;
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
    public class GetAllScheduleWorking : PaginationRequestV2<ScheduleWorking>
    {
        public string? Search { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public int? UserId { get; set; }

        public override Expression<Func<ScheduleWorking, bool>> GetExpressions()
        {
            var queryExpression = PredicateBuilder.New<ScheduleWorking>(true);

            // Filter by search term in Status
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();
                queryExpression = queryExpression.And(cus => cus.Status != null && cus.Status.ToLower().Contains(Search));
            }

            // Filter by Status
            if (!string.IsNullOrWhiteSpace(Status))
            {
                var statuses = Status.Split('.')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                queryExpression = queryExpression.And(tran => statuses.Contains(tran.Status));
            }
            if (!string.IsNullOrWhiteSpace(Type))
            {
                Expression = Expression.And(u => u.Type == Type);
            }

          

            // Filter by IsActived
            queryExpression = queryExpression.And(u => u.IsActived == true);

            return queryExpression;
        }

    }
}