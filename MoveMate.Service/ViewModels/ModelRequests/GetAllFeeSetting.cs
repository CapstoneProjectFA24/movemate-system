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
        public string? Type { get; set; }


        public override Expression<Func<FeeSetting, bool>> GetExpressions()
        {
            // Start with a base expression
            var queryExpression = PredicateBuilder.New<FeeSetting>(true);

            // Search filtering
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();
                queryExpression = queryExpression.Or(cus => cus.Name != null && cus.Name.ToLower().Contains(Search));
            }

            // Name filtering
            if (!string.IsNullOrWhiteSpace(Name))
            {
                queryExpression = queryExpression.And(u => u.Name != null && u.Name == Name);
            }

            // Type filtering
            if (!string.IsNullOrWhiteSpace(Type))
            {
                var statuses = Type.Split('.')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                Expression = Expression.And(tran => statuses.Contains(tran.Type));
            }

            // Ensure only active settings are included
            queryExpression = queryExpression.And(u => u.IsActived == true);

            return queryExpression;
        }

    }
}