using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class TrackerSourceResponse
    {
        public int Id { get; set; }

        public int BookingTrackerId { get; set; }

        public string ResourceUrl { get; set; }

        public string ResourceCode { get; set; }

        public string Type { get; set; }
    }
}
