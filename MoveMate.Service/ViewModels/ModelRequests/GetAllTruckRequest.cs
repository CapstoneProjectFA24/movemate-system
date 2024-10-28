using System.Linq.Expressions;
using LinqKit;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using MoveMate.Service.Commons.Page;

namespace MoveMate.Service.ViewModels.ModelRequests;

public class GetAllTruckRequest : PaginationRequest<Truck>
{
    public string? SearchByCategory { get; set; }

    public override Expression<Func<Truck, bool>> GetExpressions()
    {
        if (!string.IsNullOrWhiteSpace(SearchByCategory))
        {
            SearchByCategory = SearchByCategory.Trim().ToLower();

            var cateList = SearchByCategory.Split(',')
                .Select(s => int.TryParse(s, out var value) ? (int?)value : null)
                .Where(s => s.HasValue)
                .Select(s => s.Value)
                .ToArray();

            Expression = Expression.And(entity => cateList.Cast<int?>().Contains(entity.TruckCategoryId));
        }

        return Expression;
    }
}