﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class PorterUpdateDriverRequest
    {
        public List<BookingDetailRequest> BookingDetails { get; set; } = new List<BookingDetailRequest>();
    }
}
