using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoveMate.Service.ThirdPartyService.Payment.Models
{
    public static class UrlHelper
    {
        /// <summary>
        /// Generate a server URL with HTTPS scheme.
        /// </summary>
        /// <param name="httpContextAccessor">The IHttpContextAccessor instance.</param>
        /// <returns>The server URL with HTTPS scheme.</returns>
        public static string GetSecureServerUrl(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor?.HttpContext == null)
            {
                throw new Exception("HttpContext is null.");
            }

            var host = httpContextAccessor.HttpContext.Request.Host.ToUriComponent();

            if (string.IsNullOrEmpty(host))
            {
                throw new Exception("Host is not available.");
            }

            // Always force HTTPS scheme
            return $"https://{host}";
        }
    }
}
