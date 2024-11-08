using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelResponses;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendBookingConfirmationEmailAsync(string toEmail, BookingResponse bookingResponse)
        {
            // Email subject and body content
            var subject = "Booking Confirmation";
            var messageBody = $"Hello, your booking with ID {bookingResponse.Id} has been confirmed!";

            // Create a new MimeMessage object
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("FromName", "movemate202@gmail.com")); // Your email address
            message.To.Add(new MailboxAddress("", toEmail)); // Recipient's email address
            message.Subject = subject; // Email subject
            message.Body = new TextPart("plain") { Text = messageBody }; // Body content

            // Connect to Gmail SMTP server and send the email
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls); // Use TLS for security
                client.Authenticate("movemate202@gmail.com", "Ps@12345"); // Gmail email and app password
                await client.SendAsync(message); // Send the email
                client.Disconnect(true); // Disconnect from the SMTP server
            }
        }
    }
}
