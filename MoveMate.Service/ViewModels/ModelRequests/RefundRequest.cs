using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class RefundRequest
    {
        public bool? IsRefunded { get; set; }
        public string? ReasonRefundFailed { get; set; }
    }
}
