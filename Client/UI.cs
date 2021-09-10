using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class UI
    {
        public static void SentMessage(Client client)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\nSent: => ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"'{client.RequestMessage}'\n");
        }

        public static void RecivedMessage(Client client)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write($"\nRecived: <= ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"'{client.ResponseMessage}'\n");
        }

        public static void HelpMessage()
        {
            Console.WriteLine("\nTo get current [value] type 'client --get [key]'");
            Console.WriteLine("To set current [value] by [key] type 'client --set [key] [value]'");
            Console.WriteLine("To see full commands list type 'client'\n");
        }
        public static void SuccessMessage(Client client)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nConnected: ");
            OnMessage(client);
        }

        public static void FailedMessage(Client client, string message = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nFailed: ");
            OnMessage(client);
            AddInfo(client, message);
        }

        private static void AddInfo(Client client, string info)
        {
            if (info == null || string.IsNullOrEmpty(info) || string.IsNullOrWhiteSpace(info))
                return;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{new string(' ', 10)}Info: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{info}");
        }

        public static void ServerIsUnableMessage(Client client)
        {
            WarningMessage(client, "Server is unable...");
        }

        public static void WarningMessage(Client client, string message = null)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\nWarning: ");
            Console.ForegroundColor = ConsoleColor.White;
            AddInfo(client, message);
        }

        public static void DefaultMessage()
        {
            Console.WriteLine("\nEnter 'client --help' for additional information");
        }

        public static void DisconnectedMessage(Client client, string message = null)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Disconnected: ");
            OnMessage(client);
        }

        private static void OnMessage(Client client)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{new string(' ', 5)}[{DateTime.Now.TimeOfDay}]");
            Console.WriteLine($"{new string(' ', 5)}IP Endpoint: [{client.Configuration.IpAddress}:{client.Configuration.Port}]");
            Console.WriteLine($"{new string(' ', 5)}Retry: [{client.Configuration.RetryConnection}]");
        }

        public static void InitUi()
        {
            Console.Title = "Client";
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(@$"
            {new string(' ', Console.BufferWidth / 2 - 35)}   _|_|_|  _|  _|                        _|      
            {new string(' ', Console.BufferWidth / 2 - 35)} _|        _|        _|_|    _|_|_|    _|_|_|_|  
            {new string(' ', Console.BufferWidth / 2 - 35)} _|        _|  _|  _|_|_|_|  _|    _|    _|      
            {new string(' ', Console.BufferWidth / 2 - 35)} _|        _|  _|  _|        _|    _|    _|      
            {new string(' ', Console.BufferWidth / 2 - 35)}   _|_|_|  _|  _|    _|_|_|  _|    _|      _|_|  ");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
