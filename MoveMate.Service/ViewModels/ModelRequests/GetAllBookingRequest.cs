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
using Catel.Collections;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class GetAllBookingRequest : PaginationRequest<MoveMate.Domain.Models.Booking>
    {
        public string? Search { get; set; }
        public int? UserId { get; set; }
        public string? Status { get; set; }

        public bool? IsReviewOnl { get; set; }
        
        public bool? IsReview { get; set; }
        public bool? IsFailed { get; set; }
        public bool? IsWaiting {  get; set; }

        public override Expression<Func<MoveMate.Domain.Models.Booking, bool>> GetExpressions()
        {
            return GetExpressionsWithRole(null);
        }

        public Expression<Func<MoveMate.Domain.Models.Booking, bool>> GetExpressionsWithRole(int? userRoleId)
        {
            bool isStaff = false;
            
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Search = Search.Trim().ToLower();

                var queryExpression = PredicateBuilder.New<MoveMate.Domain.Models.Booking>(true);
                queryExpression.Or(cus => cus.DeliveryPoint.ToLower().Contains(Search));


                Expression = Expression.And(queryExpression);
            }

            if (userRoleId != 1 && userRoleId != 6)
            {
                if (!string.IsNullOrWhiteSpace(UserId.ToString()))
                {
                    Expression = Expression.And(u => u.UserId == UserId || u.Assignments.Any(a => a.UserId == UserId));
                }
            }

            if (!string.IsNullOrWhiteSpace(IsReviewOnl.ToString()))
            {
                Expression = Expression.And(u => u.IsReviewOnline == IsReviewOnl );
            }
            
            if (!string.IsNullOrWhiteSpace(IsReview.ToString()))
            {
                Expression = Expression.And(u => u.IsStaffReviewed == IsReview );
                if (IsReview == true)
                {
                    Expression = Expression.And(u => (u.IsReviewOnline == true && u.Status != BookingEnums.PENDING.ToString()) || (u.IsStaffReviewed == IsReviewOnl) );
                }
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
               
                var statuses = Status.Split('.')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();  

                if (statuses.Contains("HOLD"))
                {
                    statuses.AddRange(new[] { "ASSIGNED", "PENDING" });
                    statuses.Remove("HOLD");
                }
                if (statuses.Contains("VALIDATTION"))
                {
                    statuses.AddRange(new[] { "WAITING", "DEPOSITING", "REVIEWED" });
                    statuses.Remove("VALIDATTION");
                }
                if (statuses.Contains("EVALUATING"))
                {
                    statuses.AddRange(new[] { "REVIEWING" });
                    statuses.Remove("EVALUATING");
                }
                if (statuses.Contains("PROGRESSING"))
                {
                    statuses.AddRange(new[] { "IN_PROGRESS", "COMING", "PAUSED" });
                    statuses.Remove("PROGRESSING");
                }
                if (statuses.Contains("DONE"))
                {
                    statuses.AddRange(new[] { "COMPLETED" });
                    statuses.Remove("DONE");
                }
                if (statuses.Contains("PAID"))
                {
                    statuses.AddRange(new[] { "COMPLETED" });
                    statuses.Remove("PAID");
                }
                if (statuses.Contains("ADVANCE"))
                {
                    if (IsReviewOnl.HasValue && IsReviewOnl.Value)
                    {
                        statuses.AddRange(new[] { "IN_PROGRESS", "COMING", "PAUSED" });
                    }
                    else
                    {
                        statuses.AddRange(new[] { "IN_PROGRESS", "COMING", "PAUSED", "REVIEWING", "REVIEWED" });
                    }
                    statuses.Remove("ADVANCE");
                }
                if (statuses.Contains("COMPENSATION"))
                {
                    statuses.AddRange(new[] { "REFUNDING" });
                    statuses.Remove("COMPENSATION");
                }
                if (statuses.Contains("CANCELED"))
                {
                    statuses.AddRange(new[] { "CANCEL" });
                    statuses.Remove("CANCELED");
                }
                if (statuses.Contains("NEW"))
                {
                    if (IsReviewOnl.HasValue && IsReviewOnl.Value)
                    {
                        statuses.AddRange(new[] { "PENDING", "ASSIGNED", "REVIEWING", "REVIEWED", "DEPOSITING" });
                    }
                    else
                    {
                        statuses.AddRange(new[] { "PENDING", "ASSIGNED", "WAITING", "DEPOSITING" });
                    }
                    statuses.Remove("NEW");
                }

                statuses = statuses.Distinct().ToList();

                Expression = Expression.And(tran => statuses.Contains(tran.Status));     
            }

            if (IsFailed.HasValue && IsFailed.Value)
            {
                Expression = Expression.And(u => u.Assignments.Any(a => a.Status == AssignmentStatusEnums.FAILED.ToString()));
            }
            if (IsWaiting.HasValue && IsWaiting.Value)
            {
                Expression = Expression.And(u => u.BookingDetails.Any(a => a.Status == BookingDetailStatusEnums.WAITING.ToString()));
            }

            Expression = Expression.And(u => u.IsDeleted == false);

            return Expression;
        }
    }
}