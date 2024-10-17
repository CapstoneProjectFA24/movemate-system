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
    public class GetAllHouseTypeRequest : PaginationRequestV2<HouseType>
    {
        public string? Search { get; set; }
        public string? Name { get; set; }


        public override Expression<Func<HouseType, bool>> GetExpressions()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<HouseType>(true);
                queryExpression.Or(cus => cus.Name.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                Expression = Expression.And(u => u.Name == Name);
            }

            return Expression;
        }
    }
}