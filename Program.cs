using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SkyrimTogetherWatchdog
{
    class Program
    {
        private static Timer watchdog;
        private static int port;

        static void Main(string[] args)
        {
            watchdog = new Timer();
            watchdog.Elapsed += Watchdog_Elapsed;
            watchdog.AutoReset = true;
            watchdog.Interval = 10000;
            watchdog.Start();

            port = 10578;

            Console.WriteLine("Watchdog started. Starting server.");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.CurrentDirectory + @"\RunServer.bat"
                }
            };

            process.Start();

            if (process.Responding)
                Console.WriteLine("Server has been started successfully.");

            while (true)
            { }
        }

        private static void Watchdog_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool NeedsRestart = false;
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var connections = properties.GetActiveUdpListeners();
            List<int> ports = new List<int>();

            foreach (var connx in connections)
                ports.Add(connx.Port);

            if (!ports.Contains(port))
                NeedsRestart = true;

            if (NeedsRestart)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Environment.CurrentDirectory + @"\RunServer.bat"
                    }
                };

                process.Start();

                Console.WriteLine("Server crashed or was not responding. Process killed. Server restarted.");
            }
        }
    }
}
