using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class HouseTypesResponse
    {
        public int Id { get; set; }
        public int? BookingId { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }
        public List<HouseTypeSettingResponse> HouseTypeSettings { get; set; }
    }
}