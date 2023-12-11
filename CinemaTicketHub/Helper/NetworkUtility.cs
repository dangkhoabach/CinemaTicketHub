using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Web;

namespace CinemaTicketHub.Helper
{
    public class NetworkUtility
    {
        public static string GetIPv4Address()
        {
            string ipAddress = String.Empty;

            try
            {
                // Lấy tất cả các địa chỉ IP của máy
                IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());

                // Lọc và lấy địa chỉ IPv4 đầu tiên (nếu có)
                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork) // IPv4
                    {
                        ipAddress = address.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có lỗi
                Console.WriteLine("Error getting IPv4 address: " + ex.Message);
            }

            return ipAddress;
        }
    }
}