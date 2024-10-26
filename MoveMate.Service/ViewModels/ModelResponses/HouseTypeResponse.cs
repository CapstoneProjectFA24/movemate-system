using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class HouseTypeResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}