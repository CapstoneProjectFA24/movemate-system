using MoveMate.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class ScheduleResponse
    {
        public int Id { get; set; }

        public bool? IsActived { get; set; }

        public bool? IsDefault { get; set; }

        public int? WorkOvertime { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? ScheduleDetailsId { get; set; }

        public List<ScheduleDetailResponse> ScheduleDetails { get; set; }
    }
}
