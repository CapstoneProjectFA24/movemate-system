﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class ServicesResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActived { get; set; }

        public int Tier { get; set; }

        public string ImageUrl { get; set; }

        public int DiscountRate { get; set; }

        public double Amount { get; set; }

        public int ParentServiceId { get; set; }

        public List<ServiceResponse> InverseParentService { get; set; }
    }
}