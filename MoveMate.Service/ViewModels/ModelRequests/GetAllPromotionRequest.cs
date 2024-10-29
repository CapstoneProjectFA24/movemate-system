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
    public class GetAllPromotionRequest : PaginationRequestV2<PromotionCategory>
    {
        public string? Search { get; set; }
        public int? ServiceId { get; set; }
        public DateTime? DateFilter { get; set; }
        public DateTime? HuntingDate { get; set; }

        public override Expression<Func<PromotionCategory, bool>> GetExpressions()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<PromotionCategory>(true);
                queryExpression.Or(cus => cus.Name.ToLower().Contains(Search));
                queryExpression.Or(cus => cus.Description.ToLower().Contains(Search));

                Expression = Expression.And(queryExpression);
            }

            if (ServiceId.HasValue)
            {
                Expression = Expression.And(u => u.ServiceId == ServiceId.Value);
            }

            if (DateFilter.HasValue)
            {
                Expression = Expression.And(u => u.StartDate <= DateFilter && u.EndDate >= DateFilter);
            }

            if (HuntingDate.HasValue)
            {
                var huntingDatePlusOneDay = HuntingDate.Value.AddDays(1); // Adjusted logic
                Expression = Expression.And(u => u.StartDate >= HuntingDate && u.StartDate <= huntingDatePlusOneDay); // Fetch promotions that start on or after the day after the hunting date
            }

            Expression = Expression.And(u => u.IsDeleted == false);

            return Expression;
        }

    }
}