using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace portCheck
{
    internal class Program
    {
        public static bool IsPortOpen(string host, int port, int timeout = 1000)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(timeout);
                    if (!success)
                        return false;

                    client.EndConnect(result);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        static void Main(string[] args)
        {
            string ip = "192.168.1.72"; 
            int port = 80;

            bool isOpen = IsPortOpen(ip, port);
            Console.WriteLine(isOpen ? "Порт открыт" : "Порт закрыт");
        }
    }
}
