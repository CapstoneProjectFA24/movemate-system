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
        public bool? IsGroup { get; set; }
        public int? TruckCategoryId { get; set; }
        public bool IsBanned { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool IsAccepted { get; set; } = true;

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

            if (IsGroup.HasValue)
            {
                if (IsGroup.Value)
                {
                    Expression = Expression.And(u => u.GroupId.HasValue);
                }
                else
                {
                    Expression = Expression.And(u => !u.GroupId.HasValue);
                }
            }

            if (!string.IsNullOrWhiteSpace(TruckCategoryId.ToString()))
            {
                Expression = Expression.And(u => u.Truck.TruckCategoryId == TruckCategoryId);
            }

          
            
            Expression = Expression.And(u => u.IsBanned == IsBanned);
            Expression = Expression.And(u => u.IsAccepted == IsAccepted);
            Expression = Expression.And(u => u.IsDeleted == IsDeleted);

            return Expression;
        }
    }
}