using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class AddScheduleIntoGroup
    {
        public int GroupId { get; set; }
        public int ScheduleId { get; set; }
    }
}
