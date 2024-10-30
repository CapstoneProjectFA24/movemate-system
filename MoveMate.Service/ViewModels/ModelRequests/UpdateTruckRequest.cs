using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UpdateTruckRequest
    {
        [Required]
        public int TruckCategoryId { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string NumberPlate { get; set; }
        [Required]
        public double Capacity { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public bool IsInsurrance { get; set; }
        [Required]
        public virtual ICollection<TruckImgRequest> TruckImgs { get; set; } = new List<TruckImgRequest>();
    }
}
