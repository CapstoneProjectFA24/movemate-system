using System.Net;
using System.Net.Sockets;

namespace MoveMate.Service.ThirdPartyService.Payment.VNPay;

public static class UtilitiesExtensions
{
    public static string GetIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Console.WriteLine(ip.ToString());
                return ip.ToString();
            }
        }

        return "127.0.0.1";
    }
}
