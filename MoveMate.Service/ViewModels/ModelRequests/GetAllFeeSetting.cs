using LinqKit;
using MoveMate.Domain.Enums;
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
    public class GetAllFeeSetting : PaginationRequestV2<FeeSetting>
    {
        public string? Search { get; set; }
        public string? Name { get; set; }


        public override Expression<Func<FeeSetting, bool>> GetExpressions()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<FeeSetting>(true);
                queryExpression.Or(cus => cus.Name.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                Expression = Expression.And(u => u.Name == Name);
            }

            Expression = Expression.And(u => u.IsActived == true);
            Expression = Expression.And(i => i.Type == TypeFeeEnums.SYSTEM.ToString());

            return Expression;
        }
    }
}