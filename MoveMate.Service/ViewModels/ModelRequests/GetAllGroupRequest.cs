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
    public class GetAllGroupRequest : PaginationRequestV2<Group>
    {
        public string? Search { get; set; }

        public override Expression<Func<Group, bool>> GetExpressions()
        {
            var queryExpression = PredicateBuilder.New<Group>(true);


            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();
                queryExpression = queryExpression.And(group => group.Status.ToLower().Contains(Search));
            }

           
            queryExpression = queryExpression.And(group => group.IsActived == true);

            return queryExpression;
        }

    }
}