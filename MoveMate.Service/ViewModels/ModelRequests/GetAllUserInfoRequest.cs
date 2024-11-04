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
    public class GetAllUserInfoRequest : PaginationRequestV2<UserInfo>
    {
        public string? Search { get; set; }
        public int? UserId { get; set; }
        public string? Type { get; set; }


        public override Expression<Func<UserInfo, bool>> GetExpressions()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<UserInfo>(true);
                queryExpression.Or(cus => cus.Value.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {
                var statuses = Type.Split('.')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                Expression = Expression.And(tran => statuses.Contains(tran.Type));
            }

            if (!string.IsNullOrWhiteSpace(UserId.ToString()))
            {
                Expression = Expression.And(u => u.UserId == UserId);
            }



            Expression = Expression.And(u => u.IsDeleted == false);

            return Expression;
        }
    }
}
