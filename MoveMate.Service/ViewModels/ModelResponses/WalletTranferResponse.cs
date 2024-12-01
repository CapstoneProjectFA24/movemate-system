using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class WalletTranferResponse
    {
        public double? BalanceBefore {  get; set; }
        public double? BalanceAfter { get; set; }
        public int? UserId { get; set; }
        public int? WalletId { get; set; }
        public TransactionResponse TransactionResponse { get; set; }
    }
}
