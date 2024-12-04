using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class ScheduleDailyResponse
    {
        public int Id { get; set; }
        public int ScheduleWorkingId { get; set; }
        public int GroupId { get; set; }
        public DateOnly? Date { get; set; }
    }
}
