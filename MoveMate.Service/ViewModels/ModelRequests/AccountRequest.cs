using System.ComponentModel.DataAnnotations;

namespace MoveMate.Service.ViewModels.ModelRequests
{
    public class AccountRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}