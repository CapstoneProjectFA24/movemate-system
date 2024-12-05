using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class ManagerResolveRequest
    {
        public bool IsCompensation {  get; set; }
        public string? FailedReason { get; set; }
        public double? RealAmount { get; set; }
        public string PaymentMethod { get; set; }
        public virtual ICollection<ResourceRequest> ResourceList { get; set; } = new List<ResourceRequest>();
    }
}
