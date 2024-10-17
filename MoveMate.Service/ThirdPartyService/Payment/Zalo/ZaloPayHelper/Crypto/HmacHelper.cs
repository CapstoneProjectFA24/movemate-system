using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MoveMate.Service.ThirdPartyService.Payment.Zalo.ZaloPayHelper.Crypto
{
    public enum ZaloPayHMAC
    {
        HMACMD5,
        HMACSHA1,
        HMACSHA256,
        HMACSHA512
    }

    public class HmacHelper
    {
        public static string Compute(string key = "", string message = "",
            ZaloPayHMAC algorithm = ZaloPayHMAC.HMACSHA256)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] hashMessage = algorithm switch
            {
                ZaloPayHMAC.HMACMD5 => new HMACMD5(keyByte).ComputeHash(messageBytes),
                ZaloPayHMAC.HMACSHA1 => new HMACSHA1(keyByte).ComputeHash(messageBytes),
                ZaloPayHMAC.HMACSHA256 => new HMACSHA256(keyByte).ComputeHash(messageBytes),
                ZaloPayHMAC.HMACSHA512 => new HMACSHA512(keyByte).ComputeHash(messageBytes),
                _ => new HMACSHA256(keyByte).ComputeHash(messageBytes)
            };

            return BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
        }
    }
}