using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using System.Net;

namespace QueenApiNode.HttpService
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.Use(async (context, next) =>
            {
                var clientIp = GetClientIp(context);
                context.Set("ClientIpAddress", clientIp);


                // Continue to the next middleware
                await next.Invoke();
            });
            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            appBuilder.UseWebApi(config);

        }

        private string GetClientIp(IOwinContext context)
        {
            // Try to get the client IP address from the CF-Connecting-IP header
            string ipAddress = context.Request.Headers.Get("CF-Connecting-IP");
            if (string.IsNullOrEmpty(ipAddress))
            {

                ipAddress = context.Request.Headers.Get("CF-CONNECTING-IP");
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                // If CF-Connecting-IP is not present, try X-Forwarded-For
                ipAddress = context.Request.Headers.Get("X-Forwarded-For");
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                // If CF-Connecting-IP is not present, try X-Forwarded-For
                ipAddress = context.Request.Headers.Get("X-FORWARDED-FOR");
            }

            if (string.IsNullOrEmpty(ipAddress))
            {
                // If neither header is present, fall back to the remote IP address
                ipAddress = context.Request.RemoteIpAddress;
            }

            return ExpandIPAddress(ipAddress);
        }

        private string ExpandIPAddress(string ipAddress)
        {
            if (!IsValidIPv6(ipAddress)) return ipAddress;
            // Parse the compressed IPv6 address to an IPAddress object
            IPAddress ip = IPAddress.Parse(ipAddress);

            // Convert the address bytes to a full, expanded format
            byte[] addressBytes = ip.GetAddressBytes();
            string fullAddress = "";

            for (int i = 0; i < addressBytes.Length; i += 2)
            {
                // Combine two bytes into a 16-bit number and format as a 4-digit hex number
                ushort segment = (ushort)((addressBytes[i] << 8) + addressBytes[i + 1]);
                fullAddress += segment.ToString("x4");

                if (i < addressBytes.Length - 2)
                {
                    fullAddress += ":";
                }
            }

            return fullAddress;
        }
        private bool IsValidIPv6(string address)
        {
            return IPAddress.TryParse(address, out IPAddress ipAddress) && ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
        }
    }
}
