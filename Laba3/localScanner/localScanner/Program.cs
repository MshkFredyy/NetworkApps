using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace localScanner
{
    internal class Program
    {
        class PortScanner
        {
            public static async Task ScanPortsAsync(string ipAddress, int startPort, int endPort)
            {
                for (int port = startPort; port <= endPort; port++)
                {
                    bool isOpen = await IsPortOpenAsync(ipAddress, port, 100);
                    if (isOpen)
                    {
                        Console.WriteLine($"Порт {port} открыт");
                    }
                }
            }

            private static async Task<bool> IsPortOpenAsync(string ipAddress, int port, int timeout)
            {
                using (var tcpClient = new TcpClient())
                {
                    try
                    {
                        var connectTask = tcpClient.ConnectAsync(ipAddress, port);
                        var timeoutTask = Task.Delay(timeout);
                        var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                        if (completedTask == connectTask)
                        {
                            // Проверяем, получилось ли подключение
                            return tcpClient.Connected;
                        }
                        else
                        {
                            // Таймаут соединения
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            static async Task Main(string[] args)
            {
                string localIP = "192.168.1.72";
                int startPort = 1;
                int endPort = 1024;

                Console.WriteLine($"Сканирование портов с {startPort} по {endPort} на {localIP}...");
                await ScanPortsAsync(localIP, startPort, endPort);
                Console.WriteLine("Сканирование завершено.");
            }
        }
    }
}
