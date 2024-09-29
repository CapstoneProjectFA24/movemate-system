using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoveMate.Domain.Enums;
using MoveMate.Repository.Repositories.UnitOfWork;
using MoveMate.Service.Exceptions;
using MoveMate.Service.IServices;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Services
{
    public class ZaloPayService : IZaloPayService
    {
        private readonly IConfiguration _config;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ZaloPayService> _logger;

        public ZaloPayService(IConfiguration config, IUnitOfWork unitOfWork, IMapper mapper, ILogger<ZaloPayService> logger)
        {
            _config = config;
            _unitOfWork = (UnitOfWork)unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> CreateOrderAsync(int userId, int bookingId)
        {
            var appId = _config["ZaloPay:AppId"];
            var key1 = _config["ZaloPay:Key1"];
            var endpoint = _config["ZaloPay:Endpoint"];

            // Retrieve the user from the repository
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException($"User with ID {userId} not found.");
            }

            // Retrieve the booking from the repository
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                throw new NotFoundException($"Booking with ID {bookingId} not found.");
            }

            // Validate that the booking belongs to the provided user ID
            if (booking.UserId != userId)
            {
                throw new UnauthorizedAccessException($"Booking with ID {bookingId} does not belong to the user with ID {userId}.");
            }

            // Check if the booking status is either 'WAITING' or 'COMPLETED'
            if (booking.Status != BookingEnums.WAITING.ToString() && booking.Status != BookingEnums.COMPLETED.ToString())
            {
                throw new InvalidOperationException($"Booking with ID {bookingId} must have a status of 'WAITING' or 'COMPLETED' to proceed.");
            }

            // Determine the amount based on the booking status
            decimal amount = booking.Status.Equals(BookingEnums.WAITING.ToString()) ? (decimal)booking.Deposit : (decimal)booking.Total;

            // Create payment parameters and generate MAC (Message Authentication Code)
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var orderInfo = new
            {
                app_id = appId,
                app_trans_id = bookingId.ToString(),
                app_time = timestamp,
                amount, // Use the decimal amount directly
                app_user = userId.ToString(),
                description = booking.Note,
                bank_code = "ZaloPay",
                mac = GenerateMac(appId, bookingId.ToString(), timestamp, amount, key1)
            };

            // Log the order info for debugging
            _logger.LogInformation($"Order Info: {JsonConvert.SerializeObject(orderInfo)}");

            // Send request to ZaloPay
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(orderInfo), Encoding.UTF8, "application/json");

                _logger.LogInformation($"Sending request to: {endpoint}");
                var response = await client.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to create ZaloPay order: {response.StatusCode} - {errorContent}");
                    throw new Exception($"Failed to create ZaloPay order: {response.StatusCode} - {errorContent}");
                }

                return await response.Content.ReadAsStringAsync();
            }
        }

        private string GenerateMac(string appId, string orderId, long timestamp, decimal amount, string key1)
        {
            var rawData = $"{appId}|{orderId}|{timestamp}|{amount}|{key1}";
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key1)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
