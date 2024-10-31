using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class TransactionResponse
    {
        public int Id { get; set; }

        public int? PaymentId { get; set; }

        public int? WalletId { get; set; }

        public string? Resource { get; set; }

        public double? Amount { get; set; }

        public string? Status { get; set; }

        public string? Substance { get; set; }

        public string? PaymentMethod { get; set; }

        public string? TransactionCode { get; set; }

        public string? TransactionType { get; set; }
        public bool? IsCredit { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
