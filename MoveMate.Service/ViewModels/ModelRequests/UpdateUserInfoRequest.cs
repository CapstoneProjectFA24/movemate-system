using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UpdateUserInfoRequest
    {
        public string? Type { get; set; }

        public string? ImageUrl { get; set; }

        public string? Value { get; set; }
    }
}
