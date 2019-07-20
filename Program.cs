using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Timers;

namespace SkyrimTogetherWatchdog
{
    class Program
    {
        private static Timer watchdog;
        private static Dictionary<string, int> servers;

        static void Main(string[] args)
        {
            servers = new Dictionary<string, int>();

            List<string> files = Directory.GetFiles(Environment.CurrentDirectory + @"\cfg").Select(Path.GetFileName).ToList();
            foreach (string file in files)
            {
                string line = string.Empty;

                using (var stream = new StreamReader(Environment.CurrentDirectory + @"\cfg\" + file))
                {
                    while ((line = stream.ReadLine()) != null)
                    {
                        if (line.Length < 1 || line.StartsWith("#"))
                        {
                            continue;
                        }

                        string[] split = line.Split(';');
                        int port = Convert.ToInt32(split[0]);
                        string name = split[1];
                        servers.Add(name, port);
                    }
                }
            }

            if (servers.Count == 0)
            {
                Console.WriteLine("You have no configured servers.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            foreach (KeyValuePair<string, int> pair in servers)
                StartServer(pair.Key, pair.Value);

            watchdog = new Timer();
            watchdog.Elapsed += Watchdog_Elapsed;
            watchdog.AutoReset = true;
            watchdog.Interval = 10000;
            watchdog.Start();

            while (true)
            { }
        }

        private static bool IsListening(int Port)
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var connections = properties.GetActiveUdpListeners();
            List<int> ports = new List<int>();

            foreach (var connx in connections)
                if (connx.Port == Port)
                    return true;

            return false;
        }

        private static void StartServer(string Name, int Port)
        {
            var Process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.CurrentDirectory + @"\Server.exe",
                    Arguments = "-name \"" + Name + "\" -port " + Port + "",
                }
            };

            Process.Start();

            Console.WriteLine("Started server \"" + Name + "\" on port " + Port + ".");
        }

        private static void Watchdog_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (KeyValuePair<string, int> server in servers)
            {
                if (!IsListening(server.Value))
                {
                    StartServer(server.Key, server.Value);
                    Console.WriteLine("Restarted server due to crash: \"" + server.Key + "\" on port " + server.Value + ".");
                }
            }
        }
    }
}
