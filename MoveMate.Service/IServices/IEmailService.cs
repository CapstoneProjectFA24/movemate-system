using MoveMate.Service.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.IServices
{
    public interface IEmailService
    {
        Task SendBookingCancellationEmailAsync(string toEmail, BookingResponse bookingResponse);
        Task SendBookingSuccessfulEmailAsync(string toEmail, BookingResponse bookingResponse);
        Task SendEmailAsync(string toEmail, string subject, string templateFileName, Dictionary<string, string> placeholders);
    }

}
