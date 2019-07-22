using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Timers;

namespace SkyrimMasterServer
{
    internal class MasterServer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MasterServer));
        private static IDatabase Database;
        private static Dictionary<string, string> Config;

        private static Timer Watchdog;
        private static Dictionary<string, int> Servers;

        private static void Main()
        {
            BasicConfigurator.Configure();
            Log.Info("log4net started");

            Config = new Dictionary<string, string>();
            try
            {
                using (StreamReader streamReader = new StreamReader(Environment.CurrentDirectory + @"\config\MasterServer.ini"))
                {
                    string line = string.Empty;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line.Length >= 1 && !line.StartsWith("#"))
                        {
                            int num = line.IndexOf('=');

                            if (num != -1)
                                Config.Add(line.Substring(0, num), line.Substring(num + 1));
                        }
                    }

                    Log.Info("config loaded");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            Database = new IDatabase(Config["database.host"], Config["database.port"], Config["database.username"], Config["database.password"], Config["database.dbname"]);

            if (Database.RunQuickNoResult("SELECT 1+1"))
            {
                Log.Info("database connected");
            }
            else
            {
                Log.Error("database not connected, edit config and restart server");
                Console.ReadKey();
                Environment.Exit(0);
            }

            while (true) { }
        }

        private static bool IsListening(int Port)
        {
            IPGlobalProperties Properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] Listeners = Properties.GetActiveUdpListeners();

            foreach (IPEndPoint endpoint in Listeners)
            {
                if (endpoint.Port == Port)
                    return true;
            }

            return false;
        }

        private static void StartServer(string Name, int Port)
        {
            Process Process = new Process();
            Process.StartInfo = new ProcessStartInfo
            {
                FileName = Environment.CurrentDirectory + @"\Server.exe",
                Arguments = "-name \"" + Name + "\" -port " + Port
            };

            Process.Start();
            Console.WriteLine("Started server \"" + Name + "\" on port " + Port + ".");
        }

        private static void Watchdog_Elapsed(object sender, ElapsedEventArgs e)
        {
        }
    }
}
