using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;

namespace SkyrimTogetherWatchdog
{
    class Program
    {
        private static Timer watchdog;

        static void Main(string[] args)
        {
            watchdog = new Timer();
            watchdog.Elapsed += Watchdog_Elapsed;
            watchdog.AutoReset = true;
            watchdog.Interval = 10000;
            watchdog.Start();

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
            Process[] processes = Process.GetProcessesByName("Server");

            if (processes.Count() > 0)
            {
                foreach (Process process in processes)
                {
                    if (!process.Responding || process.HasExited)
                    {
                        process.Kill();
                        NeedsRestart = true;
                    }
                }
            }
            else
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
