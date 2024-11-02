using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UpdateUserInfoRequest
    {
        [Required]
        public string? Type { get; set; }
        [Required]
        public string? ImageUrl { get; set; }
        [Required]
        public string? Value { get; set; }
    }
}
