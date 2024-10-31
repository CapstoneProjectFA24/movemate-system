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
    public class GetAllUserRequest : PaginationRequest<User>
    {
        public string? Search { get; set; }
        public string? Name { get; set; }
        public string? RoleName { get; set; }
        public string? Status { get; set; }

        public override Expression<Func<User, bool>> GetExpressions()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<User>(true);
                queryExpression.Or(cus => cus.Email.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                Expression = Expression.And(u => u.Name == Name);
            }

            if (!string.IsNullOrWhiteSpace(RoleName))
            {
                Expression = Expression.And(u => u.Role.Name.Contains(RoleName));
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                var statuses = Status.Split('.')
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