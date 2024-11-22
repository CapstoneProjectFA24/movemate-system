using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class AddUserIntoGroup
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }
}
