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
    public class GetAllTruckCategoryRequest : PaginationRequestV2<TruckCategory>
    {
        public string? Search { get; set; }


        public override Expression<Func<TruckCategory, bool>> GetExpressions()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<TruckCategory>(true);
                queryExpression.Or(cus => cus.CategoryName.ToLower().Contains(Search));
                queryExpression.Or(cus => cus.Summarize.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            Expression = Expression.And(u => u.IsDeleted == false);

            return Expression;
        }
    }
}
