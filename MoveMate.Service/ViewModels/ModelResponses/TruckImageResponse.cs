using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class TruckImageResponse
    {
        public int? Id { get; set; }
        public int? TruckId { get; set; }

        public string? ImageUrl { get; set; }

        public string? ImageCode { get; set; }
    }
}
