using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Laba4
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string subnet = "192.168.1";
            int portToCheck = 80;
            var activeDevices = new System.Collections.Generic.List<string>();

            Console.WriteLine("Сканирую сеть...");

            for (int i = 1; i < 255; i++)
            {
                string ip = $"{subnet}.{i}";
                Ping ping = new Ping();

                try
                {
                    PingReply reply = await ping.SendPingAsync(ip, 100);
                    if (reply.Status == IPStatus.Success)
                    {
                        string hostname = "";
                        try
                        {
                            IPHostEntry hostEntry = await Dns.GetHostEntryAsync(ip);
                            hostname = hostEntry.HostName;
                        }
                        catch
                        {
                            hostname = "unknown";
                        }
                        Console.WriteLine($"Устройство найдено: {ip} ({hostname})");
                        activeDevices.Add(ip);
                    }
                }
                catch
                {

                }
            }

            Console.WriteLine("Выберите устройство для проверки порта: ");
            for (int i =0; i < activeDevices.Count; i++)
            {
                Console.WriteLine($"{i}: {activeDevices[i]}");
            }

            if (activeDevices.Count == 0)
            {
                Console.WriteLine("Устройства не найдены!");
                return;
            }

            Console.WriteLine("Введите номер устройства: ");
            if (int.TryParse(Console.ReadLine(), out int selectedIndex) &&
                selectedIndex >= 0 && selectedIndex < activeDevices.Count)
            {
                string selectedIp = activeDevices[selectedIndex];
                Console.WriteLine($"Проверка порта {portToCheck} на устройстве {selectedIp}...");

                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    try
                    {
                        var connectTask = socket.ConnectAsync(selectedIp, portToCheck);
                        bool connected = connectTask.Wait(500);
                        if (socket.Connected)
                        {
                            Console.WriteLine("Порт открыт!");
                        }
                        else
                        {
                            Console.WriteLine("Порт закрыт или устройство недоступно!");
                        }
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("Порт закрыт или устройство недоступно!");
                    }
                }
            }
            else
            {
                Console.WriteLine("Неправильный выбор!");
            }
        }
    }
}
