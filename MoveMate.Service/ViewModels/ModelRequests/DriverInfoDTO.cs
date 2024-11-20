using MoveMate.Service.ViewModels.ModelResponses.Assignments;
using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class DriverInfoDTO
    {
        public int? BookingNeedStaffs { get; set; }
       
        public List<UserResponse>? OtherStaffs { get; set; } = new List<UserResponse>();
        public int CountStaffs { get; set; } 

        public String StaffType { get; set; }

        public bool? IsSussed { get; set; } = false;
    }

}
