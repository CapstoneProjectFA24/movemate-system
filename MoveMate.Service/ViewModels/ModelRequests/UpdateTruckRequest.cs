using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UpdateTruckRequest
    {
        public int? TruckCategoryId { get; set; }

        public string? Model { get; set; }

        public string? NumberPlate { get; set; }

        public double? Capacity { get; set; }

        public bool? IsAvailable { get; set; }

        public string? Brand { get; set; }

        public string? Color { get; set; }

        public bool? IsInsurrance { get; set; }

        //public int? UserId { get; set; }

        public virtual ICollection<TruckImgRequest> TruckImgs { get; set; } = new List<TruckImgRequest>();
    }
}
