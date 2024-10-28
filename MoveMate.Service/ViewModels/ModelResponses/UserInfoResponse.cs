using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class UserInfoResponse
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string? Type { get; set; }

        public string? ImageUrl { get; set; }

        public string? Value { get; set; }
    }
}