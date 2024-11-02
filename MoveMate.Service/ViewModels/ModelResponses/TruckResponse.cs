using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class TruckResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? TruckCategoryId { get; set; }

        public string? Model { get; set; }

        public string? NumberPlate { get; set; }

        public double? Capacity { get; set; }

        public bool? IsAvailable { get; set; }

        public string? Brand { get; set; }

        public string? Color { get; set; }

        public bool? IsInsurrance { get; set; }

        public virtual ICollection<TruckImgResponse> TruckImgs { get; set; } = new List<TruckImgResponse>();

    }
}
