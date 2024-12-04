using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class GroupUserResponse
    {
        public int Reviewers {  get; set; }
        public int ReviewersNeed { get; set; }
        public int DriversTruck1 { get; set; }
        public int DriversTruck1Need { get; set; }
        public int DriversTruck2 { get; set; }
        public int DriversTruck2Need { get; set; }
        public int DriversTruck3 { get; set; }
        public int DriversTruck3Need { get; set; }
        public int DriversTruck4 { get; set; }
        public int DriversTruck4Need { get; set; }
        public int DriversTruck5 { get; set; }
        public int DriversTruck5Need { get; set; }
        public int DriversTruck6 { get; set; }
        public int DriversTruck6Need { get; set; }
        public int Porters { get; set; }
        public int PortersNeed { get; set;}

    }
}
