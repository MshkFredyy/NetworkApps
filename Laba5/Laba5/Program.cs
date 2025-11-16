using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Laba5
{
    internal class Program
    {
        // Запуск от имени администратора
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);

            socket.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.72"), 0));

            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

            byte[] inValue = new byte[4] { 1, 0, 0, 0 };
            byte[] outValue = new byte[4];
            socket.IOControl(IOControlCode.ReceiveAll, inValue, outValue);

            byte[] buffer = new byte[4096];

            Console.WriteLine("Начинаем сниффинг...");

            while (true)
            {
                int received = socket.Receive(buffer);
                if (received > 0)
                {
                    
                    int ipHeaderLength = (buffer[0] & 0x0F) * 4; // длина в байтах
                    int totalLength = (buffer[2] << 8) + buffer[3];
                    byte protocol = buffer[9];
                    string sourceIP = new IPAddress(new byte[] { buffer[12], buffer[13], buffer[14], buffer[15] }).ToString();
                    string destIP = new IPAddress(new byte[] { buffer[16], buffer[17], buffer[18], buffer[19] }).ToString();

                    Console.WriteLine($"Получено {received} байт");
                    Console.WriteLine($"IP Длина заголовка: {ipHeaderLength} байт");
                    Console.WriteLine($"Общая длина: {totalLength}");
                    Console.WriteLine($"Протокол: {protocol}"); // 6 - TCP; 17 - UDP
                    Console.WriteLine($"Источник IP: {sourceIP}");
                    Console.WriteLine($"Назначение IP: {destIP}");

                    Console.WriteLine("IP заголовка (hex): " + BitConverter.ToString(buffer, 0, ipHeaderLength));

                }
            }
        }
    }
}
