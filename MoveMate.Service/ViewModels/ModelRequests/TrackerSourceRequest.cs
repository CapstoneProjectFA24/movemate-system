using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class TrackerSourceRequest
    {
        public virtual ICollection<ResourceRequest> ResourceList { get; set; } = new List<ResourceRequest>();
    }
}
