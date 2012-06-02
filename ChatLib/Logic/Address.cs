using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace ChatLib
{
    public static class Address
    {
        /// <summary>
        /// Gets the local IP of this computer
        /// </summary>
        /// <returns>The local IP address of the computer</returns>
        public static IPAddress LocalIP
        {
            get
            {
                IPHostEntry host;
                IPAddress address = null;
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        address = ip;
                    }
                }
                return address;
            }
        }

        public static IPAddress ExternalIP
        {
            get
            {
                try
                {
                    // Get address
                    const string url = "http://automation.whatismyip.com/n09230945.asp";
                    string address = HttpHelper.Get(url);

                    IPAddress ip;
                    Address.TryParseAddress(address, out ip);
                    return ip;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Try to resolve the host address to get the IP address of it
        /// </summary>
        /// <param name="address">The host address to resolve</param>
        /// <param name="ip">The IP address that was resolved if successful</param>
        /// <returns>If the host address was successfully resolved</returns>
        public static bool ResolveHost(string address, out IPAddress ip)
        {
            try
            {
                IPHostEntry host;
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ipAddress in host.AddressList)
                {
                    if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ip = ipAddress;
                        return true;
                    }
                }
                // if it does not find it
                ip = null;
                return false;
            }
            catch
            {
                ip = null;
                return false;
            }
        }

        /// <summary>
        /// Validates the address to know if it is a valid IP address
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>If the address is a valid IP address</returns>
        public static bool ValidateIP(string address)
        {
            IPAddress ip;
            return IPAddress.TryParse(address, out ip);
        }

        /// <summary>
        /// Validates the address to know if it is a valid host address
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>If the address is a valid host address</returns>
        public static bool ValidateHost(string address)
        {
            IPAddress ip;
            return ResolveHost(address, out ip);
        }

        /// <summary>
        /// Validates the address to know if it is a valid IP address or host address
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>If the address is valid</returns>
        public static bool ValidateAddress(string address)
        {
            // Remove any spaces
            address = address.Replace(" ", string.Empty);

            // Try as an IP first
            bool isIP = ValidateIP(address);

            // Check or run host resolve
            if (isIP) return true;

            bool canResolve = ValidateHost(address);
            if (canResolve) return true;
            else return false;
        }

        /// <summary>
        /// Try to resolve the address to get the IP address of it
        /// </summary>
        /// <param name="address">The IP address or host address to resolve</param>
        /// <param name="ip">The IP address that was resolved if successful</param>
        /// <returns>If the IP address or host address was successfully resolved</returns>
        public static bool TryParseAddress(string address, out IPAddress ip)
        {
            // Remove any spaces
            address = address.Replace(" ", string.Empty);

            // Try as an IP first
            bool isIP = IPAddress.TryParse(address, out ip);

            // Check or run host resolve
            if (isIP) return true;

            bool canResolve = ResolveHost(address, out ip);
            if (canResolve) return true;
            else return false;
        }
    }
}
