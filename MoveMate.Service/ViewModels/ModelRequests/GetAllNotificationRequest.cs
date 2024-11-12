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
    public class GetAllNotificationRequest : PaginationRequest<Notification>
    {
        public string? Search { get; set; }
        public int? UserId { get; set; }

        public override Expression<Func<Notification, bool>> GetExpressions()
        {
            bool isStaff = false;

            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<Notification>(true);
                queryExpression.Or(cus => cus.Topic.ToLower().Contains(Search));
                queryExpression.Or(cus => cus.Name.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            if (!string.IsNullOrWhiteSpace(UserId.ToString()))
            {
                Expression = Expression.And(u => u.UserId == UserId);
            }

           

          

           

            return Expression;
        }
    }
}
