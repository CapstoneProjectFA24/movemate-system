using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class FailReportRequest
    {
        public string Status { get; set; }
        public string FailReason { get; set; }
    }
}
