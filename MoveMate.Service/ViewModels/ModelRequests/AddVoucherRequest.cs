using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class AddVoucherRequest
    {
        public int Id { get; set; }

        public int PromotionCategoryId { get; set; }
    }
}
