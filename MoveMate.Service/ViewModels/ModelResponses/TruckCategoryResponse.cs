using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class TruckCategoryResponse
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public double MaxLoad { get; set; }

        public string Description { get; set; }

        public string ImgUrl { get; set; }

        public string EstimatedLength { get; set; }

        public string EstimatedWidth { get; set; }

        public string EstimatedHeight { get; set; }

        public string Summarize { get; set; }

        public double Price { get; set; }

        public int TotalTrips { get; set; }
    }
}
