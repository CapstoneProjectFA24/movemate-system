using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.Utils
{
    public class PaymentUtils
    {
        public static long GenerateOrderCode(int bookingId, long newGuid)
        {
            // Step 1: Create a combined string
            // Step 1: Create a combined string
            string combinedString = $"{bookingId}-{newGuid}";

            // Step 2: Generate a hash
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedString));

                // Step 3: Convert the first 8 bytes of the hash to a long
                long orderCode = BitConverter.ToInt64(hashBytes, 0);

                // Ensure the orderCode is positive
                if (orderCode < 0)
                {
                    orderCode = -orderCode; // Make it positive if necessary
                }

                // Step 4: Limit orderCode to be less than 10000000000
                const long maxOrderCode = 10000000000;
                if (orderCode >= maxOrderCode)
                {
                    orderCode = orderCode % maxOrderCode; // Wrap around to keep it below the limit
                }

                return orderCode;
            }
        }
    }
}