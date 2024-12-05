using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class PorterCheckReportRequest
    {
        public string Status {  get; set; }
        public string? FailedReason { get; set; }
    }
}
