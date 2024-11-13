using LinqKit;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using MoveMate.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Service.Commons.Page;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class GetAllStaffDailyRequest : PaginationRequest<Group>
    {
        public string? Search { get; set; }
        public string? UserName { get; set; }
        public string? Status { get; set; }

        public override Expression<Func<Group, bool>> GetExpressions()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<Group>(true);
                queryExpression.Or(cus => cus.DurationTimeActived.ToString().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            //if (!string.IsNullOrWhiteSpace(UserName))
            //{
            //    Expression = Expression.And(u => u.User.Name == UserName);
            //}


            if (!string.IsNullOrWhiteSpace(Status))
            {
                var statuses = Status.Split('.')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                Expression = Expression.And(tran => statuses.Contains(tran.Status));
            }

            Expression = Expression.And(u => u.IsActived == true);

            return Expression;
        }
    }
}