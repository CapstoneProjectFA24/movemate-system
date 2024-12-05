using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class ExceptionRequest
    {
        public int BookingId { get; set; }
        [JsonIgnore] 
        public string Time { get; set; } = DateTime.Now.ToString("yy-MM-dd hh:mm:ss");

        public string? Location { get; set; }

        public string? Point { get; set; }

        public string? Description { get; set; }

        public string? Title { get; set; }
        public double? EstimatedAmount { get; set; }
        public bool? IsInsurance { get; set; }
        public virtual ICollection<ResourceRequest> ResourceList { get; set; } = new List<ResourceRequest>();
    }
}
