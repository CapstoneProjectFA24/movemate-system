using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.RabbitMQ.DTO
{
    public class NotiListDto
    {
        public int BookingId { get; set; }
        public string Type { get; set; }
        public string StaffType { get; set; }
    }
}
