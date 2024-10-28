using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Service.Commons.Errors;

namespace MoveMate.Service.ViewModels.ModelResponse
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public bool IsError { get; set; }
        public List<Error> Errors { get; set; }
        public AccountTokenResponse Tokens { get; set; }
    }
}