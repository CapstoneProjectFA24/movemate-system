using Fluid.Ast;
using LinqKit;
using MoveMate.Domain.Models;
using MoveMate.Service.Commons.Page;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class GetAllScheduleDailyRequest : PaginationRequestV2<Schedule>
    {
        public int? ScheduleWorkingId { get; set; }
        public int? GroupId { get; set; }
        public string Date { get; set; }

        public override Expression<Func<Schedule, bool>> GetExpressions()
        {
            Expression<Func<Schedule, bool>> expression = u => true;

            if (GroupId.HasValue)
            {
                expression = expression.And(u => u.ScheduleWorkings.Any(sw => sw.GroupId == GroupId.Value));
            }

            if (ScheduleWorkingId.HasValue)
            {
                expression = expression.And(u => u.ScheduleWorkings.Any(sw => sw.Id == ScheduleWorkingId.Value));
            }

            // Filter by Date if it's provided (ignoring default DateOnly value)
            if (!string.IsNullOrWhiteSpace(Date))
            {
                if (DateOnly.TryParseExact(Date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
                {
                    expression = expression.And(u => u.Date.HasValue && u.Date.Value == parsedDate);
                }
            }
            expression = expression.And(u => u.IsDeleted == false);

            return expression;
        }
    }
}
