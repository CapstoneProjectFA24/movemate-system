using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int RoleId { get; set; }
        public int WalletId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public string AvatarUrl { get; set; }
        
         
    }


}
