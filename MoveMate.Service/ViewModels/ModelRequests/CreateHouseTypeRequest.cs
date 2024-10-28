using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class CreateHouseTypeRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public bool? IsActived { get; set; } = true;
    }
}
