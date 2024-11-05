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
    public class GetAllServiceRequest : PaginationRequestV2<MoveMate.Domain.Models.Service>
    {
        public string? Search { get; set; }
        public string? Name { get; set; }

        public string? Type { get; set; }



        public override Expression<Func<MoveMate.Domain.Models.Service, bool>> GetExpressions()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<MoveMate.Domain.Models.Service>(true);
                queryExpression.Or(cus => cus.Name.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {
                Type = Type.Trim().ToLower();

                Expression = Expression.And(u => u.Type.ToLower().Contains(Type));
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                Expression = Expression.And(u => u.Name == Name);
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {
                var statuses = Type.Split('.')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                Expression = Expression.And(tran => statuses.Contains(tran.Type));
            }

            Expression = Expression.And(u => u.IsActived == true);

            Expression = Expression.And(i => i.Tier == 0);
            
            return Expression;
        }
    }
}