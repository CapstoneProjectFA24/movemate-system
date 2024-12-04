using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class ScheduleRequest
    {
        public DateOnly? Date { get; set; }
        public int ScheduleWorkingId { get; set; }
        public int GroupId { get; set; }
    }
}
