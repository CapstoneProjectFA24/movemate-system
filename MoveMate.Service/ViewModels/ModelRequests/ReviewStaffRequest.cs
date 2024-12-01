using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class ReviewStaffRequest
    {
        public double StarReview {  get; set; }
        public string? Review {  get; set; }
    }
}
