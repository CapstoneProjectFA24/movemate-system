using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class GetUserResponse
    {
        public int Id { get; set; }

        public int? RoleId { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }   

        public string? Gender { get; set; }

        public string? Email { get; set; }

        public string? AvatarUrl { get; set; }

        public DateTime? Dob { get; set; }

        public bool? IsBanned { get; set; }

        public bool? IsDeleted { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public bool? IsInitUsed { get; set; }

        public bool? IsDriver { get; set; }

        public string? CodeIntroduce { get; set; }

        public string? NumberIntroduce { get; set; }
    }
}
