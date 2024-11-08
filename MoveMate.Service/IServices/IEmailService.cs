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
        Task SendBookingConfirmationEmailAsync(string toEmail, BookingResponse bookingResponse);
    }

}
