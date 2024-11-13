using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class ScheduleWorkingResponse
    {
        public int Id { get; set; }

        public string? Status { get; set; }

        public bool? IsActived { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? DurationTimeActived { get; set; }

        public string? Type { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public virtual ICollection<GroupResponse> BookingStaffDailies { get; set; } = new List<GroupResponse>();
    }
}
