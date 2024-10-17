using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class UserInfoResponse
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string? PermanentAddress { get; set; }
        public string? TemporaryResidenceAddress { get; set; }
    }
}