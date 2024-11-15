using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class ScheduleBookingResponse
    {
        public int Id { get; set; }

        public bool? IsActived { get; set; }

        public string? Shard { get; set; }

        public virtual ICollection<AssignmentResponse> Assignments { get; set; } = new List<AssignmentResponse>();
    }
}
