using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class DriverInfoDTO
    {
        public int UserId { get; set; }
        public string StaffType { get; set; }
        public int ScheduleId { get; set; }
        public string Phone {  get; set; }
    }

}
