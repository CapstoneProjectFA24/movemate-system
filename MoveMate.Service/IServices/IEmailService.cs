using MoveMate.Service.Commons;
using MoveMate.Service.ViewModels.ModelResponses;
using MoveMate.Service.ViewModels.ModelResponses.Assignments;
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
        Task SendAssignStaffResponsibleEmailAsync(string toEmail, AssignmentResponse assignmentResponse);
        Task SendJobAcceptanceEmailAsync(string toEmail, GetUserResponse userResponse);
        Task SendEmailAsync(string toEmail, string subject, string templateFileName, Dictionary<string, string> placeholders);

        
    }

}
