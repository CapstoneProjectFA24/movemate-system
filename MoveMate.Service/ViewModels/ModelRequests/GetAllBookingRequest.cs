using LinqKit;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Domain.Enums;
using MoveMate.Service.Commons.Page;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class GetAllBookingRequest : PaginationRequest<MoveMate.Domain.Models.Booking>
    {
        public string? Search { get; set; }
        public int? UserId { get; set; }
        public string? Status { get; set; }

        public bool? IsReviewOnl { get; set; }
        
        public bool? IsReview { get; set; }

        public override Expression<Func<MoveMate.Domain.Models.Booking, bool>> GetExpressions()
        {
            bool isStaff = false;
            
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<MoveMate.Domain.Models.Booking>(true);
                queryExpression.Or(cus => cus.DeliveryPoint.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            /*if (!string.IsNullOrWhiteSpace(UserId.ToString()))
            {
                Expression = Expression.And(u => u.UserId == UserId);
            }

            if (!string.IsNullOrWhiteSpace(UserId.ToString()))
            {
                Expression = Expression.And(u => u.Assignments.Any(a => a.UserId == UserId));
            }*/
            
            if (!string.IsNullOrWhiteSpace(UserId.ToString()))
            {
                Expression = Expression.And(u => u.UserId == UserId || u.Assignments.Any(a => a.UserId == UserId));
            }

            if (!string.IsNullOrWhiteSpace(IsReviewOnl.ToString()))
            {
                Expression = Expression.And(u => u.IsReviewOnline == IsReviewOnl );
            }
            
            if (!string.IsNullOrWhiteSpace(IsReview.ToString()))
            {
                Expression = Expression.And(u => u.IsStaffReview == IsReview );
                if (IsReview == true)
                {
                    Expression = Expression.And(u => (u.IsReviewOnline == true && u.Status != BookingEnums.PENDING.ToString()) || (u.IsStaffReview == IsReviewOnl) );
                }
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                var statuses = Status.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                Expression = Expression.And(tran => statuses.Contains(tran.Status));
            }

            Expression = Expression.And(u => u.IsDeleted == false);

            return Expression;
        }
    }
}