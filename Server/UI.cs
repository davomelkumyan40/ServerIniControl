using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.TCP;

namespace Server
{
    public class UI
    {
        public static void ListeningMessage(ServerHost host)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Listening:  {host.Configuration.IpAddress}:{host.Configuration.Port}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void OnMessage(ServerHost host, string message = null)
        {
            Console.WriteLine($"{new string(' ', 5)}Time: [{DateTime.Now.TimeOfDay}]");
            Console.WriteLine($"{new string(' ', 5)}Endpoint: [{host.Configuration.IpAddress}:{host.Configuration.Port}]");
            Console.WriteLine($"{new string(' ', 5)}Connected: [{host.Clients.Count}]");
            Console.WriteLine($"{new string(' ', 5)}Status: [{host.Listening}]");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (!string.IsNullOrEmpty(message))
            {
                Console.Write($"{new string(' ', 5)}Additional Info: [{host.Listening}]");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"{new string(' ', 10)}{message}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void FailedListening(ServerHost host, string message = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed: ");
            Console.ForegroundColor = ConsoleColor.White;
            OnMessage(host, message);
        }

        public static void HelpMessage(ServerHost host)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("HELP: ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            foreach (var cmd in host.Configuration.ServerSideCommands)
            {
                Console.WriteLine($"{new string(' ', 5)}{cmd}");
            }
            Console.WriteLine($"\nCurrent Directory: [{AppDomain.CurrentDomain.BaseDirectory}]");
        }

        public static void StartedMessage(ServerHost host)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Starting Host: ");
            Console.ForegroundColor = ConsoleColor.White;
            OnMessage(host);
        }


        public static void DisplayData(Dictionary<string, string> data)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Loaded: ");

            Console.BackgroundColor = ConsoleColor.Black;
            foreach (var section in data)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write($"\t");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"[{section.Key}]");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" = ");
                Console.WriteLine($"[{section.Value}]");
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void UnknownCommand(string command)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\tUnknown: {command}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void ServerStopedMessage(ServerHost host)
        {
            if (!host.IsRunning)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Stoped: ");
                Console.ForegroundColor = ConsoleColor.White;
                OnMessage(host);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"No Instance on {host.Configuration.IpAddress}:{host.Configuration.Port}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }


        public static void ListenerStopedMessage(ServerHost host)
        {
            if (!host.Listening)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Stoped Listening: [{host.Configuration.IpAddress}:{host.Configuration.Port}]");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"No Instance on {host.Configuration.IpAddress}:{host.Configuration.Port}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void InitUi()
        {
            Console.Title = "Server";
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@$"
{new string(' ', Console.BufferWidth / 2 - 35)}   _|_|_|  _|_|_|_|  _|_|_|    _|      _|  _|_|_|_|  _|_|_|    
{new string(' ', Console.BufferWidth / 2 - 35)} _|        _|        _|    _|  _|      _|  _|        _|    _|  
{new string(' ', Console.BufferWidth / 2 - 35)}   _|_|    _|_|_|    _|_|_|    _|      _|  _|_|_|    _|_|_|    
{new string(' ', Console.BufferWidth / 2 - 35)}       _|  _|        _|    _|    _|  _|    _|        _|    _|  
{new string(' ', Console.BufferWidth / 2 - 35)} _|_|_|    _|_|_|_|  _|    _|      _|      _|_|_|_|  _|    _|  ");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
