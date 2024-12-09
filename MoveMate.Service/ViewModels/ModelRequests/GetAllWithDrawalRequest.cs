using LinqKit;
using MoveMate.Domain.Enums;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class GetAllWithDrawalRequest : PaginationRequestV2<Withdrawal>
    {
        public string? Search { get; set; }
        public int? UserId { get; set; }
        public int? WalletId { get; set; }
        public bool IsCancel { get; set; } 
        public bool IsSuccess { get; set; } 


        public override Expression<Func<Withdrawal, bool>> GetExpressions()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<Withdrawal>(true);
                queryExpression.Or(cus => cus.BankName.ToLower().Contains(Search));
                queryExpression.Or(cus => cus.BankNumber.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }
            if (UserId.HasValue)
            {

                Expression = PredicateBuilder.New<Withdrawal>(true); // Reset existing conditions
                Expression = Expression.And(b => b.UserId == UserId.Value);

                // Return immediately since this condition overrides others
                return Expression;
            }
            if (WalletId.HasValue)
            {
                Expression = PredicateBuilder.New<Withdrawal>(true); // Reset existing conditions
                Expression = Expression.And(b => b.WalletId == WalletId.Value);

            }

            Expression = Expression.And(u => u.IsCancel == IsCancel);
            Expression = Expression.And(u => u.IsSuccess == IsSuccess);

            return Expression;
        }
    }
}
