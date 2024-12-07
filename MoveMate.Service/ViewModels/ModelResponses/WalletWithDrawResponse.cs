using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class WalletWithDrawResponse
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? WalletId { get; set; }

        public double? BalanceBefore { get; set; }

        public double? BalanceAfter { get; set; }

        public DateTime? Date { get; set; }

        public string? BankName { get; set; }

        public string? BankNumber { get; set; }

        public bool? IsSuccess { get; set; }

        public bool? IsCancel { get; set; }

        public string? CancelReason { get; set; }

        public double? Amount { get; set; }
    }
}
