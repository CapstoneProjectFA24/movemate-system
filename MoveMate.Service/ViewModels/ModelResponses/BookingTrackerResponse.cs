using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class BookingTrackerResponse
    {
        public int BookingId { get; set; }

        public string Time { get; set; }

        public string Type { get; set; }

        public string Location { get; set; }

        public string Point { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public List<TrackerSourceResponse> TrackerSources { get; set; }
    }
}
