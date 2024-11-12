using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using MoveMate.Service.IServices;
using MoveMate.Service.ViewModels.ModelResponses;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Commons;
using System.Globalization;
using MoveMate.Domain.Models;

namespace MoveMate.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EmailService> logger)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // Method to load an embedded resource (email template)
        private async Task<string> LoadEmbeddedResourceAsync(string resourceName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourcePath = $"MoveMate.Service.Resources.EmailTemplates.{resourceName}";


                // Check if the resource exists
                using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    if (stream == null)
                    {
                        throw new FileNotFoundException("Email template file not found.");
                    }

                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading resource: {ex.Message}");
                throw;
            }
        }

        // Method to send email using the template and placeholders
        public async Task SendEmailAsync(string toEmail, string subject, string templateFileName, Dictionary<string, string> placeholders)
        {
            // Load the HTML template
            var messageBody = await LoadEmbeddedResourceAsync(templateFileName);

            // Replace placeholders in the HTML template
            foreach (var placeholder in placeholders)
            {
                messageBody = messageBody.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
            }

            // Create the email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MoveMate", "movemate202@gmail.com"));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = messageBody }; // Use HTML content

            // Send the email using an SMTP client
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("movemate202@gmail.com", "ievp xmil kxhv tqtr"); // Use proper authentication
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending email: {ex.Message}");
                    throw;
                }
            }
        }

        // Example method for booking confirmation that calls the generic SendEmailAsync
        public async Task SendBookingCancellationEmailAsync(string toEmail, BookingResponse bookingResponse)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(bookingResponse.UserId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            var currentDateTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss", new CultureInfo("vi-VN"));

            // Replace placeholders in the template
            var placeholders = new Dictionary<string, string>
            {
                { "UserName", user.Name },
                { "BookingId", bookingResponse.Id.ToString() },
                { "CurrentDateTime", currentDateTime }
            };

            // Call the SendEmailAsync method with the cancellation template
            await SendEmailAsync(toEmail, "Booking Cancellation", "BookingCancellation.html", placeholders);
        }

        public async Task SendBookingSuccessfulEmailAsync(string toEmail, BookingResponse bookingResponse)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(bookingResponse.UserId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            var currentDateTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss", new CultureInfo("vi-VN"));

            // Replace placeholders in the template
            var placeholders = new Dictionary<string, string>
    {
        { "UserName", user.Name },
        { "BookingId", bookingResponse.Id.ToString() },
        { "BookingDate", bookingResponse.BookingAt },
        { "TotalAmount", bookingResponse.Total.ToString("C0", new CultureInfo("vi-VN")) },
        { "CurrentDateTime", currentDateTime }
                 };


            // Call the SendEmailAsync method with the booking successful template
            await SendEmailAsync(toEmail, "Booking Successful", "BookingSuccessful.html", placeholders);
        }

        // Add more methods for other email types, calling SendEmailAsync with different templates and placeholders


       

    }
}
