using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class PaymentResponse
    {
        public int Id { get; set; }

        public int? BookingId { get; set; }

        public string? BankCode { get; set; }

        public string? BankTransNo { get; set; }

        public double? Amount { get; set; }

        public string? ResponseCode { get; set; }

        public bool? Success { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public TransactionResponse Transactions { get; set; }
    }
}
