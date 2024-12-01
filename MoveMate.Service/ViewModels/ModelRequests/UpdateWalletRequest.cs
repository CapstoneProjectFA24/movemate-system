using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class UpdateWalletRequest
    {
        public string BankNumber { get; set; }

        public string BankName { get; set; }
    }
}
