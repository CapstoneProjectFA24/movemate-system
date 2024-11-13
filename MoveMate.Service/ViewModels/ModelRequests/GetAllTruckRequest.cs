using System.Linq.Expressions;
using LinqKit;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using MoveMate.Service.Commons.Page;

namespace MoveMate.Service.ViewModels.ModelRequests;

public class GetAllTruckRequest : PaginationRequestV2<Truck>
{
    public string? Search { get; set; }
    public int? UserId { get; set; }
    public int? TruckCategoryId { get; set; }

    public override Expression<Func<Truck, bool>> GetExpressions()
    {
        if (!string.IsNullOrWhiteSpace(Search))
        {
            Search = Search.Trim().ToLower();

            var queryExpression = PredicateBuilder.New<Truck>(true);
            queryExpression.Or(cus => cus.Model.ToLower().Contains(Search));
            queryExpression.Or(cus => cus.Brand.ToLower().Contains(Search));


            Expression = Expression.And(queryExpression);
        }

        if (UserId.HasValue)
        {
            Expression = Expression.And(u => u.UserId == UserId.Value);
        }

        if (TruckCategoryId.HasValue)
        {
            Expression = Expression.And(u => u.TruckCategoryId == TruckCategoryId.Value);
        }


        Expression = Expression.And(u => u.IsDeleted == false);

        return Expression;
    }
}