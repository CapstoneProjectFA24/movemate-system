using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class GroupResponse
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public bool? IsActived { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? DurationTimeActived { get; set; }

        public List<ScheduleWorkingResponse> ScheduleWorkings { get; set; }

        public List<UserResponse> Users { get; set; }
    }
}