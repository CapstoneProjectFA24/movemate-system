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
    public class GetAllVoucherRequest : PaginationRequestV2<Voucher>
    {
        public string? Search { get; set; }
        public int? PromotionId { get; set; }
        public int? ServiceIds { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateFilter { get; set; }
        public DateTime? HuntingDate { get; set; }

        public override Expression<Func<Voucher, bool>> GetExpressions()
        {
            // Search filter
            if (!string.IsNullOrWhiteSpace(Search))
            {
                var queryExpression = PredicateBuilder.New<Voucher>(true);
                Search = Search.Trim().ToLower();
                queryExpression = queryExpression.And(cus => cus.Code != null && cus.Code.ToLower().Contains(Search));
            }

            // Promotion ID filter
            if (PromotionId.HasValue)
            {
                Expression = Expression.And(v => v.PromotionCategoryId == PromotionId.Value);
            }
            
            if (UserId.HasValue)
            {
                Expression = Expression.And(v => v.UserId == UserId.Value);
            }

            // Date filter for promotions within a specific range
            if (DateFilter.HasValue)
            {
                Expression = Expression.And(v =>
                    v.PromotionCategory != null &&
                    v.PromotionCategory.StartDate <= DateFilter &&
                    v.PromotionCategory.EndDate >= DateFilter);
            }

            // Hunting Date filter, ensuring promotion starts within a range from HuntingDate
            if (HuntingDate.HasValue)
            {
                var huntingDatePlusOneDay = HuntingDate.Value.AddDays(1);
                Expression = Expression.And(v =>
                    v.PromotionCategory != null &&
                    v.PromotionCategory.StartDate >= HuntingDate &&
                    v.PromotionCategory.StartDate <= huntingDatePlusOneDay);
            }

            if (ServiceIds.HasValue)
            {
                Expression = Expression.And(v =>
                    v.PromotionCategory != null &&
                    v.PromotionCategory.ServiceId == ServiceIds.Value);
            }

            // Exclude deleted vouchers
            Expression = Expression.And(v => v.IsDeleted == false);

            return Expression;
        }


    }
}
