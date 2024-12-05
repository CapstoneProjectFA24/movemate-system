using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class WalletResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public float Balance { get; set; }

        public string? BankNumber { get; set; }

        public string? BankName { get; set; }
        public string? ExpirdAt { get; set; }
        public string? CardHolderName { get; set; }
        public bool? IsLocked { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}