using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateHouseTypeRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        [JsonIgnore]
        public bool? IsActived { get; set; } = true;
    }
}
