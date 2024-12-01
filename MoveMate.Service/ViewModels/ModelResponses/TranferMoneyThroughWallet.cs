using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class TranferMoneyThroughWallet
    {
        public WalletTranferResponse UserReceive {  get; set; }
        public WalletTranferResponse UserTranfer { get; set; }

    }
}
