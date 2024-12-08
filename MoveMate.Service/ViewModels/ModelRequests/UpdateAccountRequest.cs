using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UpdateAccountRequest
    {
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? AvatarUrl { get; set; }

        public DateTime? Dob { get; set; }
    }
}
