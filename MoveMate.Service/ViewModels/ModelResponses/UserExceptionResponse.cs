using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ViewModels.ModelResponses
{
    public class UserExceptionResponse
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? CardHolderName { get; set; }
        public string? BankNumber { get; set; }
        public string? BankName { get; set; }
    }
}
