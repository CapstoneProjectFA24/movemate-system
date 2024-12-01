using MoveMate.Service.ViewModels.ModelResponses.Assignments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class GetAllPromotionResponse
    {
        public List<PromotionResponse>? PromotionUser { get; set; } = new List<PromotionResponse>();
        public List<PromotionResponse>? PromotionNoUser { get; set; } = new List<PromotionResponse>();
    }
}
